using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class MenuMapLevel : MonoBehaviour {
    public RectTransform map;
    public GameObject[] worlds;

    /// <summary>
	/// The first option to be selected
	/// </summary>
	public GameObject firstSelected;

	private int previousWorld;
	private GameObject[] levels;
    /// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;

    private class OnSelectWorld : MonoBehaviour, ISelectHandler {
        public int world;
        public MenuMapLevel delegateAction;
        public void OnSelect(BaseEventData eventData) {
            delegateAction.Displacement(world);
        }
    }

    private class OnSelectLevel: MonoBehaviour, ISelectHandler {
        public int world;
        public int level;
        public Transform levelPosition;
        public MenuMapLevel delegateAction;
        public void OnSelect(BaseEventData eventData) {
            delegateAction.Displacement(world, levelPosition.position);
        }
    }

    private RectTransform[] worldsTransform;
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
        worldsTransform = new RectTransform[worlds.Length];
		levels = new GameObject[worlds.Length];

        ConfigureWorlds();  
    }

    private void ConfigureWorlds() {
        for (int i = 0; i < worlds.Length; i++) {
            GameObject aWorld = worlds[i];
            worldsTransform[i] = aWorld.GetComponent<RectTransform>();
            //get the button of the world (title)
            foreach (Transform childTransform in aWorld.transform) {
                GameObject child = childTransform.gameObject;
                if (child.tag == Tags.Level) {
                    levels[i] = child;
                    ConfigureLevels(i);
                    continue;
                }

                Text title = child.GetComponent<Text>();
                //when found...
                if (title != null) {
                    //add a script to watch
                    child.AddComponent(typeof(OnSelectWorld));
                    OnSelectWorld script = child.GetComponent<OnSelectWorld>();
                    script.world = i;
                    script.delegateAction = this;
                }
            }
        }
    }

    private void ConfigureLevels(int world) {
        GameObject allLevelWorld = levels[world];
        int i = 0;
        foreach (Transform childTransform in allLevelWorld.transform) {
            GameObject child = childTransform.gameObject;

            //add a script to watch
            child.AddComponent(typeof(OnSelectLevel));
            OnSelectLevel script = child.GetComponent<OnSelectLevel>();
            script.world = world;
            script.level = i;
            script.levelPosition = childTransform;
            script.delegateAction = this;
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
    /// Center the world
    /// </summary>
    public void Displacement(int world) {
        Displacement(world, levels[world].transform.GetChild(0).transform.position);
    }

    public void Displacement(int world, Vector3 center) {
        RectTransform worldData = worldsTransform[world];

        center -= map.localPosition;
        Debug.Log(center);

        float screenWidthHalf = Screen.width / 2;
        
        Vector2 displacement = Vector2.zero;
        float offset = center.x - Screen.width;
        if (offset < 0) {
            displacement.x = center.x + (center.x - screenWidthHalf);
        } else {
            displacement.x = center.x - offset - (Screen.width / 2);
        }


        offset = center.y - Screen.height;
        if (offset < 0) offset = 0;
        displacement.y = center.y - offset - (Screen.height / 2);
        
        if (displacement.x < 0) displacement.x = 0;
        if (displacement.y < 0) displacement.y = 0;


        Vector3 finalPosition = map.localPosition;
        //Vector2 delta = map.sizeDelta;
        //delta.y = 0;
        //delta.x = -displacement.x;
        finalPosition.x = -displacement.x;
        map.localPosition += finalPosition;

        //if (previousWorld >= 0)
        //    levels[previousWorld].SetActive(false);
        //levels[world].SetActive(true);
        //previousWorld = world;

        //placed! now I need to zoom it until the width of the image is the width of the screen
        //float scale = Screen.width / worldData.rect.width;
        //scale += 1;
        //Debug.Log(scale);
        //map.localScale = Vector3.one * scale;
    }

	private void ShowLevels(int world){
		
	}

	public void Zoom(bool zoomIn, Vector3 point) {
				
    }
}
