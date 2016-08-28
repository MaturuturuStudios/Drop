using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuMapLevel3D : MonoBehaviour {
    #region Public Attributes
    /// <summary>
    /// List of worlds
    /// </summary>
    public GameObject[] worlds;
    /// <summary>
    /// The camera of the canvas
    /// </summary>
    public Camera cameraCanvas;
    /// <summary>
    /// The limits of the camera
    /// </summary>
    public BoxCollider2D limits;
    /// <summary>
    /// Normal distance of the camera in map level
    /// </summary>
    public float normalDistance=-300;
    /// <summary>
    /// Distance of the camera must reach (and come back to normal) when zooming
    /// </summary>
    public float zoomOutDistance=-400;
    /// <summary>
    /// Time of zooming
    /// </summary>
    public float timeZooming=1;
    /// <summary>
    /// The function to zoom. needs to start on zero and finish on zero
    /// </summary>
    public AnimationCurve easeFunctionZoom;
    /// <summary>
    /// Time to wait until the level start
    /// </summary>
    public float waitBeforeStartLevel = 1f;
    /// <summary>
    /// Game object containing the map image and the worlds
    /// </summary>
    public SpriteRenderer imageMap;
   
    /// <summary>
    /// The border of a normal level
    /// </summary>
    public Sprite ringBaseLevel;
    /// <summary>
    /// The border of the last level
    /// </summary>
    public Sprite ringLastLevel;
    /// <summary>
    /// Color for the non available levels
    /// </summary>
    public Color nonAvailableLevelColor;
    /// <summary>
    /// Color for the locked levels
    /// </summary>
    public Color lockedLevelColor;
    /// <summary>
    /// Time to reach the target point
    /// </summary>
    public float durationTravel = 1.0f;
    /// <summary>
    /// How long should  last the fading between worlds
    /// </summary>
    public float speedFading = 2f;
    /// <summary>
    /// alpha for hidden worlds
    /// </summary>
    public float hiddenAlphaWorld = 0.20f;
    #endregion

    #region Private Attributes
    /// <summary>
    /// Data reference
    /// </summary>
    private GameControllerData data;
    /// <summary>
	/// A reference to the menu's navigator.
	/// </summary>
	private MenuNavigator _menuNavigator;
    /// <summary>
    /// The transform of the camera
    /// </summary>
    private Transform transformCamera;
    /// <summary>
    /// All the cameras in the scene
    /// </summary>
    private Camera[] cameras;
    /// <summary>
    /// The previous state of all cameras
    /// </summary>
    private bool[] camerasPreviousState;
    /// <summary>
	/// The levels' panels (canvas).
	/// </summary>
	private CanvasGroup[] levelsCanvas;
    /// <summary>
    /// List of all levels
    /// </summary>
    private List<GameObject[]> levels;
    /// <summary>
    /// Attribute to know if we have to select the level
    /// </summary>
    private bool _selectOption;
    /// <summary>
    /// The actual world and level selected
    /// </summary>
    private LevelInfo actualLevelInfo;
    /// <summary>
    /// Time in which the travel between points started
    /// </summary>
    private float _startTime;
    /// <summary>
    /// Point camera should focus
    /// </summary>
    private Vector2 _targetPoint;
    /// <summary>
    /// Listeners of map selection
    /// </summary>
    private List<MapLevelListener> _listeners = new List<MapLevelListener>();
    #endregion

    #region Private class
    /// <summary>
    /// On select level class to react a events.
    /// </summary>
    private class OnSelectLevel : MonoBehaviour, ISelectHandler {
        /// <summary>
        /// World and level info
        /// </summary>
        public LevelInfo level;
        /// <summary>
        /// Script to call to
        /// </summary>
        public MenuMapLevel3D delegateAction;

        public void OnSelect(BaseEventData eventData) {
            StartCoroutine(Delegate(eventData.selectedObject));
        }

        /// <summary>
        /// Make sure at the end of frame that the final selected level
        /// is this one. This is because when selecting a level, can be non interactable,
        /// so the event is skiping to the next one and this way we avoid animations and
        /// other things starting
        /// </summary>
        /// <param name="selected">the selected object (the level selected)</param>
        /// <returns></returns>
        private IEnumerator Delegate(GameObject selected) {
            yield return new WaitForEndOfFrame();

            if (EventSystem.current.currentSelectedGameObject == selected) {
                delegateAction.SelectLevel(level);
            }
        }
    }
    #endregion

    #region Public Methods
    public void OnEnable() {
        //we have to select the option in update
        _selectOption = true;
        SelectMapCamera(true);
    }

    public void OnDisable() {
        SelectMapCamera(false);
    }

    /// <summary>
	/// Subscribes a listener to the menu map's events.
	/// Returns false if the listener was already subscribed.
	/// </summary>
	/// <param name="listener">The listener to subscribe</param>
	/// <returns>If the listener was successfully subscribed</returns>
	public bool AddListener(MapLevelListener listener) {
        if (_listeners.Contains(listener))
            return false;
        _listeners.Add(listener);
        return true;
    }

    /// <summary>
    /// Unsubscribes a listener to the menu map's events.
    /// Returns false if the listener wasn't subscribed yet.
    /// </summary>
    /// <param name="listener">The listener to unsubscribe</param>
    /// <returns>If the listener was successfully unsubscribed</returns>
    public bool RemoveListener(MapLevelListener listener) {
        if (!_listeners.Contains(listener))
            return false;
        _listeners.Remove(listener);
        return true;
    }

    /// <summary>
    /// Get all levels and prepare the map
    /// </summary>
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
        data = GameObject.FindGameObjectWithTag(Tags.GameData).GetComponent<GameControllerData>();

        levels = new List<GameObject[]>();
        levelsCanvas = new CanvasGroup[worlds.Length];
 

        ConfigureWorlds();

        transformCamera = cameraCanvas.GetComponent<Transform>();

        //show the world but not focus
        actualLevelInfo.world = -1;
        actualLevelInfo.level = -1;
        
        GameObject first = levels[0][0];
        MeshRenderer renderer = first.GetComponentInChildren<MeshRenderer>(true);
        Color color = renderer.material.color;
        color.a = 1;

        for(int i=0; i<levels[0].Length; i++) {
            GameObject aLevel = levels[0][i];
            renderer = aLevel.GetComponentInChildren<MeshRenderer>(true);
            renderer.material.color = color;
        }
        levelsCanvas[0].alpha = 1;



        actualLevelInfo.level = 0;
        actualLevelInfo.world = 0;

        //deactivate map until is needed
        this.gameObject.SetActive(false);
    }

    public void Update() {
        //if we have to select the option...
        if (_selectOption) {
            //only once!
            _selectOption = false;
            //select the option
            //pass the last unlocked converted in index from 0
            //set the initial point (last level unlocked)
            LevelInfo lastLevel = data.GetLastUnlockedLevel();
            Debug.Log(lastLevel.world);
            Debug.Log(lastLevel.level);
            SelectLevel(lastLevel);
            ChangeRingLevel(lastLevel, lastLevel);


            _targetPoint = levels[actualLevelInfo.world][actualLevelInfo.level].transform.position;
            Vector3 targeting = WithinBounds(_targetPoint);
            targeting.z = normalDistance;
            transformCamera.position = targeting;

            //set the distance and start zooming
            Vector3 localPosition = transformCamera.localPosition;
            localPosition.z = normalDistance;
            transformCamera.localPosition = localPosition;

            StartCoroutine(Zoom());
        }

        //B, return, start
        if (Input.GetButtonDown(Axis.Irrigate) || Input.GetButtonDown(Axis.Back)
            || Input.GetButtonDown(Axis.Start)) {
            _menuNavigator.ComeBack();
        }

        //move the camera to the target
        Vector3 actualPosition = cameraCanvas.transform.position;
        Vector3 target = WithinBounds(_targetPoint);

        //calculate position...
        float percentageTime = (Time.unscaledTime - _startTime) / durationTravel;
        float positionX = Mathf.SmoothStep(actualPosition.x, target.x, percentageTime);
        float positionY = Mathf.SmoothStep(actualPosition.y, target.y, percentageTime);

        Vector3 newPosition = new Vector3(positionX, positionY, actualPosition.z);
        cameraCanvas.transform.position = newPosition;
    }

    /// <summary>
    /// Get the number of worlds
    /// </summary>
    /// <returns></returns>
    public int GetNumberWorlds() {
        return worlds.Length;
    }

    /// <summary>
    /// Get the number of levels of that world
    /// </summary>
    /// <param name="world">index of world from 1 to max number</param>
    /// <returns></returns>
    public int GetNumberLevels(int world) {
        if (world < 1 || world > worlds.Length) return 0;
        return levels[world].Length;
    }

    /// <summary>
    /// Selects the level.
    /// The levels of the world appears and center to the concrete level
    /// </summary>
    /// <param name="world">World.</param>
    /// <param name="level">Level.</param>
    public void SelectLevel(LevelInfo info) {
        if (info.world < 0 || info.level < 0 || info.world >= worlds.Length
            || info.level >= levels[info.world].Length) return;

        foreach (MapLevelListener listener in _listeners)
            listener.OnChangeLevel(actualLevelInfo, info);


        //store the actual level
        actualLevelInfo.level = info.level;
        //show the world
        ShowLevels(info.world);

        //if has a change of world, zoom in and out
        if (actualLevelInfo.world != info.world) {
            StartCoroutine(Zoom());

            foreach (MapLevelListener listener in _listeners)
                listener.OnChangeWorld(actualLevelInfo.world, info.world);
        }

        //store the actual world
        actualLevelInfo.world = info.world;
        _startTime = Time.unscaledTime;
        //focus the selected level
        //has no effect if it was already selected
        FocusLevel(info);

        //get the target point
        _targetPoint = levels[info.world][info.level].transform.position;
    }

    /// <summary>
    /// Tell the listeners a level is selected
    /// </summary>
    /// <param name="level"></param>
    public void NotifyListenersSelectedLevel(LevelInfo level) {
        foreach (MapLevelListener listener in _listeners)
            listener.OnSelectedLevel(level);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Select or deselect the camera of the map, modifing the rest of the cameras
    /// </summary>
    /// <param name="activate">true if want the camera map activated</param>
    private void SelectMapCamera(bool activate) {
        if (activate) {
            //get control of the camera
            //get all cameras and store its status
            cameras = new Camera[Camera.allCamerasCount];
            Camera.GetAllCameras(cameras);
            camerasPreviousState = new bool[cameras.Length];

            for (int i = 0; i < cameras.Length; i++) {
                camerasPreviousState[i] = cameras[i].gameObject.activeSelf;
                //want all of them disabled except our camera
                if (cameraCanvas == cameras[i]) continue;
                cameras[i].gameObject.SetActive(false);
                cameras[i].enabled = false;
            }

            //rare case in which the script is disabled before having an enable situation
        } else if(cameras!=null){
            //restore status cameras
            for (int i = 0; i < cameras.Length; i++) {
                cameras[i].gameObject.SetActive(camerasPreviousState[i]);
                cameras[i].enabled = true;
            }
        }

        //disable or enable map's camera
        cameraCanvas.enabled = activate;
    }

    /// <summary>
    /// Focus the level (like if the user has put the selection over the level)
    /// </summary>
    /// <param name="world"></param>
    /// <param name="level"></param>
    private void FocusLevel(LevelInfo info) {
        if (EventSystem.current.currentSelectedGameObject != levels[info.world][info.level])
            EventSystem.current.SetSelectedGameObject(levels[info.world][info.level]);
    }

    /// <summary>
    /// Change the rings of base and next level
    /// </summary>
    /// <param name="previousWorld">index from 0</param>
    /// <param name="previousLevel">index from 0</param>
    /// <param name="world">index from 0</param>
    /// <param name="level">index from 0</param>
    private void ChangeRingLevel(LevelInfo previous, LevelInfo actual) {
        //check index...
        if (previous.level < 0 || previous.world < 0 || actual.world < 0 || actual.level < 0) return;
        if (previous.world >= worlds.Length || actual.world >= worlds.Length
            || actual.level >= levels[actual.world].Length || previous.level >= levels[previous.world].Length) return;

        //quit the ring to previous level
        levels[previous.world][previous.level].GetComponentInChildren<SpriteRenderer>().sprite = ringBaseLevel;

        //put the ring to the new level
        levels[actual.world][actual.level].GetComponentInChildren<SpriteRenderer>().sprite = ringLastLevel;
    }

    /// <summary>
	/// Shows the levels of a world but not change the view point
	/// </summary>
	/// <param name="world">World.</param>
	private void ShowLevels(int world) {
        //if already actived, return
        if (actualLevelInfo.world == world) return;

        //fade in the selected world
        StartCoroutine(FadeIn(levelsCanvas[world], world));
    }

    /// <summary>
    /// Hidde or not the text
    /// </summary>
    /// <param name="world">the world to hidde</param>
    /// <param name="hidden">true if hidde the text false otherwise</param>
    private IEnumerator HiddeText(int world, bool hidden) {
        if (world >= 0) {
            float alpha = (hidden) ? 0:1;
            bool done = false;
            do {
                GameObject first = levels[world][0];
                MeshRenderer renderer = first.GetComponentInChildren<MeshRenderer>(true);
                Color color = renderer.material.color;
                color.a = Mathf.MoveTowards(color.a, alpha, Time.unscaledDeltaTime * speedFading);

                if (Mathf.Abs(color.a - alpha) < 0.01) done = true;

                foreach (GameObject aLevel in levels[world]) {
                    renderer = aLevel.GetComponentInChildren<MeshRenderer>(true);
                    renderer.material.color = color;
                }
                yield return null;
            } while (!done);
        }
    }

    /// <summary>
    /// Fade int a canvas group
    /// </summary>
    /// <param name="canvas"></param>
    private IEnumerator FadeIn(CanvasGroup canvas, int fadingInWorld) {
        if (canvas != null) {
            StartCoroutine(HiddeText(fadingInWorld, false));

            while (canvas.alpha < 1) {
                //fading...
                canvas.alpha = Mathf.MoveTowards(canvas.alpha, 1, Time.unscaledDeltaTime * speedFading);
                yield return null;
            }
        }
    }

    /// <summary>
    /// Fade out a canvas group
    /// </summary>
    /// <param name="canvas"></param>
    private IEnumerator FadeOut(CanvasGroup canvas, int fadingOutWorld) {
        if (canvas != null) {
            //hide all text
            StartCoroutine(HiddeText(fadingOutWorld, true));

            while (canvas.alpha > hiddenAlphaWorld) {
                //fading...
                canvas.alpha = Mathf.MoveTowards(canvas.alpha, hiddenAlphaWorld, Time.unscaledDeltaTime * speedFading);
                yield return null;
            }
        }
    }

    /// <summary>
    /// Return the position corrected to be inside the map
    /// </summary>
    /// <param name="position">the target position</param>
    /// <returns>the corrected position</returns>
    private Vector3 WithinBounds(Vector3 position) {
        Vector3 target = Vector3.zero;

        //z axis of the camera
        Vector3 positionCamera = transformCamera.position;
        //distance of the map from the camera
        float distance = Mathf.Abs(positionCamera.z);

        //vertex of the camera views
        Vector3 upperLeftCamera = cameraCanvas.ViewportToWorldPoint(new Vector3(0, 1, distance));
        Vector3 downRightCamera = cameraCanvas.ViewportToWorldPoint(new Vector3(1, 0, distance));

        //half size of the camera view
        float different = Mathf.Abs(upperLeftCamera.x) - Mathf.Abs(positionCamera.x);
        float halfWidth = Mathf.Abs(different);
        different = Mathf.Abs(downRightCamera.y) - Mathf.Abs(positionCamera.y);
        float halfHeight = Mathf.Abs(different);
        
        //bounds of the map
        Bounds boundMap = limits.bounds;
        
        //don't get out of the map
        target.x = Mathf.Clamp(position.x, boundMap.min.x + halfWidth, boundMap.max.x - halfWidth);
        target.y = Mathf.Clamp(position.y, boundMap.min.y + halfHeight, boundMap.max.y - halfHeight);

        return target;
    }

    /// <summary>
    /// Configure the worlds
    /// </summary>
    private void ConfigureWorlds() {
        for (int i = 0; i < worlds.Length; i++) {
            GameObject aWorld = worlds[i];

            //get the canvas of the world
            levelsCanvas[i] = aWorld.GetComponent<CanvasGroup>();
            //hide it
            levelsCanvas[i].alpha = hiddenAlphaWorld;
            //configure the levels of the world
            ConfigureLevels(i);
        }
        //put the special ring
        LevelInfo lastUnlocked = data.GetLastUnlockedLevel();
        ChangeRingLevel(lastUnlocked, lastUnlocked);
    }

    /// <summary>
    /// Configures the levels.
    /// </summary>
    /// <param name="world">World.</param>
    private void ConfigureLevels(int world) {
        GameObject allLevelWorld = worlds[world];

        int i = -1;
        levels.Add(new GameObject[allLevelWorld.transform.childCount - 1]);

        LevelInfo theLevel;
        theLevel.world = world;

        //for each level...
        foreach (Transform childTransform in allLevelWorld.transform) {
            //not the first one (title)
            if (i == -1) {
                i++;
                continue;
            }
            theLevel.level = i;

            //get the object of the level
            GameObject child = childTransform.gameObject;
            //store it
            levels[world][i] = child;

            //add a script to watch selection over the level
            child.AddComponent(typeof(OnSelectLevel));
            OnSelectLevel script = child.GetComponent<OnSelectLevel>();

            script.level.world= world; //which world belongs
            script.level.level = i; //number of level

            //get the text and hidde it
            MeshRenderer renderer = child.GetComponentInChildren<MeshRenderer>(true);
            Color color = renderer.material.color;
            color.a = 0;
            renderer.material.color = color;

            //check if the level is locked (or non-available)
            if (!data.IsUnlockedlevel(theLevel)) {
                Button button = child.GetComponent<Button>();
                ColorBlock colorBlock = button.colors;
                colorBlock.disabledColor = lockedLevelColor;
                button.interactable = false;
                button.colors = colorBlock;
            }

            //check if the level is non-available (put it grey)
            if (!data.IsAvailableLevel(theLevel)) {
                Button button = child.GetComponent<Button>();
                ColorBlock colorBlock = button.colors;
                colorBlock.disabledColor = nonAvailableLevelColor;
                button.interactable = false;
                button.colors = colorBlock;
            }

            script.delegateAction = this; //script to delegate

            //get the script to give the scene asociated to this level
            //and few information
            SceneLevel sceneLevel = child.GetComponent<SceneLevel>();
            sceneLevel.waitBeforeStartLevel = waitBeforeStartLevel;
            sceneLevel.level = data.GetScene(theLevel);
            sceneLevel.infoLevel = theLevel;
            sceneLevel.menuMap = this;

            i++;
        }
    }
    
    /// <summary>
    /// Make a zoom
    /// </summary>
    /// <returns></returns>
    private IEnumerator Zoom() {
        float startTime = Time.unscaledTime;

        float percentageTime = (Time.unscaledTime - startTime) / timeZooming;
        do {
            Vector3 position = transformCamera.localPosition;
            position.z = Mathf.Lerp(normalDistance, zoomOutDistance, easeFunctionZoom.Evaluate(percentageTime));
            transformCamera.localPosition = position;
            yield return null;
            percentageTime = (Time.unscaledTime - startTime) / timeZooming;
        } while (percentageTime < 1);
    }
    #endregion
}
