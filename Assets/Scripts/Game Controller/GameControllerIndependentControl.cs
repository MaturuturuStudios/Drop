using UnityEngine;
using System.Collections.Generic;

public class GameControllerIndependentControl : MonoBehaviour {

    public GameObject currentCharacter;
    public List<GameObject> allCharacters;


    void Start(){/*Nothing at the moment*/}

	void Update () {/*Nothing at the moment*/}


    public void SetControl(int id)
    {
        if (id < allCharacters.Count && id >= 0)
        {
            //Try to  stop it
            StopDrop();

            //Set Control
            currentCharacter = allCharacters[id];

        }
    }

    public void AddDrop(GameObject drop)
    {
        //Add to control list
        allCharacters.Add(drop);

        StopDrop();

        //Set Control
        currentCharacter = drop;
    }

    public void RemoveDrop(GameObject drop)
    {
        if (allCharacters.Count > 1)
        {
            //remove from control list
            allCharacters.Remove(drop);

            //I don't know if i have to kill the drop
            Destroy(drop);

            //Set Control
            StopDrop();
            currentCharacter = allCharacters[0];
        }
    }

    public void ControlNextDrop()
    {
        if (allCharacters.Count > 1)
        {
            //Get next index
            int next_drop = allCharacters.IndexOf(currentCharacter) + 1;

            //Loop control
            if (next_drop == allCharacters.Count)
                next_drop = 0;

            //Set Control
            StopDrop();
            currentCharacter = allCharacters[next_drop];
        }
    }

    public void ControlBackDrop()
    {
        if (allCharacters.Count > 1)
        {
            //Get prev index
            int back_drop = allCharacters.IndexOf(currentCharacter) - 1;

            //Loop control
            if (back_drop == -1)
                back_drop = allCharacters.Count - 1;

            //Set Control
            StopDrop();
            currentCharacter = allCharacters[back_drop];
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