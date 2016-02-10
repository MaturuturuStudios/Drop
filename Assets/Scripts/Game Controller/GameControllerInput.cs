using UnityEngine;
using System.Collections;
using System;

public class GameControllerInput : MonoBehaviour {

	// Internal references
	private GameControllerIndependentControl _switcher;

	void Start() {
		// Do nothing
		_switcher = GetComponent<GameControllerIndependentControl>();
	}

	void Update() {
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

	}

    private void HasToChange()
    {

        if (Input.GetKeyDown(KeyCode.Keypad1))
            _switcher.SetControl(0);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            _switcher.SetControl(1);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            _switcher.SetControl(2);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            _switcher.SetControl(3);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            _switcher.ControlNextDrop();
        if (Input.GetKeyDown(KeyCode.Keypad6))
            _switcher.ControlBackDrop();

    }
}