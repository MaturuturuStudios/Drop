using UnityEngine;
using System.Collections;

public class Game_Controller : MonoBehaviour {

	// External references
	public GameObject character;

	void Start () {
		// Do nothing
	}

	void Update () {
		// Horizontal input
		float h_input = Input.GetAxis ("Horizontal");
		character.GetComponent<Character_Movement>().SetInput(h_input);

		// Jump input
		float jump_input = Input.GetAxis ("Jump");
		if (jump_input > 0)
			character.GetComponent<Character_Jump>().Jump();
	}
}