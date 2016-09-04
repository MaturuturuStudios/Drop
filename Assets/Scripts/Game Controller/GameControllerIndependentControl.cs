using UnityEngine;
using System.Collections.Generic;

public class GameControllerIndependentControl : MonoBehaviour {

    #region Attributes

    /// <summary>
    /// Drop prefab reference ot instantiate
    /// </summary>
    public GameObject PfDrop;

    /// <summary>
    /// Current character controlled
    /// </summary>
    [HideInInspector]
    public GameObject currentCharacter;

    /// <summary>
    /// List of all current characters controlled
    /// </summary>
    [HideInInspector]
    public List<GameObject> allCurrentCharacters;


    /// <summary>
    /// Drop number to diferenciate the instantiated characters
    /// </summary>
    private int _dropNameCounter;


    /// <summary>
    /// Reference to main camera controller
    /// </summary>
    private MainCameraController _cameraController;

    #endregion

    #region Methods

    void Awake() {

        //Get the camera controller
        _cameraController = GetComponentInChildren<MainCameraController>();

        //Get the mext drop number for label
        _dropNameCounter = allCurrentCharacters.Count;
    }


    public GameObject CreateDrop(bool setControl = false, bool addToControlList = true, int size = 1) {
        return CreateDrop(Vector3.zero, setControl, addToControlList, size);
    }

    /// <summary>
    /// Add a drop to the scene, and under player control.
    /// Under player's control from list and set it out of the scene
    /// </summary>
    /// <param name="setControl">Set the control to the drop</param>
    /// <param name="addToControlList">Set the created drop to the controlled drops list</param>
    /// <param name="size">Set the desired size to instantiated character</param>
    public GameObject CreateDrop(Vector3 initialPosition, bool setControl = false, bool addToControlList = true, int size = 1) {

        // Create a drop
        GameObject drop = Instantiate(PfDrop, initialPosition, Quaternion.identity) as GameObject;

		// Set the name to the object
        drop.gameObject.name = "Drop" + ++_dropNameCounter;

        // Look for the characters pool in hierarchy
        GameObject charactersPool = GameObject.Find("Characters");
        // If don't exist create it
        if (!charactersPool)
            charactersPool = new GameObject("Characters");
        // Put it into the Characters object
        drop.transform.parent = charactersPool.transform;

        // Set the desired size
        drop.GetComponent<CharacterSize>().SetSize(size);

        //Add to controled drop list
        if (addToControlList)
            allCurrentCharacters.Add(drop);

        //Set Control
        if (setControl)
            SetControl(drop);

        return drop;
    }

    /// <summary>
    /// Remove a drop under player's control from list and set it out of the scene
    /// </summary>
    /// <param name="drop">drop to remove from control & scene</param>
    /// <param name="setControlNextDrop">On drop deleted set the control to next drop</param>
    public void DestroyDrop(GameObject drop, bool setControlNextDrop = false) {

        if (!IsUnderControl (drop) || allCurrentCharacters.Count > 1) {

            //Set control to the next drop if it was the one under control
            if (setControlNextDrop && currentCharacter == drop)
                ControlNextDrop();
            //remove from control list
            allCurrentCharacters.Remove(drop);

			//Disactive drop because it isn't destroy untill end of frame
            drop.SetActive(false);

            //Destroy drop
            Destroy(drop);
        }
    }

    /// <summary>
    /// Sets the control to the next drop in list
    /// </summary>
    public void ControlNextDrop() {

        if (allCurrentCharacters.Count > 1) {

            //Get next index
            int next_drop = allCurrentCharacters.IndexOf(currentCharacter) + 1;

            //Loop control
            if (next_drop == allCurrentCharacters.Count)
                next_drop = 0;

            //Set Control
            currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();
            SetControl(next_drop);
        }
    }

    /// <summary>
    /// Sets the control to the previous drop in list
    /// </summary>
    public void ControlBackDrop() {

        if (allCurrentCharacters.Count > 1) {

            //Get prev index
            int back_drop = allCurrentCharacters.IndexOf(currentCharacter) - 1;

            //Loop control
            if (back_drop == -1)
                back_drop = allCurrentCharacters.Count - 1;

            //Set Control
            currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();
            SetControl(back_drop);
        }
    }

    /// <summary>
    /// Sets the control to an specific drop
    /// </summary>
    /// <param name="index">Index of drop in the allCurentCharacters list</param>
    public void SetControl(int index, bool isFusion = false) {

        // Can't change controll if it is in shoot mode
        bool notShoothing = (currentCharacter == null || !currentCharacter.GetComponent<CharacterShoot>().isShooting());

        if (index < allCurrentCharacters.Count && index > -1 && notShoothing) {
            //Try to stop abandoned drop
            if(currentCharacter != null)
                currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();

            //Set Control to new drop
            currentCharacter = allCurrentCharacters[index];

            //Update camera objective
            _cameraController.SetObjective(currentCharacter, isFusion);
        }
    }

    /// <summary>
    /// Set the control to the given drop if is on the list of drops under player's control
    /// </summary>
    /// <param name="drop">drop to control</param>
    public void SetControl(GameObject drop, bool isFusion = false) {
        int index = allCurrentCharacters.IndexOf(drop);

        if (index > -1)
            SetControl(index, isFusion);
        else {
            index = allCurrentCharacters.Count;
            allCurrentCharacters.Add(drop);
            SetControl(index, isFusion);
        }
    }

    /// <summary>
    /// Checks if a drops under player's control
    /// </summary>
    /// <param name="drop">drop to check control</param>
    public bool IsUnderControl(GameObject drop) {
        return allCurrentCharacters.Contains(drop);
    }

    /// <summary>
    /// Count how many characters there are into the scene
    /// if splited, will count the size of the drop
    /// example, two drops of size two will return 4 if splitted is true
    /// </summary>
    /// <param name="splited">if true, will count the size of a drop as independent</param>
    public int CountAllDrops(bool splited = false) {
        // Look for all drops
        GameObject[] drops = GameObject.FindGameObjectsWithTag("Player");

        // Count all drops
        int totalDrops = 0;
        for (int i = 0; i < drops.Length; ++i) {
            if (splited)
                totalDrops += drops[i].GetComponent<CharacterSize>().GetSize();
            else
                ++totalDrops;
        }

        return totalDrops;

    }

    /// <summary>
    /// Count how many characters there are into the scene
    /// if splited, will count the size of the drop
    /// example, two drops of size two will return 4 if splitted is true
    /// </summary>
    /// <param name="splited">if true, will count the size of a drop as independent</param>
    public int CountControlledDrops(bool splited = false) {
        int drops = allCurrentCharacters.Count;
        if (splited) {
            drops = 0;
            for(int i=0; i<allCurrentCharacters.Count; i++) {
                drops += allCurrentCharacters[i].GetComponent<CharacterSize>().GetSize();
            }
        }
        return drops;
    }
    #endregion
}