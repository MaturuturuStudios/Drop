﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MenuMapLevel : MonoBehaviour {
    #region Public Attributes
    /// <summary>
    /// Game object containing the map image and the worlds
    /// </summary>
    public RectTransform map;
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
    /// First world to be selected
    /// </summary>
    public int firstSelectedWorld = 1;
    /// <summary>
    /// First level to be selected
    /// </summary>
    public int firstSelectedLevel = 1;
    /// <summary>
    /// The image of the map, to calculate the bounds
    /// </summary>
    public RectTransform sizeImageWorld;
    /// <summary>
    /// alpha for hidden worlds
    /// </summary>
    public float hiddenAlphaWorld = 0.25f;
    /// <summary>
    /// Time to reach the target point
    /// </summary>
    public float durationTravel = 1.0f;
    /// <summary>
    /// How long should  last the fading between worlds
    /// </summary>
    public float durationFading = 0.5f;
    //public float zoomIn=1.3f;
    //public float zoomOut=1f;

    public Sprite ringBaseLevel;
    public Sprite ringSelectedLevel;
    #endregion

    #region Private Attributes
    /// <summary>
    /// List of positions for all the levels
    /// </summary>
    private List<Vector2[]> levelPositions;
    /// <summary>
    /// List of all levels
    /// </summary>
    private List<GameObject[]> levels;
    /// <summary>
    /// The calculated limit of the image map
    /// </summary>
    private Vector2 limitImage;
    /// <summary>
    /// The previous world actived.
    /// </summary>
    private int actualWorldActive = 0;
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
    //Debug/development only
    private float scale=1.3f;
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

    //fade in and out the worlds
    //golden ring on last level

    #region Public Methods
    public void OnEnable() {
        //we have to select the option in update
        _selectOption = true;
    }

    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();

        levelsCanvas = new CanvasGroup[worlds.Length];
		levelPositions = new List<Vector2[]>();
        levels= new List<GameObject[]>();

        ResizeLimits();
        ConfigureWorlds();
        map.localScale = new Vector3(scale, scale, scale);
    }

    public void Update() {
        //if we have to select the option...
        if (_selectOption) {
            //only once!
            _selectOption = false;
            //select the option
            //pass the last unlocked converted in index from 0
            SelectLevel(lastUnlockedWorld - 1, lastUnlockedLevel - 1);
            ChangeRingLevel(0, 0, lastUnlockedWorld-1, lastUnlockedLevel-1);
        }

        //B, return, start
        if (Input.GetButtonDown(Axis.Irrigate) || Input.GetButtonDown(Axis.Back)
            || Input.GetButtonDown(Axis.Start)) {
            _menuNavigator.ComeBack();
        }


        //make the zoom
        Zoom();

        //move the camera to the target
        Vector2 actualPosition = map.localPosition;
        float percentageTime = (Time.time - _startTime) / durationTravel;
        float positionX = Mathf.SmoothStep(actualPosition.x, _targetPoint.x, percentageTime);
        float positionY = Mathf.SmoothStep(actualPosition.y, _targetPoint.y, percentageTime);

        Vector3 newPosition = new Vector3(positionX, positionY, 0);




        //float differenceScale = Mathf.Abs(scale - map.localScale.x);
        //if (differenceScale != 0) {
        //    //differenceScale = (differenceScale>0)? 0.3f:-0.3f;
        //_offsetZoom.x = newPosition.x / 1.3f;
        //_offsetZoom.y = newPosition.y / 1.3f;
        //_offsetZoom *= 0.3f;
        //    Debug.Log(_offsetZoom);

        //    //_offsetZoom -= sizeImageScale;
        //    _offsetZoom /= 2;
        //}

        newPosition.x -= _offsetZoom.x;
        newPosition.y -= _offsetZoom.y;
        map.localPosition = newPosition;

    }

    /// <summary>
    /// Gets the actual world selected.
    /// </summary>
    /// <returns>The actual world.</returns>
    public int GetActualWorld() {
        return actualWorldActive;
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
        CenterWorld(world, levelPositions[world][level]);
        FocusLevel(world, level);
    }

    /// <summary>
    /// With the actual world selected, center the view in the given point.
    /// </summary>
    /// <param name="center">Center.</param>
    public void CenterPoint(Vector3 center) {
        CenterWorld(GetActualWorld(), center);
    }

    /// <summary>
    /// Selects the level.
    /// The levels of the world appears and center to the given point
    /// </summary>
    /// <param name="world">World.</param>
    /// <param name="center">Center.</param>
    public void CenterWorld(int world, Vector3 center) {
        ShowLevels(world);
        MoveView(center);
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
        levels[previousWorld][previousLevel].GetComponentsInChildren<Image>()[1].sprite = ringBaseLevel;

        //put the ring to the new level
        levels[world][level].GetComponentsInChildren<Image>()[1].sprite = ringSelectedLevel;
    }

    /// <summary>
    /// Get the limits of the map with the actual scale
    /// </summary>
    private void ResizeLimits(){
        limitImage = RectTransformUtility.WorldToScreenPoint(null, sizeImageWorld.sizeDelta);
		limitImage *= map.localScale.x;
		Vector2 sizeDeltaScaled = RectTransformUtility.WorldToScreenPoint(null, map.sizeDelta);
		limitImage -= new Vector2(sizeDeltaScaled.x, sizeDeltaScaled.y);
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
		levelPositions.Add(new Vector2[allLevelWorld.transform.childCount-1]);
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

            Vector2 screenpoint = RectTransformUtility.WorldToScreenPoint(null, childTransform.localPosition);
            levelPositions[world][i] = screenpoint;

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
	/// Moves the view.
	/// </summary>
	/// <param name="center">Center.</param>
	private void MoveView(Vector3 center) {
        _startTime = Time.time;
        //put the point at bottom left of screen
        Vector2 position = center * scale;
        //move the point to the x center of screen
        float halfWidth = map.sizeDelta.x / 2.0f;
        position.x -= halfWidth;

        //move the point to the y center of screen
        float halfHeight = map.sizeDelta.y / 2.0f;
        position.y -= halfHeight;

        //none of them should be zero (less than the left/bottom side of image)
        if (position.x < 0) position.x = 0;
        if (position.y < 0) position.y = 0;

        //none of them should be more than the right/up side of the image
        ResizeLimits();
        if (position.x > limitImage.x) position.x = limitImage.x;
        if (position.y > limitImage.y) position.y = limitImage.y;

        _targetPoint = new Vector2(-position.x, -position.y);

    }

	/// <summary>
	/// Shows the levels of a world but not change the view point
	/// </summary>
	/// <param name="world">World.</param>
	private void ShowLevels(int world){
		if (actualWorldActive == world)
			return;

        if (actualWorldActive >= 0)
            StartCoroutine(FadeOut(levelsCanvas[actualWorldActive]));
			//levelsCanvas[actualWorldActive].alpha=hiddenAlpha;

		//levelsCanvas[world].alpha=1;
        StartCoroutine(FadeIn(levelsCanvas[actualWorldActive]));
        actualWorldActive = world;

        //if change of world, make a zoom effect
        Zoom(true);
    }

    public IEnumerator FadeIn(CanvasGroup canvas) {
        float start = Time.deltaTime;
        while(canvas.alpha < 1) {
            canvas.alpha = Mathf.Lerp(canvas.alpha, 1, (Time.deltaTime - start) / durationFading);
            //Debug.Log(canvas.alpha);
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("End");
    }

    public IEnumerator FadeOut(CanvasGroup canvas) {
        float start = Time.deltaTime;
        while (canvas.alpha < 1) {
            canvas.alpha = Mathf.Lerp(canvas.alpha, hiddenAlphaWorld, (Time.deltaTime - start) / durationFading);
            //Debug.Log(canvas.alpha);
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log("End");
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
    #endregion



    public float durationZoom = 0.5f;
    private float _startTimeZoom;
    private float _targetZoom;
    private bool _secondPartZoom = false;
    private Vector2 _offsetZoom;
    private bool zooming;

    private void Zoom(bool startZoom = false) {
        _offsetZoom = Vector2.zero;
        //if(!startZoom && !zooming) return;

        ////start zooming
        //if (startZoom) {
        //    _startTimeZoom = Time.time;
        //    _targetZoom = 1;
        //    zooming = true;
        //    _secondPartZoom = false;
        //}

        ////update value
        //float percentageTime = (Time.time - _startTimeZoom) / durationZoom;
        ////check if zoom in or out
        //if (percentageTime >= 1) {
        //    if (!_secondPartZoom) {
        //        _startTimeZoom = Time.time;
        //        _secondPartZoom = true;
        //        _targetZoom = scale;
        //        percentageTime = (Time.time - _startTimeZoom) / durationZoom;

        //    } else if (_secondPartZoom) {
        //        _secondPartZoom = false;
        //        zooming = false;
        //    }
        //}
        ////scale the map
        //float actualScale = map.localScale.x;
        //float newScale = Mathf.SmoothStep(actualScale, _targetZoom, percentageTime);
        //map.localScale = new Vector3(newScale, newScale, newScale);
    }

}
