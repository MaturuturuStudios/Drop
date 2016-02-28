using UnityEngine;
using System.Collections;
using System;

public class GameControllerInput : MonoBehaviour {

    // Internal references
    private GameControllerIndependentControl _switcher;
    private CameraSwitcher _cameraSwitcher;
    private CameraDebugController _cameraDebugControler;
    private bool _shootmodeON=false;

	void Start() {
		// Do nothing
		_switcher = GetComponent<GameControllerIndependentControl>();
        _cameraSwitcher = GameObject.FindGameObjectWithTag("CameraSet")
                                .GetComponent<CameraSwitcher>();
        _cameraDebugControler = GameObject.FindGameObjectWithTag("DebugCamera")
                                .GetComponent<CameraDebugController>();
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

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("Switch orthographic mode");
            if (_cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG)
                _cameraSwitcher.SetCameraMode(CameraSwitcher.CameraMode.GAME);
            else
                _cameraSwitcher.SetCameraMode(CameraSwitcher.CameraMode.DEBUG);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Switch travel mode");
            if (_cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.TRAVEL)
                _cameraSwitcher.SetCameraMode(CameraSwitcher.CameraMode.GAME);
            else
                _cameraSwitcher.SetCameraMode(CameraSwitcher.CameraMode.TRAVEL);
        }
        if (Input.GetKeyDown(KeyCode.F3) && _cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG)
        {
            Debug.Log("Back camera");
            _cameraSwitcher.BackCamera();
        }
        if (Input.GetKeyDown(KeyCode.F4) && _cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG)
        {
            Debug.Log("Next camera");
            _cameraSwitcher.NextCamera();

        }
        
        if (_cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG) {
            Vector3 nextPosition = _cameraDebugControler.debugObjective.transform.position;
            if (Input.GetKey(KeyCode.U))
            {
                Debug.Log("set camera near");
                 _cameraDebugControler.SetPhantomMovement(CameraDebugController.PhantomMovement.NEAR);
            }
            if (Input.GetKey(KeyCode.I))
            {
                Debug.Log("set camera up");
                _cameraDebugControler.SetPhantomMovement(CameraDebugController.PhantomMovement.UP);
            }
            if (Input.GetKey(KeyCode.O))
            {
                Debug.Log("set camera far");
                _cameraDebugControler.SetPhantomMovement(CameraDebugController.PhantomMovement.FAR);
            }
            if (Input.GetKey(KeyCode.J))
            {
                Debug.Log("set camera to left");
                _cameraDebugControler.SetPhantomMovement(CameraDebugController.PhantomMovement.LEFT);
            }
            if (Input.GetKey(KeyCode.K))
            {
                Debug.Log("set camera down");
                _cameraDebugControler.SetPhantomMovement(CameraDebugController.PhantomMovement.DOWN);
            }
            if (Input.GetKey(KeyCode.L))
            {
                Debug.Log("set camera to right");
                _cameraDebugControler.SetPhantomMovement(CameraDebugController.PhantomMovement.RIGHT);
            }
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