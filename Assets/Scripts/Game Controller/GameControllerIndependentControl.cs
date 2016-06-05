using UnityEngine;
using System.Collections.Generic;

public class GameControllerIndependentControl : MonoBehaviour {

    #region Attributes
    /// <summary>
    /// Current character controlled
    /// </summary>
    public GameObject currentCharacter;
    /// <summary>
    /// All current characters controlled list
    /// </summary>
    public List<GameObject> allCurrentCharacters;


	/// <summary>
    /// Drop prefab reference
    /// </summary>
    public GameObject PfDrop;
	//Drop name number when dinamically create it
    private int _dropNameCounter;
    
    //Camera reference
    private MainCameraController _cameraController;
    #endregion

    #region Methods
    /// <summary>
    /// Unity's method called when this entity is created, even if it is disabled.
    /// </summary>
    void Awake() {
        //Get the camera
        _cameraController = GetComponentInChildren<MainCameraController>();

	}
	
    /// <summary>
    /// Unity's method called on start script only one time
    /// </summary>
    void Start() {
		//Get the mext drop number
        _dropNameCounter = allCurrentCharacters.Count;
        if (_dropNameCounter == 0 && currentCharacter != null) {
            allCurrentCharacters.Add(currentCharacter);
            ++_dropNameCounter;
        }

    }

    /// <summary>
    /// Add a drop to the scene, and under player control.nder player's control from list and set it out of the scene
    /// </summary>
    /// <param name="setControl">Set the control to the drop</param>
    /// <param name="addToControlList">Set the created drop to the controlled drops list</param>
    public GameObject CreateDrop(bool setControl = false, bool addToControlList = true) {
        //Create a drop
        GameObject drop = Instantiate(PfDrop);
		//Set the name to the object
        drop.gameObject.name = "Drop" + ++_dropNameCounter;

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
    #endregion
}