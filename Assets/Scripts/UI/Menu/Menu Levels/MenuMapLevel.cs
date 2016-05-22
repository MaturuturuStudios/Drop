using UnityEngine;
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
    /// The image of the map, to calculate the bounds
    /// </summary>
    public RectTransform sizeImageWorld;
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
    /// The border of a normal level
    /// </summary>
    public Sprite ringBaseLevel;
    /// <summary>
    /// The border of the las level
    /// </summary>
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

        FadeIn();
        FadeOut();


        //make the zoom
        Zoom();

        //move the camera to the target
        Vector2 actualPosition = map.localPosition;
        float percentageTime = (Time.time - _startTime) / durationTravel;
        float positionX = Mathf.SmoothStep(actualPosition.x, _targetPoint.x, percentageTime);
        float positionY = Mathf.SmoothStep(actualPosition.y, _targetPoint.y, percentageTime);

        Vector3 newPosition = new Vector3(positionX, positionY, 0);

        //en principio esto funciona
        //la posición está calculada en función de la escala del mapa (scale)
        //con una regla de tres se saca la posición que tendría con la escala
        //actual del mapa y restando ambas, se obtiene el offset necesario
        //para que el punto permanezca centrado (siguiendo la trayectoria lineal
        //hecha por el SmoothStep anterior)
        Vector2 offset = Vector2.zero;
        float z = (newPosition.x * map.localScale.x) / scale;
        offset.x = newPosition.x - z;

        z = (newPosition.y * map.localScale.y) / scale;
        offset.y = newPosition.y - z;

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
            FadeOut(levelsCanvas[actualWorldActive]);
        
        FadeIn(levelsCanvas[world]);
        actualWorldActive = world;

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
        canvasFadeIn.alpha = Mathf.Lerp(canvasFadeIn.alpha, 1, Time.deltaTime*speedFading);

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
        canvasFadeOut.alpha = Mathf.Lerp(canvasFadeOut.alpha, hiddenAlphaWorld, Time.deltaTime*speedFading);

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
    #endregion


    //Debug/development only
    private float scale = 1.3f;
    public float durationZoom = 1f;
    private float _startTimeZoom;
    private float _targetZoom=1;
    private Vector2 _offsetZoom;
    private bool zooming=false;

    /// <summary>
    /// llamado desde show level con startZoom true
    /// y continuado en update (startzoom false)
    /// </summary>
    /// <param name="startZoom"></param>
    private void Zoom(bool startZoom = false) {
        //_offsetZoom = Vector2.zero;
        //if (!startZoom && !zooming) return;

        ////start zooming
        //if (startZoom) {
        //    _startTimeZoom = Time.time;
        //    _targetZoom = 1;
        //    zooming = true;
        //}

        ////scale the map
        //float actualScale = map.localScale.x;
        //float newScale = 0;
        ////update value
        //float percentageTime = (Time.time - _startTimeZoom) / durationZoom;
        ////check if zoom in or out
        //if (percentageTime <0.5) {
        //    newScale = Mathf.SmoothStep(actualScale, _targetZoom, percentageTime*2);
        //} else if (percentageTime>1f) {
        //    zooming = false;
        //} else {
        //    newScale = Mathf.SmoothStep(actualScale, scale, percentageTime/2);
        //}


        //map.localScale = new Vector3(newScale, newScale, newScale);
    }

}
