using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MenuMapLevel : MonoBehaviour {
    #region Public Attributes
    
    /// <summary>
    /// The camera of the canvas
    /// </summary>
    public Camera cameraCanvas;
    /// <summary>
    /// Game object containing the map image and the worlds
    /// </summary>
    public SpriteRenderer imageMap;
    /// <summary>
    /// The map with image and levels to be resized
    /// </summary>
    public GameObject mapResizing;
    /// <summary>
    /// The border of a normal level
    /// </summary>
    public Sprite ringBaseLevel;
    /// <summary>
    /// The border of the las level
    /// </summary>
    public Sprite ringSelectedLevel;
    /// <summary>
    /// List of worlds
    /// </summary>
	public GameObject[] worlds;
    /// <summary>
    /// The last unlocked world
    /// </summary>
    public int lastUnlockedWorld = 1;
    /// <summary>
    /// the last unlocked level of the last unlocked world
    /// </summary>
    public int lastUnlockedLevel = 2;
    /// <summary>
    /// alpha for hidden worlds
    /// </summary>
    public float hiddenAlphaWorld = 0.20f;
    /// <summary>
    /// Time to reach the target point
    /// </summary>
    public float durationTravel = 1.0f;
    /// <summary>
    /// How long should  last the fading between worlds
    /// </summary>
    public float speedFading = 2f;
    /// <summary>
    /// Duration of zoom
    /// </summary>
    public float durationZoom = 0.8f;
    /// <summary>
    /// The desired zoom between levels
    /// </summary>
    public float _targetZoom = 0.8f;
    #endregion


    #region Private Attributes
    /// <summary>
    /// Keep track of offset when zooming
    /// </summary>
    private Vector3 offsetApplied;
    /// <summary>
    /// Is zooming?
    /// </summary>
    private bool zooming = false;
    /// <summary>
    /// When zoom started
    /// </summary>
    private float _startTimeZoom;
    /// <summary>
    /// Original scale
    /// </summary>
    private float scale;
    /// <summary>
    /// Corner of map at original size
    /// </summary>
    private Vector2 bottomLeftInitial;
    /// <summary>
    /// All the cameras in the scene
    /// </summary>
    private Camera[] cameras;
    /// <summary>
    /// The previous state of all cameras
    /// </summary>
    private bool[] camerasPreviousState;
    /// <summary>
    /// List of positions for all the levels
    /// </summary>
    //private List<Vector2[]> levelPositions;
    /// <summary>
    /// List of all levels
    /// </summary>
    private List<GameObject[]> levels;
    /// <summary>
    /// The previous world actived.
    /// </summary>
    private int actualWorldActive = 0;
    /// <summary>
    /// 
    /// </summary>
    private int actualLevel=0;
	/// <summary>
	/// The levels' panels (canvas).
	/// </summary>
	private CanvasGroup[] levelsCanvas;
    /// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;
    /// <summary>
    /// Point camera should focus
    /// </summary>
    private Vector2 _targetPoint;
    /// <summary>
    /// Time in which the travel between points started
    /// </summary>
    private float _startTime;
    /// <summary>
    /// The canvas that is fading in
    /// </summary>
    private CanvasGroup canvasFadeIn = null;
    /// <summary>
    /// The canvas that is fading out
    /// </summary>
    private CanvasGroup canvasFadeOut = null;
    /// <summary>
	/// A reference to the menu's navigator.
	/// </summary>
	private MenuNavigator _menuNavigator;
    #endregion

    #region Private class
    /// <summary>
    /// On select level class to react a events.
    /// </summary>
    private class OnSelectLevel: MonoBehaviour, ISelectHandler {
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
        public MenuMapLevel delegateAction;

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

        //get control of the camera
        //get all cameras and store its status
        cameras = new Camera[Camera.allCamerasCount];
        Camera.GetAllCameras(cameras);
        camerasPreviousState = new bool[cameras.Length];
        for(int i=0; i<cameras.Length; i++) {
            camerasPreviousState[i] = cameras[i].gameObject.activeSelf;
            //want all of them disabled except our camera
            if (cameraCanvas == cameras[i]) continue;
            cameras[i].gameObject.SetActive(false);
            cameras[i].enabled = false;
        }
        
        //get the camera of menu map actived
        cameraCanvas.enabled = true;
    }

    public void OnDisable() {
        //if offset, recover it and reset, just in case
        if (offsetApplied != Vector3.zero) {
            //recover the applied offset and reset it
            Vector3 actualPosition = cameraCanvas.transform.position;
            actualPosition -= offsetApplied;
            cameraCanvas.transform.position = actualPosition;
            offsetApplied = Vector3.zero;
        }

        //restore status cameras
        for (int i = 0; i < cameras.Length; i++) {
            cameras[i].gameObject.SetActive(camerasPreviousState[i]);
            cameras[i].enabled = true;
        }

        //disable camera
        cameraCanvas.enabled = false;
    }

    public void Awake() {
        
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();

        levelsCanvas = new CanvasGroup[worlds.Length];
		//levelPositions = new List<Vector2[]>();
        levels= new List<GameObject[]>();
        
        ConfigureWorlds();

        scale = mapResizing.transform.localScale.x;

        //initial corner
        Vector3 position = imageMap.transform.position;
        bottomLeftInitial = position + imageMap.bounds.min;
    }

    public void Update() {
        //if we have to select the option...
        if (_selectOption) {
            //only once!
            _selectOption = false;
            //select the option
            //pass the last unlocked converted in index from 0
            SelectLevel(lastUnlockedWorld - 1, lastUnlockedLevel - 1);
            ChangeRingLevel(0, 0, lastUnlockedWorld - 1, lastUnlockedLevel - 1);
        }

        //B, return, start
        if (Input.GetButtonDown(Axis.Irrigate) || Input.GetButtonDown(Axis.Back)
            || Input.GetButtonDown(Axis.Start)) {
            _menuNavigator.ComeBack();
        }

        //fade if needed the levels
        FadeIn();
        FadeOut();

        //make the zoom
        Zoom();

        //move the camera to the target
        Vector3 actualPosition = cameraCanvas.transform.position;
        actualPosition -= offsetApplied; //quit the offset applied
        //calculate position...
        float percentageTime = (Time.unscaledTime - _startTime) / durationTravel;
        float positionX = Mathf.SmoothStep(actualPosition.x, _targetPoint.x, percentageTime);
        float positionY = Mathf.SmoothStep(actualPosition.y, _targetPoint.y, percentageTime);

        Vector3 newPosition = new Vector3(positionX, positionY, actualPosition.z);

        //get the offset for the zoom
        Vector3 position = imageMap.transform.position;
        Vector2 sizeMap = position + imageMap.bounds.min;

        Vector3 offset = bottomLeftInitial-sizeMap;
        offset.x = Mathf.Abs(offset.x);
        offset.y = Mathf.Abs(offset.y);
        offset /= 2.0f;
        offset.z = 0;

        offsetApplied = offset;
        newPosition += offset;
        Vector3 target;
        WithinBounds(newPosition, out target);
        target.z = actualPosition.z;
        cameraCanvas.transform.position = target;

    }
    

    /// <summary>
    /// Gets the actual world selected.
    /// </summary>
    /// <returns>The actual world.</returns>
    public int GetActualWorld() {
        return actualWorldActive;
    }

    /// <summary>
    /// Get the actual level
    /// </summary>
    /// <returns></returns>
    public int GetActualLevel() {
        return actualLevel;
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
        if (world<1 || world > worlds.Length) return 0;
        return levels[world].Length;
    }

    /// <summary>
    /// Unlock/Lock the whole world
    /// If out of index, do nothing
    /// </summary>
    /// <param name="world">index from 1 to number of worlds</param>
    /// <param name="unlock">true if unlock the level, false otherwise</param>
    public void UnlockWorld(int world, bool unlock = true) {
        world--;
        if (world < 0 || world > worlds.Length - 1) return;

        //activate all levels
        foreach (GameObject level in levels[world]) {
            level.GetComponent<Button>().interactable = unlock;
        }

        //store the last level and world
        if (world + 1 > lastUnlockedWorld) {
            lastUnlockedWorld = world + 1;
            lastUnlockedLevel = levels[world].Length;
        }
    }

    /// <summary>
    /// Lock or unlock a concrete level from a world
    /// If out of index, do nothing
    /// </summary>
    /// <param name="world">index from 1 to number of worlds</param>
    /// <param name="level">index from 1 to number of levels of that world</param>
    /// <param name="unlock">true if unlock the level, false otherwise</param>
    public void UnlockLevel(int world, int level, bool unlock = true) {
        //is passed from index 1, we treat it from index 0
        world--;
        level--;
        if (world < 0 || world > worlds.Length - 1 || level<0 || level >= levels[world].Length) return;
        levels[world][level].GetComponent<Button>().interactable = unlock;

        if (world + 1 > lastUnlockedWorld && level + 1 > lastUnlockedLevel) {
            ChangeRingLevel(lastUnlockedWorld-1,lastUnlockedLevel-1, world, level);
            //last unlocked start from 1
            lastUnlockedWorld = world + 1;
            lastUnlockedLevel = level + 1;
        }
    }

    /// <summary>
    /// Selects the level in the actual world
    /// </summary>
    /// <param name="level">Level from index 0</param>
    public void SelectLevel(int level) {
        if (level < 0 || level >= levels[GetActualWorld()].Length) return;
        SelectLevel(GetActualWorld(), level);
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
        
        actualLevel = level;
        ShowLevels(world);
        actualWorldActive = world;
        _startTime = Time.unscaledTime;
        FocusLevel(world, level);
        
        //get the target point (needed at normal scale, so calculate it
        Vector3 originalSize= mapResizing.transform.localScale = new Vector3(scale, scale, scale);
        _targetPoint = GetTargetPoint(world, level);
        mapResizing.transform.localScale = originalSize;

    }
    #endregion


    #region Private methods
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
            ||level >=levels[world].Length || previousLevel >= levels[previousWorld].Length) return;
        
        //quit the ring to previous level
        levels[previousWorld][previousLevel].GetComponentInChildren<SpriteRenderer>().sprite = ringBaseLevel;

        //put the ring to the new level
        levels[world][level].GetComponentInChildren<SpriteRenderer>().sprite = ringSelectedLevel;
    }

    /// <summary>
    /// Configure the worlds
    /// </summary>
    private void ConfigureWorlds() {
		for (int i = 0; i < worlds.Length; i++) {
            GameObject aWorld = worlds[i];

			//get the canvas of the world
			levelsCanvas[i] = aWorld.GetComponent<CanvasGroup>();
			//configure the levels of the world
			ConfigureLevels(i);
        }
    } 

	/// <summary>
	/// Configures the levels.
	/// Need to be called in order (0 to last)
	/// </summary>
	/// <param name="world">World.</param>
    private void ConfigureLevels(int world) {
		GameObject allLevelWorld = worlds[world];

        int i = -1;
		//levelPositions.Add(new Vector2[allLevelWorld.transform.childCount-1]);
        levels.Add(new GameObject[allLevelWorld.transform.childCount-1]);

        //for each level...
        foreach (Transform childTransform in allLevelWorld.transform) {
            //not the first one (title)
            if (i == -1) {
                i++;
                continue;
            }

            GameObject child = childTransform.gameObject;
            levels[world][i] = child;
            //levelPositions[world][i] = childTransform.position;

            //add a script to watch selection over the level
            child.AddComponent(typeof(OnSelectLevel));
            OnSelectLevel script = child.GetComponent<OnSelectLevel>();

            script.world = world; //which world belongs
            script.level = i; //number of level

            if(world > lastUnlockedWorld-1) {
                child.GetComponent<Button>().interactable = false;
            } else if (world == lastUnlockedWorld-1) {
                //check level
                if (i > lastUnlockedLevel - 1) {
                    child.GetComponent<Button>().interactable = false;
                } else if (i == lastUnlockedLevel - 1) { //last level

                }
            }
            
            script.delegateAction = this; //script to delegate
            i++;
        }
	}

    /// <summary>
    /// calculate the point to reach always between bounds
    /// </summary>
    /// <param name="world"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    private Vector3 GetTargetPoint(int world, int level) {
        //the point we should center
        Vector2 position = levels[world][level].transform.position;
        Vector3 target;
        WithinBounds(position, out target);
        return target;
    }

    /// <summary>
    /// Return the position corrected to be inside the map
    /// </summary>
    /// <param name="position"></param>
    /// <param name="target"></param>
    private void WithinBounds(Vector3 position, out Vector3 target) {
        target = Vector3.zero;
        // Calculate if it is out of bounds
        Vector2 halfSizeCamera = Vector2.zero;
        halfSizeCamera.y = cameraCanvas.orthographicSize;
        halfSizeCamera.x = (cameraCanvas.orthographicSize * 16) / 9;

        //limits of the image map
        Vector2 positionImage = imageMap.transform.position;
        Vector2 sizeMap = imageMap.bounds.extents;
        float top = positionImage.y + sizeMap.y - halfSizeCamera.y;
        float left = positionImage.x - sizeMap.x + halfSizeCamera.x;
        float bottom = positionImage.y - sizeMap.y + halfSizeCamera.y;
        float right = positionImage.x + sizeMap.x - halfSizeCamera.x;

        //don't get out of the map
        target.x = Mathf.Clamp(position.x, left, right);
        target.y = Mathf.Clamp(position.y, bottom, top);
    }

	/// <summary>
	/// Shows the levels of a world but not change the view point
	/// </summary>
	/// <param name="world">World.</param>
	private void ShowLevels(int world){
		if (actualWorldActive == world)
			return;

        if (actualWorldActive >= 0)
            FadeOut(levelsCanvas[actualWorldActive]);
        
        FadeIn(levelsCanvas[world]);

        //if change of world, make a zoom effect
        Zoom(true);
    }
    
    /// <summary>
    /// Fade int a canvas group
    /// </summary>
    /// <param name="canvas"></param>
    public void FadeIn(CanvasGroup canvas=null) {
        if (canvas == null && canvasFadeIn == null) return;

        if(canvas!=null && canvasFadeIn != null) {
            canvasFadeIn.alpha = 1;
        }

        //if first time, start counter
        if (canvas != null) {
            canvasFadeIn = canvas;
        }

        //fading...
        canvasFadeIn.alpha = Mathf.Lerp(canvasFadeIn.alpha, 1, Time.unscaledDeltaTime*speedFading);

        //finished
        if (canvasFadeIn.alpha >= 1) canvasFadeIn = null;
        
    }
    
    /// <summary>
    /// Fade out a canvas group
    /// </summary>
    /// <param name="canvas"></param>
    public void FadeOut(CanvasGroup canvas=null) {
        if (canvas == null && canvasFadeOut == null) return;

        if (canvas != null && canvasFadeOut != null) {
            canvasFadeOut.alpha = hiddenAlphaWorld;
        }

        //if first time, start counter
        if (canvas != null) {
            canvasFadeOut = canvas;
        }

        //fading...
        canvasFadeOut.alpha = Mathf.Lerp(canvasFadeOut.alpha, hiddenAlphaWorld, Time.unscaledDeltaTime*speedFading);

        //finished
        if (canvasFadeOut.alpha <= hiddenAlphaWorld) canvasFadeOut = null;
    }

    /// <summary>
    /// Focus the level (input focus)
    /// </summary>
    /// <param name="world"></param>
    /// <param name="level"></param>
    private void FocusLevel(int world, int level) {
        if (EventSystem.current.currentSelectedGameObject != levels[world][level])
            EventSystem.current.SetSelectedGameObject(levels[world][level]);
    }

    /// <summary>
    /// llamado desde show level con startZoom true
    /// y continuado en update (startzoom false)
    /// </summary>
    /// <param name="startZoom"></param>
    private void Zoom(bool startZoom = false) {
        if (!startZoom && !zooming) return;

        //start zooming
        if (startZoom) {
            _startTimeZoom = Time.unscaledTime;
            zooming = true;
        }

        //scale the map
        //float actualScale = mapResizing.transform.localScale.x;
        float newScale = 0;
        //update value
        float percentageTime = (Time.unscaledTime - _startTimeZoom) / durationZoom;

        //check if zoom in or out
        if (percentageTime < 0.5) {
            newScale = Mathf.SmoothStep(scale, _targetZoom, percentageTime * 2.0f);
        } else if (percentageTime > 1f) {
            zooming = false;
            newScale = scale;
        } else {
            newScale = Mathf.SmoothStep(_targetZoom, scale, (percentageTime - 0.5f) * 2.0f);
        }

        mapResizing.transform.localScale = new Vector3(newScale, newScale, newScale);
    }
    #endregion
    
}
