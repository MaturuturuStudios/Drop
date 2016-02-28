using UnityEngine;
using System.Collections;
using System;

public class GameControllerInput : MonoBehaviour {

    // Internal references
    private GameControllerIndependentControl _switcher;
    private CameraController _cameraControler;
    private bool _shootmodeON=false;

	void Start() {
		// Do nothing
		_switcher = GetComponent<GameControllerIndependentControl>();
        _cameraControler =  _switcher._cameraController;
}

	void Update() {
        //TODO
        // if you put it in this line in Start() you can't change of drop, you lose the reference
        // I have to take over of here and actualize it when control is changed in GameControllerIndependentControl
		CharacterControllerCustomPlayer cccp = _switcher.currentCharacter.GetComponent<CharacterControllerCustomPlayer>();

		// Horizontal input
		float hInput = Input.GetAxis("Horizontal");
		cccp.HorizontalInput = hInput;

		// Vertical input
		float vInput = Input.GetAxis("Vertical");
		cccp.VerticalInput = vInput;

		// Jump input
		float jumpInput = Input.GetAxis("Jump");
		cccp.JumpInput = jumpInput;


   

        HasToChange();
        DebugCamera();
        DebugAddDrops();
        DebugSize();
	}

    /// <summary>
    /// Control the size
    /// </summary>
    private void DebugSize() {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
            _switcher.currentCharacter.GetComponent<CharacterSize>().IncrementSize();

        if(Input.GetKeyDown(KeyCode.KeypadMinus))
            _switcher.currentCharacter.GetComponent<CharacterSize>().DecrementSize();

        //Some number pressed? we use 1-9 as range 1-9
        bool done = false;
        for(int i = 1; i < 10 && !done; i++) {
            if(Input.GetKeyDown("" + i)) {
                _switcher.currentCharacter.GetComponent<CharacterSize>().SetSize(i);
                done = true;
            }
        }
    }

    private void HasToChange() {

        if (Input.GetKeyDown(KeyCode.Keypad1))
            _switcher.SetControl(0);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            _switcher.SetControl(1);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            _switcher.SetControl(2);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            _switcher.SetControl(3);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            _switcher.ControlBackDrop();
        if (Input.GetKeyDown(KeyCode.Keypad6))
            _switcher.ControlNextDrop();

    }

    private void DebugCamera()
    {

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Switch debug mode");
            if(_cameraControler.cameraMode == CameraController.CameraMode.DEBUG)
                _cameraControler.cameraMode = CameraController.CameraMode.GAME;
            else
                _cameraControler.cameraMode = CameraController.CameraMode.DEBUG;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("Switch intro mode");
            if (_cameraControler.cameraMode == CameraController.CameraMode.INTRO)
                _cameraControler.cameraMode = CameraController.CameraMode.GAME;
            else
                _cameraControler.cameraMode = CameraController.CameraMode.INTRO;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("set camera near");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("set camera up");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("set camera far");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("set camera to left");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("set camera down");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("set camera to right");
        }


    }

    private void DebugAddDrops() {

        if(Input.GetKeyDown(KeyCode.Keypad7)) {
            GameObject drop = _switcher.AddDrop(true);
            Debug.Log("add drop");
        }
        

        if (Input.GetKeyDown(KeyCode.Keypad8))
            _switcher.KillDrop(_switcher.currentCharacter);
        

    }
}