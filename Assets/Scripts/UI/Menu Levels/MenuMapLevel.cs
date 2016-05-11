﻿using UnityEngine;
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
        for(int i=0; i<worlds.Length; i++) {
            GameObject aWorld = worlds[i];
            worldsTransform[i] = aWorld.GetComponent<RectTransform>();
            //get the button of the world (title)
            foreach (Transform childTransform in aWorld.transform) {
                GameObject child = childTransform.gameObject;
				if (child.tag==Tags.Level) {
					levels [i] = child;
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
        RectTransform worldData = worldsTransform[world];

        float displacement = 0;
        if (world > 0) {
            //get half width canvas
            float halfWidthScreen = Screen.width / 2.0f;
            //get point world to center
            float worldPosition = worldData.localPosition.x;
            //get width world/2
            float halfWorldWidth = worldData.rect.width / 2.0f;
            //calculate position...
            float worldCenter = worldPosition;
            
            
            displacement = worldCenter + halfWidthScreen;
            if (displacement < 0) {
                displacement = 0;
            }
        }

        Vector3 finalPosition = map.localPosition;
        Vector2 delta = map.sizeDelta;
        delta.y = 0;
        delta.x = -displacement;
        finalPosition.x = -displacement;
        map.localPosition = finalPosition;

		if (previousWorld >= 0)
			levels [previousWorld].SetActive (false);
		levels[world].SetActive (true);
		previousWorld = world;


		//map.localScale = Vector3.one * 1.5f;
		//Zoom (true, finalPosition);
    }

	private void ShowLevels(int world){
		
	}

	public void Zoom(bool zoomIn, Vector3 point) {
				
    }
}
