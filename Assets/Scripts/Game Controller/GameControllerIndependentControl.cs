using UnityEngine;
using System.Collections.Generic;

public class GameControllerIndependentControl : MonoBehaviour {

    //Control atributes
    public GameObject currentCharacter;
    public List<GameObject> allCurrentCharacters;

    //Drop prefab
    public GameObject PfDrop;
    private int _dropNameCounter;
    
    //Cameras
    public MainCameraController _cameraController;
    public Camera _fixedCameras;
    public List<FixedCameraController> _fixedCameraControllers;

    void Awake()
    {
        //Get the camera
        _cameraController = Camera.main.GetComponent<MainCameraController>();
        for (int i = 0; i < Camera.allCamerasCount; ++i)
            if (Camera.allCameras[i].CompareTag("FixedCamera"))
                _fixedCameraControllers.Add(Camera.allCameras[i]
                    .GetComponent<FixedCameraController>());

		//Actualize camera objective
		_cameraController.SetObjective(currentCharacter);
		for (int i = 0; i < _fixedCameraControllers.Count; ++i)
			_fixedCameraControllers[i].SetObjective(currentCharacter);

	}

    void Start() {
        _dropNameCounter = allCurrentCharacters.Count;
    }

    /// <summary>
    /// Add a drop to the scene, and under player control.nder player's control from list and set it out of the scene
    /// </summary>
    /// <param name="setControl">Set the control to the drop</param>
    public GameObject CreateDrop(bool setControl = false, bool addToControlList = true) {
        //Create a drop
        GameObject drop = (GameObject)Instantiate(PfDrop);
        drop.gameObject.name = "Drop" + ++_dropNameCounter;

        //Add to controled drop list
        if (addToControlList)
            allCurrentCharacters.Add(drop);

        //Move the drop to players position
        drop.transform.position = currentCharacter.transform.position;

        //Set Control
        if (setControl)
            SetControl(drop);

        return drop;
    }

    /// <summary>
    /// Remove a drop under player's control from list and set it out of the scene
    /// </summary>
    /// <param name="drop">drop to remove from control & scene</param>
    public void DestroyDrop(GameObject drop, bool setControl = false) {

        if (!IsUnderControl (drop) || allCurrentCharacters.Count > 1) {
            //Set control to the next drop if it was the one under control
            if (setControl && currentCharacter == drop)
                ControlNextDrop();
            //remove from control list
            allCurrentCharacters.Remove(drop);

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
    public void SetControl(int index) {
        bool shootmode = currentCharacter.GetComponent<CharacterShoot>().isShooting();
        if (index < allCurrentCharacters.Count && index > -1 && !shootmode) {
            //Try to stop abandoned drop
            currentCharacter.GetComponent<CharacterControllerCustomPlayer>().Stop();

            //Set Control to new drop
            currentCharacter = allCurrentCharacters[index];

            //Actualize camera objective
            _cameraController.SetObjective(currentCharacter);
            for (int i = 0; i < _fixedCameraControllers.Count; ++i)
                _fixedCameraControllers[i].SetObjective(currentCharacter);
        }
    }

    /// <summary>
    /// Set the control to the given drop if is on the list of drops under player's control
    /// </summary>
    /// <param name="drop">drop to control</param>
    public void SetControl(GameObject drop) {
        int index = allCurrentCharacters.IndexOf(drop);

        if (index > -1)
            SetControl(index);
        else {
            index = allCurrentCharacters.Count;
            allCurrentCharacters.Add(drop);
            SetControl(index);
        }
    }

    /// <summary>
    /// Checks if a drops under player's control
    /// </summary>
    /// <param name="drop">drop to check control</param>
    public bool IsUnderControl(GameObject drop) {
        return allCurrentCharacters.Contains(drop);
    }
}