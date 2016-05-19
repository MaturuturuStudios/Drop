using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MenuMapLevel : MonoBehaviour {
    public RectTransform map;
	public Text title;
	public GameObject[] worlds;
	private List<Vector2[]> levelPositions;

    /// <summary>
	/// The first option to be selected
	/// </summary>
	public GameObject firstSelected;
    /// <summary>
    /// The image of the map, to calculate the bounds
    /// </summary>
    public RectTransform sizeImage;
    /// <summary>
    /// The calculated limit of the image map
    /// </summary>
    private Vector2 limitImage;
    /// <summary>
    /// The previous world actived.
    /// </summary>
    private int actualWorldActive;
	/// <summary>
	/// The levels' panels (canvas).
	/// </summary>
	private CanvasGroup[] levelsCanvas;
    /// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;

    public float zoomIn=1.8f;
    public float zoomOut=1f;

    private Vector2 _targetPoint;
    public float durationTravel=1.0f;
    private float _startTime;

    private float scale;

	/// <summary>
	/// On select level class to react a events.
	/// </summary>
    private class OnSelectLevel: MonoBehaviour, ISelectHandler {
        public int world;
        public int level;
        public MenuMapLevel delegateAction;
        public void OnSelect(BaseEventData eventData) {
			delegateAction.SelectLevel(world, level);
        }
    }

    //world and level unblocked
    //with return/b... if level, come back to world, if world, come back menu

    //while world, scroll left right
    //when hit, show under world and zoom in

    //detect navigation between worlds and levels
    //fade in and out the worlds

    public void OnEnable() {
        //we have to select the option in update
        _selectOption = true;
    }

    public void Awake() {
		levelsCanvas = new CanvasGroup[worlds.Length];
		levelPositions = new List<Vector2[]>();

        

        limitImage = sizeImage.sizeDelta;
        limitImage -= new Vector2(map.sizeDelta.x, map.sizeDelta.y);
        ConfigureWorlds();

        //scale = 1.8f;
        //map.localScale = new Vector3(scale, scale, scale);

        SelectLevel(1, 0);  
    }

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

        int i = 0;
		levelPositions.Add(new Vector2[allLevelWorld.transform.childCount]);

		//for each world...
		foreach (Transform childTransform in allLevelWorld.transform) {
            GameObject child = childTransform.gameObject;

            //add a script to watch selection over the level
            child.AddComponent(typeof(OnSelectLevel));
            OnSelectLevel script = child.GetComponent<OnSelectLevel>();
            script.world = world; //which world belongs
            script.level = i; //number of level
            Vector2 screenpoint = RectTransformUtility.WorldToScreenPoint(null, childTransform.localPosition);
            levelPositions[world][i]= screenpoint;
            script.delegateAction = this; //script to delegate
            i++;
        }
	}

    public void Update() {
        //if we have to select the option...
        if (_selectOption) {
            //only once!
            _selectOption = false;
            //select the option
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
        
        Vector2 actualPosition = map.localPosition;
        float t = (Time.time - _startTime) / durationTravel;
        float positionX = Mathf.SmoothStep(actualPosition.x, _targetPoint.x, t);
        float positionY = Mathf.SmoothStep(actualPosition.y, _targetPoint.y, t);
        Zoom();
        map.localPosition = new Vector3(positionX, positionY, 0);
        
        
    }

	/// <summary>
	/// Gets the actual world selected.
	/// </summary>
	/// <returns>The actual world.</returns>
	public int GetActualWorld(){
		return actualWorldActive;
	}

	/// <summary>
	/// Selects the level in the actual world
	/// </summary>
	/// <param name="level">Level.</param>
	public void SelectLevel(int level){
		SelectLevel(GetActualWorld (), level);
	}

	/// <summary>
	/// Selects the level.
	/// The levels of the world appears and center to the concrete level
	/// </summary>
	/// <param name="world">World.</param>
	/// <param name="level">Level.</param>
	public void SelectLevel(int world, int level=0){
		CenterWorld(world, levelPositions[world][level]);
	}

	/// <summary>
	/// With the actual world selected, center the view in the given point.
	/// </summary>
	/// <param name="center">Center.</param>
	public void CenterPoint(Vector3 center){
		CenterWorld(GetActualWorld(), center);
	}

	/// <summary>
	/// Selects the level.
	/// The levels of the world appears and center to the given point
	/// </summary>
	/// <param name="world">World.</param>
	/// <param name="center">Center.</param>
	public void CenterWorld(int world, Vector3 center){
		ShowLevels(world);
		MoveView(center);
	}

	/// <summary>
	/// Moves the view.
	/// </summary>
	/// <param name="center">Center.</param>
	private void MoveView(Vector3 center) {
        _startTime = Time.time;

        //put the point at bottom left of screen
        Vector2 position = center;
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
        if (position.x > limitImage.x) position.x = limitImage.x;
        if (position.y > limitImage.y) position.y = limitImage.y;

        //position.x *= scale;

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
			levelsCanvas[actualWorldActive].alpha=0;

		levelsCanvas[world].alpha=1;
		actualWorldActive = world;
		title.text = "World " + (world+1);
	}

	public void Zoom() {
        //float t = (Time.time - _startTime) / durationTravel;
        //float zoom;
        //if (t <= 0.5f) {
        //    zoom=Mathf.Lerp(zoomIn, zoomOut, t);
        //} else {
        //    zoom=Mathf.Lerp(zoomOut, zoomIn, t);
        //}
        //map.localScale = new Vector3(zoom,zoom, zoom);
    }
}
