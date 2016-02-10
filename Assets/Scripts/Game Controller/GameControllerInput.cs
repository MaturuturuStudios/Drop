using UnityEngine;
using System.Collections;
using System;

public class GameControllerInput : MonoBehaviour {

	// External references
	public GameObject currentCharacter;
	public GameObject[] allCharacters;

	// Internal references
	private GameControllerIndependentControl switcher;

	void Start() {
		// Do nothing
		switcher = GetComponent<GameControllerIndependentControl>();
	}

	void Update() {
		CharacterControllerCustomPlayer cccp = currentCharacter.GetComponent<CharacterControllerCustomPlayer>();

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



	private void HasToChange() {

		if (Input.GetKeyDown(KeyCode.Keypad1)) 
			currentCharacter = allCharacters[0];
		
		if (Input.GetKeyDown(KeyCode.Keypad2) && allCharacters.Length > 1) 
			currentCharacter = allCharacters[1];
		
		if (Input.GetKeyDown(KeyCode.Keypad3) && allCharacters.Length > 2) 
			currentCharacter = allCharacters[2];
		
		if (Input.GetKeyDown(KeyCode.Keypad4) && allCharacters.Length > 3) 
			currentCharacter = allCharacters[3];
		
		if (Input.GetKeyDown(KeyCode.Keypad5) && allCharacters.Length > 1) {
			int next_character = Array.IndexOf(allCharacters, currentCharacter) + 1;
			if (next_character == allCharacters.Length)
				next_character = 0;
			currentCharacter = allCharacters[next_character];
		}
		if (Input.GetKeyDown(KeyCode.Keypad6) && allCharacters.Length > 1) {
			int next_character = Array.IndexOf(allCharacters, currentCharacter) - 1;
			if (next_character == -1)
				next_character = allCharacters.Length - 1;
			currentCharacter = allCharacters[next_character];
		}

	}
}