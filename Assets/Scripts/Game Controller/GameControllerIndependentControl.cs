using UnityEngine;
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
        _DropsPool = new List<GameObject>(allCurrentCharacters);
        for (int i = allCurrentCharacters.Count - 1; i < numOfDrops; ++i)
        {
            GameObject drop = (GameObject)Instantiate(PfDrop);
            drop.GetComponent<Renderer>().enabled = false;
            drop.transform.position = new Vector3(0.0f + i, -200.0f, 0.0f);
            drop.SetActive(false);
            _DropsPool.Add(drop);
        }
    }

	void Update () {/*Nothing at the moment*/}

    public GameObject AddDrop(bool setControl = false)
    {
        GameObject drop = null;
        if (_DropsPool.Count > allCurrentCharacters.Count)
        {
            //Get a drop from the pool
            drop = FindNextAvalaibleDrop();

            //Move the drop & enable it
            Vector3 position = currentCharacter.transform.position;
            //position.y += 1 * drop.GetComponent<CharacterSize>().GetSize();
            drop.transform.position = new Vector3(position.x +2 , position.y, position.z);  //At the moment we put it on the same place as controled
            drop.GetComponent<Renderer>().enabled = true;
            drop.SetActive(false);

            //Set Control
            if (setControl)
                SetControl(allCurrentCharacters.Count - 1);

            //Add to control list
            allCurrentCharacters.Add(drop);
        }
        return drop;
    }

    public void KillDrop(GameObject drop)
    {
        if (allCurrentCharacters.Count > 1)
        {
            //remove from control list
            allCurrentCharacters.Remove(drop);

            //Set the drop out
            drop.GetComponent<Renderer>().enabled = false;
            drop.transform.position = new Vector3(0.0f, -200.0f, 0.0f);
            drop.SetActive(false);

            StopDrop();
        }
    }

    public void RemoveDropFromControlList(GameObject drop)
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


    public void SetControl(int id)
    {
        if (id < allCurrentCharacters.Count && id > -1)
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
    public void SetControl(GameObject drop)
    {
        int index = _DropsPool.IndexOf(drop);

        if (index > -1)
        {
            SetControl(index);
        }
    }

    //TODO
    public bool IsUnderControl(GameObject drop)
    {
        return _DropsPool.Contains(drop);
    }

    private void StopDrop()
    {
        //Reset State
        currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();
    }

    //TODO
    private GameObject FindNextAvalaibleDrop()
    {
        return _DropsPool[allCurrentCharacters.Count];
    }

}