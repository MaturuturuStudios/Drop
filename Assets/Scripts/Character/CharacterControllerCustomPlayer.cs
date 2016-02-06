using UnityEngine;
using System.Collections;
using System;

public class CharacterControllerCustomPlayer : MonoBehaviour {

	// Properties
	public float HorizontalInput { get; set; }
	public float VerticalInput { get; set; }
	public float JumpInput { get; set; }
	public Vector3 FacingDirection { get; set; }

	// Private attributes
	private CharacterControllerCustom _controller;

	public void Start() {
		// Recovers the other components
		_controller = GetComponent<CharacterControllerCustom>();

		// By default, the player starts facing right
		FacingDirection = Vector3.right;
	}

	public void Update() {
		// Checks where the player is facing
		FacingDirection = new Vector3(HorizontalInput, VerticalInput, 0).normalized;

		// Adds the force to the character controller
		_controller.SetInputForce(HorizontalInput, VerticalInput);

		// Makes the character jump
		if (JumpInput > 0)
			_controller.Jump();
	}
}
