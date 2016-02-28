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

    //Camera 
    public CameraController _cameraController;

    void Awake()
    {
        //Get the camera
        _cameraController = GameObject.FindGameObjectWithTag("MainCamera")
                                .GetComponent<CameraController>();
        //Actualize camera objective
        _cameraController.SetObjective(currentCharacter);
    }

    /// <summary>
    /// Called on start script
    /// </summary>
    void Start(){

        //fill the drops pool
        _DropsPool = new List<GameObject>(allCurrentCharacters);
        for (int i = allCurrentCharacters.Count - 1; i < numOfDrops; ++i)
        {
            GameObject drop = (GameObject)Instantiate(PfDrop);
            drop.gameObject.name = "Drop (" + allCurrentCharacters.Count  + ")";
            drop.GetComponent<Renderer>().enabled = false;
            drop.transform.position = new Vector3(0.0f + i, -200.0f, 0.0f);
            drop.SetActive(false);
            _DropsPool.Add(drop);
        }
    }

	void Update () {/*Nothing at the moment*/}

    /// <summary>
    /// Add a drop to the scene, and under player control.nder player's control from list and set it out of the scene
    /// </summary>
    /// <param name="setControl">Set the control to the drop</param>
    public GameObject AddDrop(bool setControl = false, bool addToControlList = true)
    {
        GameObject drop = null;
        if (_DropsPool.Count > allCurrentCharacters.Count)
        {
            //Get a drop from the pool
            drop = FindNextAvalaibleDrop();

            //Move the drop & enable it
            Vector3 position = currentCharacter.transform.position;
            drop.transform.position = new Vector3(position.x , position.y, position.z);  //At the moment we put it on the same place as controled
            drop.GetComponent<Renderer>().enabled = true;
            drop.SetActive(false);

            //Set Control
            if (setControl)
                SetControl(allCurrentCharacters.Count - 1);

            if (addToControlList)
                allCurrentCharacters.Add(drop);

        }
        else
        {
            //add more drops
            for (int i = allCurrentCharacters.Count - 1; i < numOfDrops; ++i)
            {
                GameObject clone = (GameObject)Instantiate(PfDrop);
                clone.gameObject.name = "Drop (" + allCurrentCharacters.Count + ")";
                clone.GetComponent<Renderer>().enabled = false;
                clone.transform.position = new Vector3(0.0f + i, -200.0f, 0.0f);
                clone.SetActive(false);
                _DropsPool.Add(clone);
            }

            drop = AddDrop(setControl, addToControlList);
        }
        return drop;
    }

    /// <summary>
    /// Remove a drop under player's control from list and set it out of the scene
    /// </summary>
    /// <param name="drop">drop to remove from control & scene</param>
    public void KillDrop(GameObject drop)
    {
        //remove from control list
        allCurrentCharacters.Remove(drop);

        //Set the drop out the scene
        drop.GetComponent<Renderer>().enabled = false;
        drop.transform.position = new Vector3(0.0f, -200.0f, 0.0f);
        //Disactive drop
        drop.SetActive(false);

        currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();
    }

    /// <summary>
    /// Remove a drop under player's control from list
    /// </summary>
    /// <param name="drop">drop to remove from control</param>
    public void RemoveDropFromControlList(GameObject drop)
    {
        if (allCurrentCharacters.Count > 1)
        {
            //remove from control list
            allCurrentCharacters.Remove(drop);

            //Set Control
            currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();
            SetControl(0);
        }
    }

    /// <summary>
    /// Sets the control to the next drop in list
    /// </summary>
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
            currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();
            SetControl(next_drop);

            //Actualize camera objective
            _cameraController.SetObjective(currentCharacter);
        }
    }

    /// <summary>
    /// Sets the control to the previous drop in list
    /// </summary>
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
            currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();
            SetControl(back_drop);

            //Actualize camera objective
            _cameraController.SetObjective(currentCharacter);
        }
    }

    /// <summary>
    /// Sets the control to an specific drop
    /// </summary>
    /// <param name="index">Index of drop in the allCurentCharacters list</param>
    public void SetControl(int index)
    {
        if (index < allCurrentCharacters.Count && index > -1)
        {
            //Try to stop abandoned drop
            currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();

            //Set Control to new drop
            currentCharacter = allCurrentCharacters[index];

            //Check if it is under control, if not add it
            if (!IsUnderControl(currentCharacter))
                allCurrentCharacters.Add(currentCharacter);

            //Actualize camera objective
            _cameraController.SetObjective(currentCharacter);
        }
    }

    /// <summary>
    /// Set the control to the given drop if is on the list of drops under player's control
    /// </summary>
    /// <param name="drop">drop to control</param>
    public void SetControl(GameObject drop)
    {
        int index = _DropsPool.IndexOf(drop);

        if (index > -1)
            SetControl(index);
    }

    /// <summary>
    /// Checks if a drops under player's control
    /// </summary>
    /// <param name="drop">drop to check control</param>
    public bool IsUnderControl(GameObject drop)
    {
        return allCurrentCharacters.Contains(drop);
    }

    /// <summary>
    /// Looks for the next drop available in the pool
    /// </summary>
    private GameObject FindNextAvalaibleDrop()
    {
        GameObject res = null;

        int i = 0;

        Debug.Log("_DropsPool.Count "+_DropsPool.Count.ToString());

        for (; i < _DropsPool.Count && res == null; ++i)
        {
            Debug.Log("Pos "+i.ToString());
            if (!allCurrentCharacters.Contains(_DropsPool[i]))
            {
                res = _DropsPool[i];

                Debug.Log("Found "+ i.ToString());
            }
        }

        Debug.Log("end "+i.ToString());

        return res;
    }

}