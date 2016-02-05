using UnityEngine;
using System.Collections;
using System;

public class CharacterControllerCustomPlayer : MonoBehaviour {

	// Properties
	public float HorizontalInput { get; set; }
	public float JumpInput { get; set; }
	public bool FacingRight { get; set; }

	// Public attributes
	public float maxSpeed = 8;
	public float accelerationOnGround = 10;
	public float accelerationOnAir = 5;

	// Private attributes
	private CharacterControllerCustom _controller;

	public void Start() {
		// Recovers the other components
		_controller = GetComponent<CharacterControllerCustom>();

		// By default, the player starts facing right
		FacingRight = true;
	}

	public void Update() {
		// Checks where the player is facing
		if (HorizontalInput > 0)
			FacingRight = true;
		else if (HorizontalInput < 0)
			FacingRight = false;

		// Calculates the right acceleration
		float acceleration = _controller.State.IsGrounded ? accelerationOnGround : accelerationOnAir;

		// Adds the force to the character controller
		_controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, HorizontalInput * maxSpeed,  acceleration * Time.deltaTime));

		// Makes the character jump
		if (JumpInput > 0)
			_controller.Jump();
	}
}
