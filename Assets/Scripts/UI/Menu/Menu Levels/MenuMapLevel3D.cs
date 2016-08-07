using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MenuMapLevel3D : MonoBehaviour {
    #region Public Attributes
    /// <summary>
    /// The camera of the canvas
    /// </summary>
    public Camera cameraCanvas;
    /// <summary>
    /// The limits of the camera
    /// </summary>
    public BoxCollider2D limits;

    /// <summary>
    /// Game object containing the map image and the worlds
    /// </summary>
    public SpriteRenderer imageMap;
    /// <summary>
    /// List of worlds
    /// </summary>
	public GameObject[] worlds;

    /// <summary>
    /// The border of a normal level
    /// </summary>
    public Sprite ringBaseLevel;
    /// <summary>
    /// The border of the last level
    /// </summary>
    public Sprite ringLastLevel;
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
    /// The actual world actived.
    /// Starting from 0
    /// </summary>
    private int actualWorldActive = 0;
    /// <summary>
    /// Actual level selected.
    /// Start from 0
    /// </summary>
    private int actualLevel = 0;
    /// <summary>
    /// Time in which the travel between points started
    /// </summary>
    private float _startTime;
    /// <summary>
    /// Point camera should focus
    /// </summary>
    private Vector2 _targetPoint;
    #endregion

    #region Private class
    /// <summary>
    /// On select level class to react a events.
    /// </summary>
    private class OnSelectLevel : MonoBehaviour, ISelectHandler {
        /// <summary>
        /// World the level belongs to
        /// </summary>
        public int world;
        /// <summary>
        /// level of the selectable
        /// </summary>
        public int level;
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
                delegateAction.SelectLevel(world, level);
            }
        }
    }
    #endregion

    #region Public Methods
    public void OnEnable() {
        //we have to select the option in update
        _selectOption = true;
        SelectMapCamera(true);
        
        //TODO select level       
    }

    public void OnDisable() {
        SelectMapCamera(false);
    }

    /// <summary>
    /// Get all levels and prepare the map
    /// </summary>
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
        levels = new List<GameObject[]>();
        levelsCanvas = new CanvasGroup[worlds.Length];
        actualLevel = 0;
        actualWorldActive = 0;

        ConfigureWorlds();

        transformCamera = cameraCanvas.GetComponent<Transform>();
    }

    public void Update() {
        //if we have to select the option...
        if (_selectOption) {
            //only once!
            _selectOption = false;
            //select the option
            //pass the last unlocked converted in index from 0
            SelectLevel(actualWorldActive, actualLevel);
            //TODO: seleccionar el último nivel disponible
            ChangeRingLevel(0, 0, actualWorldActive, actualLevel);
        }

        //B, return, start
        if (Input.GetButtonDown(Axis.Irrigate) || Input.GetButtonDown(Axis.Back)
            || Input.GetButtonDown(Axis.Start)) {
            _menuNavigator.ComeBack();
        }

        //fade if needed the levels
        //FadeIn();
        //FadeOut();

        //move the camera to the target
        Vector3 actualPosition = cameraCanvas.transform.position;

        //TODO zoom in zoom out

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
    public void SelectLevel(int world, int level = 0) {
        if (world < 0 || level < 0 || world >= worlds.Length
            || level >= levels[world].Length) return;

        //store the actual level
        actualLevel = level;
        //show the world
        ShowLevels(world);
        //store the actual world
        actualWorldActive = world;
        _startTime = Time.unscaledTime;
        //focus the selected level
        FocusLevel(world, level);

        //get the target point
        _targetPoint = levels[world][level].transform.position;
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

        } else {
            //restore status cameras
            for (int i = 0; i < cameras.Length; i++) {
                cameras[i].gameObject.SetActive(camerasPreviousState[i]);
                cameras[i].enabled = true;
            }
        }

        //disable or enable map's camera
        cameraCanvas.enabled = activate;
    }

    private void FocusLevel(int world, int level) {
        if (EventSystem.current.currentSelectedGameObject != levels[world][level])
            EventSystem.current.SetSelectedGameObject(levels[world][level]);
    }

    /// <summary>
    /// Change the rings of base and next level
    /// </summary>
    /// <param name="previousWorld">index from 0</param>
    /// <param name="previousLevel">index from 0</param>
    /// <param name="world">index from 0</param>
    /// <param name="level">index from 0</param>
    private void ChangeRingLevel(int previousWorld, int previousLevel, int world, int level) {
        //check index...
        if (previousLevel < 0 || previousWorld < 0 || world < 0 || level < 0) return;
        if (previousWorld >= worlds.Length || world >= worlds.Length
            || level >= levels[world].Length || previousLevel >= levels[previousWorld].Length) return;

        //quit the ring to previous level
        levels[previousWorld][previousLevel].GetComponentInChildren<SpriteRenderer>().sprite = ringBaseLevel;

        //put the ring to the new level
        levels[world][level].GetComponentInChildren<SpriteRenderer>().sprite = ringLastLevel;
    }

    /// <summary>
	/// Shows the levels of a world but not change the view point
	/// </summary>
	/// <param name="world">World.</param>
	private void ShowLevels(int world) {
        //if already actived, return
        if (actualWorldActive == world)
            return;

        //if any world actived, fade it out
        if (actualWorldActive >= 0)
            StartCoroutine(FadeOut(levelsCanvas[actualWorldActive], actualWorldActive));
        
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
            float startTime = Time.unscaledDeltaTime;
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
    }

    /// <summary>
    /// Configures the levels.
    /// </summary>
    /// <param name="world">World.</param>
    private void ConfigureLevels(int world) {
        GameObject allLevelWorld = worlds[world];

        int i = -1;
        //levelPositions.Add(new Vector2[allLevelWorld.transform.childCount-1]);
        levels.Add(new GameObject[allLevelWorld.transform.childCount - 1]);

        //for each level...
        foreach (Transform childTransform in allLevelWorld.transform) {
            //not the first one (title)
            if (i == -1) {
                i++;
                continue;
            }

            GameObject child = childTransform.gameObject;
            levels[world][i] = child;

            //add a script to watch selection over the level
            child.AddComponent(typeof(OnSelectLevel));
            OnSelectLevel script = child.GetComponent<OnSelectLevel>();

            script.world = world; //which world belongs
            script.level = i; //number of level

            //TODO
            //if (world > lastUnlockedWorld - 1) {
            //    child.GetComponent<Button>().interactable = false;
            //} else if (world == lastUnlockedWorld - 1) {
            //    //check level
            //    if (i > lastUnlockedLevel - 1) {
            //        child.GetComponent<Button>().interactable = false;
            //    } else if (i == lastUnlockedLevel - 1) { //last level

            //    }
            //}

            script.delegateAction = this; //script to delegate
            i++;
        }
    }
    #endregion
}
