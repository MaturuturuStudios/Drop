using UnityEngine;
using System.Collections;
using System;

public class GameControllerInput : MonoBehaviour {

	// Internal references
    private GameControllerIndependentControl _switcher;
    public GameObject PfDrop;

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

        HasToChange();

        DebugAddDrops();

	}

    void HasToChange()
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
            _switcher.ControlBackDrop();
        if (Input.GetKeyDown(KeyCode.Keypad6))
            _switcher.ControlNextDrop();

    }

    void DebugAddDrops()
    {

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            GameObject dropClone = (GameObject)Instantiate(PfDrop);
            _switcher.AddDrop(dropClone);
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            _switcher.RemoveDrop(_switcher.currentCharacter);
        }

    }
}