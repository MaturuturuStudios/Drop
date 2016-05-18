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

    public void OnEnable() {
        //we have to select the option in update
        _selectOption = true;
    }

    public void Awake() {
		levelsCanvas = new CanvasGroup[worlds.Length];
		levelPositions = new List<Vector2[]>();
        ConfigureWorlds();  
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
			levelPositions[world][i]=childTransform.position;
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
        float screenWidthHalf = Screen.width / 2;
        
        Vector2 displacement = Vector2.zero;
		displacement.x = center.x;
		/*
        float offset = center.x - Screen.width;
		//if negative, center is inside the view
        if (offset < 0) offset = 0;
        displacement.x = center.x - offset - (Screen.width / 2);
        


        offset = center.y - Screen.height;
        if (offset < 0) offset = 0;
        displacement.y = center.y - offset - (Screen.height / 2);
        
        if (displacement.x < 0) displacement.x = 0;
        if (displacement.y < 0) displacement.y = 0;
		*/

        Vector3 finalPosition = map.localPosition;
        //Vector2 delta = map.sizeDelta;
        //delta.y = 0;
        //delta.x = -displacement.x;
        finalPosition.x = -displacement.x;
        map.localPosition = finalPosition;

        //placed! now I need to zoom it until the width of the image is the width of the screen
        //float scale = Screen.width / worldData.rect.width;
        //scale += 1;
        //Debug.Log(scale);
        //map.localScale = Vector3.one * scale;
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

	public void Zoom(bool zoomIn, Vector3 point) {
				
    }
}
