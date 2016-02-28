using UnityEngine;
using System.Collections;
using System;

public class GameControllerInput : MonoBehaviour {

    // Internal references
    private GameControllerIndependentControl _switcher;
    private bool _shootmodeON=false;


	void Start() {
		// Do nothing
		_switcher = GetComponent<GameControllerIndependentControl>();

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

        //change drop input
        if (Input.GetButtonDown("BackDrop"))
            _switcher.ControlBackDrop();
        if (Input.GetButtonDown("NextDrop"))
            _switcher.ControlNextDrop();


        if ((Input.GetButtonDown("Aim"))) _switcher.currentCharacter.GetComponent<CharacterShoot>().Aim();
        if ((Input.GetButtonDown("Fire"))) _switcher.currentCharacter.GetComponent<CharacterShoot>().Shoot();

        HasToChange();
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