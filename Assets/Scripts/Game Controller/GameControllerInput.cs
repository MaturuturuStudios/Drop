using UnityEngine;
using System.Collections;

public class GameControllerInput : MonoBehaviour {

	// External references
	public GameObject currentCharacter;
    
	void Start () {
        // Do nothing
       
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

    }
}
