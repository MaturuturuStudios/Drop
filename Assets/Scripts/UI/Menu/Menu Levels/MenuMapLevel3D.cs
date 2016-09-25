using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

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
    /// Normal distance of the camera in map level
    /// </summary>
    public float normalDistance=-3;
    /// <summary>
    /// Distance of the camera must reach (and come back to normal) when zooming
    /// </summary>
    public float zoomOutDistance=-3;
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
	/// Control if the trigger is pressed for only call change drop one time
	/// </summary>
	private bool _triggerPressed = false;
    /// <summary>
    /// Data reference
    /// </summary>
    private GameControllerData _data;
    /// <summary>
	/// A reference to the menu's navigator.
	/// </summary>
	private MenuNavigator _menuNavigator;
    /// <summary>
    /// The transform of the camera
    /// </summary>
    private Transform _transformCamera;
    /// <summary>
    /// All the cameras in the scene
    /// </summary>
    private Camera[] _cameras;
    /// <summary>
    /// The previous state of all cameras
    /// </summary>
    private bool[] _camerasPreviousState;
    /// <summary>
    /// List of all levels
    /// </summary>
    private List<GameObject[]> _levels;
    /// <summary>
    /// List of all levels
    /// </summary>
    private List<Indicator[]> _levelsindicator;
    /// <summary>
    /// Attribute to know if we have to select the level
    /// </summary>
    private bool _selectOption;
    /// <summary>
    /// The actual world and level selected
    /// </summary>
    private LevelInfo _actualLevelInfo;
    /// <summary>
    /// Time in which the travel between points started
    /// </summary>
    private float _startTime;
    /// <summary>
    /// Point camera should focus
    /// </summary>
    private Vector3 _targetPoint;
    /// <summary>
    /// Rotation to achieve
    /// </summary>
    private Quaternion _targetQuaternion;
    /// <summary>
    /// Listeners of map selection
    /// </summary>
    private List<MapLevelListener> _listeners = new List<MapLevelListener>();
	/// <summary>
	/// The title world.
	/// </summary>
    private TextMesh _titleWorld;
	/// <summary>
	/// The fading out enumerator
	/// </summary>
	private IEnumerator fadingOut = null;
	/// <summary>
	/// The previous fading out world
	/// </summary>
	private int previousFadingOut=-1;
	/// <summary>
	/// The fading in enumerator
	/// </summary>
	private IEnumerator fadingIn=null;
	/// <summary>
	/// The double previous fading in enumerator
	/// </summary>
	private IEnumerator doubleFadingIn=null;
	/// <summary>
	/// The previous fading in world
	/// </summary>
	private int previousFadingIn=-1;
	/// <summary>
	/// The previous double fading in world
	/// </summary>
	private int previousDoubleFadingIn = -1;
    #endregion

    #region Private class
    /// <summary>
    /// On select level class to react a events.
    /// </summary>
    private class OnSelectLevel : MonoBehaviour, ISelectHandler, IDeselectHandler {
        /// <summary>
        /// World and level info
        /// </summary>
        public LevelInfo level;
		/// <summary>
		/// To select.
		/// </summary>
		public bool toSelect=true;
        /// <summary>
        /// Script to call to
        /// </summary>
        public MenuMapLevel3D delegateAction;

        public void OnSelect(BaseEventData eventData) {
			if (!toSelect) return;
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
				//start animation
				GetComponentInChildren<Indicator>().SetState(StateIndicator.SELECTED);
            }
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData) {
			if (!toSelect) return;
            GetComponentInChildren<Indicator>().SetState(StateIndicator.NORMAL);
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
        _data = GameObject.FindGameObjectWithTag(Tags.GameData).GetComponent<GameControllerData>();

        _levels = new List<GameObject[]>();
        _levelsindicator = new List<Indicator[]>();

        _titleWorld = cameraCanvas.GetComponentInChildren<TextMesh>();
        MeshRenderer titleRender = cameraCanvas.GetComponentInChildren<MeshRenderer>();
        titleRender.sortingOrder = 100;

        _transformCamera = cameraCanvas.GetComponent<Transform>();

        //show the world but not focus
        _actualLevelInfo.world = -1;
        _actualLevelInfo.level = -1;
    }

    public void Start() {
        ConfigureWorlds();

        GameObject first = _levels[0][0];
        
        MeshRenderer renderer;

        //for every level
        for (int i = 0; i < _levels[0].Length; i++) {
            //get all text of the level
            TextMesh[] text = _levels[0][i].GetComponentsInChildren<TextMesh>(true);
            
            for (int j = 0; j < text.Length; j++) {
                renderer = text[j].GetComponent<MeshRenderer>();
                Color color = renderer.material.color;
                color.a = 1;
                renderer.material.color = color;
            }
        }

        _actualLevelInfo.level = 0;
        _actualLevelInfo.world = 0;

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
            LevelInfo lastLevel = _data.GetLastUnlockedLevel();
            SelectLevel(lastLevel);
            ChangeRingLevel(lastLevel, lastLevel);
            
            _targetPoint = _levels[_actualLevelInfo.world][_actualLevelInfo.level].transform.position;
            _targetQuaternion = _levels[_actualLevelInfo.world][_actualLevelInfo.level].transform.rotation;

            Vector3 targeting = _targetPoint;
            targeting.z += normalDistance;
            _transformCamera.position = targeting;

            StartCoroutine(Zoom());
        }

        //B, return, start
        if (Input.GetButtonDown(Axis.Irrigate) || Input.GetButtonDown(Axis.Back)
            || Input.GetButtonDown(Axis.Start)) {
            _menuNavigator.ComeBack();
        }

        //Change worlds with LR
        ChangeWorldInput();

        CalculatePosition();
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
        return _levels[world].Length;
    }

    /// <summary>
    /// Selects the level.
    /// The levels of the world appears and center to the concrete level
    /// </summary>
    /// <param name="world">World.</param>
    /// <param name="level">Level.</param>
    public void SelectLevel(LevelInfo info) {
        if (info.world < 0 || info.level < 0 || info.world >= worlds.Length
            || info.level >= _levels[info.world].Length) return;

        foreach (MapLevelListener listener in _listeners)
            listener.OnChangeLevel(_actualLevelInfo, info);


        //store the actual level
        _actualLevelInfo.level = info.level;
        //show the world
        ShowLevels(info.world);

        //if has a change of world, zoom in and out
        if (_actualLevelInfo.world != info.world) {
            //fade out the previous world
            FadeOut(_actualLevelInfo.world);

            if(normalDistance!=zoomOutDistance)
                StartCoroutine(Zoom());

            //change title world
            _titleWorld.text = LanguageManager.Instance.GetText("World") + " " + (info.world + 1);

            foreach (MapLevelListener listener in _listeners)
                listener.OnChangeWorld(_actualLevelInfo.world, info.world);
        }

        //store the actual world
        _actualLevelInfo.world = info.world;
        _startTime = Time.unscaledTime;
        //focus the selected level
        //has no effect if it was already selected
        FocusLevel(info);

        //get the target point
        _targetPoint = _levels[info.world][info.level].transform.position;
        _targetQuaternion = _levels[info.world][info.level].transform.rotation;
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
    /// Change the world if input is made
    /// </summary>
    private void ChangeWorldInput() {
        // Control that triggers are pressed only one time
        if (!_triggerPressed && Input.GetAxisRaw(Axis.SelectDrop) > 0) {
            FocusLevelNextWorld();
            _triggerPressed = true;

        } else if (!_triggerPressed && Input.GetAxisRaw(Axis.SelectDrop) < 0) {
            FocusLevelPreviousWorld();
            _triggerPressed = true;

        } else if (Input.GetAxisRaw(Axis.SelectDrop) == 0)
            _triggerPressed = false;
    }

    /// <summary>
    /// Look for the next world, with preference of keeping the same level
    /// </summary>
    private void FocusLevelPreviousWorld() {
        LevelInfo next = _actualLevelInfo;
        next.world -= 1;

        GameControllerData data = GameControllerData.control;
        bool found = false;
        while (!found) {
            //if go out of limits, out of method
            if (next.world < 0) return;

            //check previous levels
            while (next.level >= 0 && !found) {
                //is unlocked?
                if (data.IsUnlockedlevel(next)) {
                    //select it
                    FocusLevel(next);
                    found = true;
                }
                //previous level...
                next.level--;
            }

            //no? check next levels of world
            while (next.level < GetNumberLevels(next.world) && !found) {
                //is unlocked?
                if (data.IsUnlockedlevel(next)) {
                    //select it
                    FocusLevel(next);
                    found = true;
                }
                //next level...
                next.level++;
            }

            //nothing? try with previous world
            next.world--;
            //reset level
            next.level = _actualLevelInfo.level;
        }
    }

    /// <summary>
    /// Look for the previous world, with preference of keeping the same level
    /// </summary>
    private void FocusLevelNextWorld() {
        LevelInfo next = _actualLevelInfo;
        next.world += 1;
        
        GameControllerData data = GameControllerData.control;
        bool found = false;
        while (!found) {
            //if go out of limits, out of method
            if (next.world >= worlds.Length) return;

            //check previous levels
            while (next.level >= 0 && !found) {
                //is unlocked?
                if (data.IsUnlockedlevel(next)) {
                    //select it
                    FocusLevel(next);
                    found = true;
                }
                //previous level...
                next.level--;
            }

            //no? check next levels of world
            next.level = _actualLevelInfo.level + 1;
            while (next.level < GetNumberLevels(next.world) && !found) {
                //is unlocked?
                if (data.IsUnlockedlevel(next)) {
                    //select it
                    FocusLevel(next);
                    found = true;
                }
                //next level...
                next.level++;
            }

            //nothing? try with next world
            next.world++;
            //reset level
            next.level = _actualLevelInfo.level;
        }
    }

    /// <summary>
    /// Calculate the position of the camera and update it
    /// </summary>
    private void CalculatePosition() {
        //move the camera to the target
        Vector3 actualPosition = _transformCamera.position;

        Vector3 result = Quaternion.Euler(_targetQuaternion.eulerAngles.x, _targetQuaternion.eulerAngles.y, 0) * -Vector3.forward;
        Vector3 pointFinal = _targetPoint - result * normalDistance;

        //calculate position...
        float percentageTime = (Time.unscaledTime - _startTime) / durationTravel;
        float positionX = Mathf.SmoothStep(actualPosition.x, pointFinal.x, percentageTime);
        float positionY = Mathf.SmoothStep(actualPosition.y, pointFinal.y, percentageTime);
        float positionZ = Mathf.Lerp(actualPosition.z, pointFinal.z, percentageTime);

        Quaternion actualRotation = _transformCamera.localRotation;
        _transformCamera.localRotation = Quaternion.Slerp(actualRotation, _targetQuaternion, percentageTime);

        Vector3 newPosition = new Vector3(positionX, positionY, positionZ);
        _transformCamera.position = newPosition;
    }

    /// <summary>
    /// Select or deselect the camera of the map, modifing the rest of the cameras
    /// </summary>
    /// <param name="activate">true if want the camera map activated</param>
    private void SelectMapCamera(bool activate) {
        if (activate) {
            //get control of the camera
            //get all cameras and store its status
            _cameras = new Camera[Camera.allCamerasCount];
            Camera.GetAllCameras(_cameras);
            _camerasPreviousState = new bool[_cameras.Length];

            for (int i = 0; i < _cameras.Length; i++) {
                _camerasPreviousState[i] = _cameras[i].gameObject.activeSelf;
                //want all of them disabled except our camera
                if (cameraCanvas == _cameras[i]) continue;
                _cameras[i].gameObject.SetActive(false);
                _cameras[i].enabled = false;
            }

            //rare case in which the script is disabled before having an enable situation
        } else if(_cameras!=null){
            //restore status cameras
            for (int i = 0; i < _cameras.Length; i++) {
				if (_cameras [i] == null) continue;
                _cameras[i].gameObject.SetActive(_camerasPreviousState[i]);
                _cameras[i].enabled = true;
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
        if (EventSystem.current.currentSelectedGameObject != _levels[info.world][info.level])
            EventSystem.current.SetSelectedGameObject(_levels[info.world][info.level]);
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
            || actual.level >= _levels[actual.world].Length || previous.level >= _levels[previous.world].Length) return;

        //quit the ring to previous level
		_levels[previous.world][previous.level].GetComponentInChildren<Indicator>().setLastUnlockedLevel(false);

        //put the ring to the new level
		_levels[actual.world][actual.level].GetComponentInChildren<Indicator>().setLastUnlockedLevel(true);
    }

    /// <summary>
	/// Shows the levels of a world but not change the view point
	/// </summary>
	/// <param name="world">World.</param>
	private void ShowLevels(int world) {
        //if already actived, return
        if (_actualLevelInfo.world == world) return;

        //fade in the selected world
        FadeIn(world);
    }

    /// <summary>
    /// Hidde or not the text
    /// </summary>
    /// <param name="world">the world to hidde</param>
    /// <param name="hidden">true if hidde the text false otherwise</param>
    private IEnumerator HiddeText(int world, bool hidden) {
        if (world >= 0) {
            float alpha = (hidden) ? 0 : 1;
            bool done = false;
            do {
                Color color = Color.white;
                GameObject first = _levels[world][0];
                TextMesh[] text = first.GetComponentsInChildren<TextMesh>(true);
                float alphaFinal = Mathf.MoveTowards(color.a, alpha, Time.unscaledDeltaTime * speedFading);

                if (Mathf.Abs(alphaFinal - alpha) < 0.01) done = true;

                //for every level...
                foreach (GameObject aLevel in _levels[world]) {
                    text = aLevel.GetComponentsInChildren<TextMesh>(true);
                    for (int j = 0; j < text.Length; j++) {
                        MeshRenderer renderer = text[j].GetComponent<MeshRenderer>();
                        color = renderer.material.color;
                        color.a = alphaFinal;
                        renderer.material.color = color;
                    }
                }
                yield return null;
            } while (!done);
        }
    }
		
    /// <summary>
    /// Fade int a canvas group
    /// </summary>
    /// <param name="canvas"></param>
    private void FadeIn(int fadingInWorld) {
		previousDoubleFadingIn = previousFadingIn;
		previousFadingIn = fadingInWorld;
		//stop if some is fading in!
		if(previousFadingOut==fadingInWorld){
			StopCoroutine(fadingOut); 
			previousFadingOut = -1;
		}
		doubleFadingIn = fadingIn;
		fadingIn = HiddeText (fadingInWorld, false);
		StartCoroutine(fadingIn);
    }


    /// <summary>
    /// Fade out a canvas group
    /// </summary>
    /// <param name="canvas"></param>
    private void FadeOut(int fadingOutWorld) {
		previousFadingOut = fadingOutWorld;
		if (previousFadingIn == fadingOutWorld) {
			StopCoroutine (fadingIn);
			previousFadingIn = -1;
		}
		if (previousDoubleFadingIn == fadingOutWorld) {
			StopCoroutine(doubleFadingIn);
			previousDoubleFadingIn = -1;
		}
		//hide all text
		fadingOut = HiddeText(fadingOutWorld, true);
		StartCoroutine(fadingOut);
   }

    /// <summary>
    /// Configure the worlds
    /// </summary>
    private void ConfigureWorlds() {
        Button previousLevel = null;

        for (int h = 0; h < worlds.Length; h++) {

            int world = h;
            GameObject allLevelWorld = worlds[world];

            int i = 0;
            _levels.Add(new GameObject[allLevelWorld.transform.childCount]);
            _levelsindicator.Add(new Indicator[allLevelWorld.transform.childCount]);

            LevelInfo theLevel;
            theLevel.world = world;

            //for each level...
            foreach (Transform childTransform in allLevelWorld.transform) {
                theLevel.level = i;

                //get the object of the level
                GameObject child = childTransform.gameObject;
                //store it
                _levels[world][i] = child;

                //get its script
                _levelsindicator[world][i] = child.GetComponentInChildren<Indicator>();

                //add a script to watch selection over the level
                child.AddComponent(typeof(OnSelectLevel));
                OnSelectLevel script = child.GetComponent<OnSelectLevel>();

                script.level.world = world; //which world belongs
                script.level.level = i; //number of level

                //get the text and hidde it
                TextMesh[] text = child.GetComponentsInChildren<TextMesh>(true);
                for (int j = 0; j < text.Length; j++) {
                    MeshRenderer renderer = text[j].gameObject.GetComponent<MeshRenderer>();
                    Color color = renderer.material.color;
                    color.a = 0;
                    renderer.material.color = color;
                }

                Button button = child.GetComponent<Button>();
                previousLevel = SetButton(previousLevel, theLevel, button, script);

                SetDataLevelScript(child, theLevel);

                i++;
            }
        }

        //last level has no more levels
        if (previousLevel != null) {
            Navigation navigation = previousLevel.navigation;
            navigation.selectOnDown = null;
            navigation.selectOnRight = null;
            previousLevel.navigation = navigation;
        }

        //put the special ring
        LevelInfo lastUnlocked = _data.GetLastUnlockedLevel();
        ChangeRingLevel(lastUnlocked, lastUnlocked);
    }

    /// <summary>
    /// Set data to the script of the level
    /// </summary>
    /// <param name="child"></param>
    /// <param name="theLevel"></param>
    private void SetDataLevelScript(GameObject child, LevelInfo theLevel) {
        //get the script to give the scene asociated to this level
        //and few information
        SceneLevel sceneLevel = child.GetComponent<SceneLevel>();
        sceneLevel.waitBeforeStartLevel = waitBeforeStartLevel;
        sceneLevel.level = _data.GetScene(theLevel);
        sceneLevel.infoLevel = theLevel;
        sceneLevel.menuMap = this;

        //Get the script to give the level asociated to this score
        ScoreLevel levelScore = _data.GetScoreLevel(theLevel);
        Score score = child.GetComponentInChildren<Score>();
        score.SetScore(levelScore.max, levelScore.achieved);
    }

    /// <summary>
    /// Set some data button
    /// </summary>
    /// <param name="previousLevel"></param>
    /// <param name="theLevel"></param>
    /// <param name="actualLevel"></param>
    /// <param name="script"></param>
    /// <returns></returns>
    private Button SetButton(Button previousLevel, LevelInfo theLevel, Button actualLevel, OnSelectLevel script) {
        int world = theLevel.world;
        int i = theLevel.level;

        Button button = actualLevel;
        bool availableForNavigation = true;
        //check if the level is locked (or non-available)
        if (!_data.IsUnlockedlevel(theLevel)) {
            _levelsindicator[world][i].SetState(StateIndicator.NON_UNLOCKED);
            button.interactable = false;
            script.toSelect = false;
            availableForNavigation = false;
        }

        //check if the level is non-available (put it grey)
        if (!_data.IsAvailableLevel(theLevel)) {
            _levelsindicator[world][i].SetState(StateIndicator.NON_AVAILABLE);
            button.interactable = false;
            availableForNavigation = false;
        }


        //link with previous level
        if (previousLevel != null && availableForNavigation) {
            Navigation nav = previousLevel.navigation;
            nav.selectOnDown = button;
            nav.selectOnRight = button;
            previousLevel.navigation = nav;

            nav = button.navigation;
            nav.selectOnUp = previousLevel;
            nav.selectOnLeft = previousLevel;
            button.navigation = nav;
        }

        if(availableForNavigation) previousLevel = button;

        script.delegateAction = this; //script to delegate

        return previousLevel;
    }
    
    /// <summary>
    /// Make a zoom
    /// </summary>
    /// <returns></returns>
    private IEnumerator Zoom() {
        float startTime = Time.unscaledTime;

        float percentageTime = (Time.unscaledTime - startTime) / timeZooming;
        do {
            Vector3 position = _transformCamera.localPosition;
            position.z -= normalDistance;
            position.z += Mathf.Lerp(normalDistance, zoomOutDistance, easeFunctionZoom.Evaluate(percentageTime));
            _transformCamera.localPosition = position;
            yield return null;
            percentageTime = (Time.unscaledTime - startTime) / timeZooming;
        } while (percentageTime < 1);
    }
    #endregion
}
