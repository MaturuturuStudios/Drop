using UnityEngine;
using System.Collections;
using System;

public class GameControllerInput : MonoBehaviour {

    // Internal references
    private GameControllerIndependentControl _switcher;
    private CameraSwitcher _cameraSwitcher;
    private CameraDebugController _cameraDebugControler;
    private Camera _camera;
    private bool _shootmodeON=false;

	void Start() {
		// Do nothing
		_switcher = GetComponent<GameControllerIndependentControl>();
        _cameraSwitcher = GameObject.FindGameObjectWithTag("CameraSet")
                                .GetComponent<CameraSwitcher>();
        _cameraDebugControler = GameObject.FindGameObjectWithTag("DebugCamera")
                                .GetComponent<CameraDebugController>();
        _camera = GameObject.FindGameObjectWithTag("MainCamera")
                                .GetComponent<Camera>();
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
        //Switch debug mode
        if (Input.GetKeyDown(KeyCode.F1))
            _cameraSwitcher.SetCameraMode(CameraSwitcher.CameraMode.DEBUG);

        //Switch travel mode
        if (Input.GetKeyDown(KeyCode.F2))
            _cameraSwitcher.SetCameraMode(CameraSwitcher.CameraMode.TRAVEL);

        //Back camera
        if (Input.GetKeyDown(KeyCode.F3) && _cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG)
            _cameraSwitcher.BackCamera();

        //Next Camera
        if (Input.GetKeyDown(KeyCode.F4) && _cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG)
            _cameraSwitcher.NextCamera();
        
        //Debug Camera Input
        if (_cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveClose = Input.GetAxis("Vertical");// Y rotation?
            float moveVertical = 0.0f;
            //At the moment I put it here I have to move it to L & R
            if (Input.GetKey(KeyCode.Q))
                moveVertical += 1.0f;
            if (Input.GetKey(KeyCode.E))
                moveVertical -= 1.0f;

            Vector3 movement = new Vector3(moveHorizontal, moveVertical, moveClose);

            _cameraDebugControler.SetMovement(movement);

            float mouseAxisX = Input.GetAxis("Mouse X");
            float mouseAxisY = Input.GetAxis("Mouse Y");
            _cameraDebugControler.SetLookAt(mouseAxisX, mouseAxisY);
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