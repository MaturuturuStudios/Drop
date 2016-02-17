﻿using UnityEngine;
using System.Collections.Generic;

public class GameControllerIndependentControl : MonoBehaviour {

    //Control atributes
    public GameObject currentCharacter;
    public List<GameObject> allCurrentCharacters;

    //Drops pool
    public int numOfDrops = 10;
    private List<GameObject> _DropsPool;

    //Drop prefab
    public GameObject PfDrop;

    void Start(){

        //fill the drops pool
        _DropsPool = new List<GameObject>();
        for (int i = 0; i < numOfDrops; ++i)
        {
            GameObject drop = (GameObject)Instantiate(PfDrop);
            drop.GetComponent<Renderer>().enabled = false;
            _DropsPool.Add(drop);
        }
    }

	void Update () {/*Nothing at the moment*/}


    public void SetControl(int id)
    {
        if (id < allCurrentCharacters.Count && id >= 0)
        {
            //Try to stop abandoned drop
            StopDrop();

            //Set Control to new drop
            currentCharacter = allCurrentCharacters[id];
        }
    }

    /// TODO: make method
    /// <summary>
    /// Set the control to the given drop if is on the list of drops under player's control
    /// </summary>
    /// <param name="drop">drop to control</param>
    public void SetControl(GameObject drop) {
        
    }

    //TODO
    public bool IsUnderControl()
    {
        return true;
    }

    public GameObject AddDrop(bool setControl = false)
    {
        GameObject drop = null;
        if (_DropsPool.Count > allCurrentCharacters.Count)
        {
            //Get a drop from the pool
            Vector3 position = currentCharacter.transform.position;
            _DropsPool[allCurrentCharacters.Count].transform.position = new Vector3(position.x, position.y, position.z);  //At the moment we put it on the same place as controled
            _DropsPool[allCurrentCharacters.Count].GetComponent<Renderer>().enabled = true;

            //Set Control
            if (setControl)
                SetControl(allCurrentCharacters.Count - 1);

            //Add to control list
            allCurrentCharacters.Add(_DropsPool[allCurrentCharacters.Count]);
        }
        return drop;
    }

    public void RemoveDropFromControl(GameObject drop)
    {
        if (allCurrentCharacters.Count > 1)
        {
            //remove from control list
            allCurrentCharacters.Remove(drop);

            //Set Control
            StopDrop();
            currentCharacter = allCurrentCharacters[0];
        }
    }

    public void KillDrop(GameObject drop)
    {
        if (allCurrentCharacters.Count > 1)
        {
            //remove from control list
            allCurrentCharacters.Remove(drop);

            //Set the drop out
            drop.transform.position = new Vector3(0.0f, 0.0f, -20.0f);  
            drop.GetComponent<Renderer>().enabled = false;

            //Set Control
            StopDrop();
            //currentCharacter = allCurrentCharacters[0];
        }
    }

    public void ControlNextDrop()
    {
        if (allCurrentCharacters.Count > 1)
        {
            //Get next index
            int next_drop = allCurrentCharacters.IndexOf(currentCharacter) + 1;

            //Loop control
            if (next_drop == allCurrentCharacters.Count)
                next_drop = 0;

            //Set Control
            StopDrop();
            currentCharacter = allCurrentCharacters[next_drop];
        }
    }

    public void ControlBackDrop()
    {
        if (allCurrentCharacters.Count > 1)
        {
            //Get prev index
            int back_drop = allCurrentCharacters.IndexOf(currentCharacter) - 1;

            //Loop control
            if (back_drop == -1)
                back_drop = allCurrentCharacters.Count - 1;

            //Set Control
            StopDrop();
            currentCharacter = allCurrentCharacters[back_drop];
        }
    }

    public void StopDrop()
    {
        CharacterControllerCustomPlayer cccp = currentCharacter.GetComponent<CharacterControllerCustomPlayer>();

        //Reset State
        cccp.HorizontalInput = 0;
        cccp.VerticalInput = 0;
        cccp.JumpInput = 0;


    }

}