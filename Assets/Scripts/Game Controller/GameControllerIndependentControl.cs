using UnityEngine;
using System.Collections;
using System;

public class GameControllerIndependentControl : MonoBehaviour {

    public GameObject currentCharacter;
    public GameObject[] allCharacters;


    void Start(){/*Nothing at the moment*/}

	void Update () {/*Nothing at the moment*/}


    public void SetControl(int id)
    {
        if (id < allCharacters.Length && id >= 0)
            currentCharacter = allCharacters[id];
    }

    public void ControlNextDrop()
    {
        int next_drop = Array.IndexOf(allCharacters, currentCharacter) + 1;
        if (next_drop == allCharacters.Length)
            next_drop = 0;
        currentCharacter = allCharacters[next_drop];
    }

    public void ControlBackDrop()
    {
        int back_drop = Array.IndexOf(allCharacters, currentCharacter) - 1;
        if (back_drop == -1)
            back_drop = allCharacters.Length - 1;
        currentCharacter = allCharacters[back_drop];
    }

}