using UnityEngine;
using System.Collections;
using System;

public class CharacterControllerCustomPlayer : MonoBehaviour {

	// Public attributes
	public float maxSpeed = 8;
	public float accelerationOnGround = 10;
	public float accelerationOnAir = 5;

	// Private attributes
	private bool _isFacingRight;
	private CharacterControllerCustom _controller;
	private float _normalizedHorizontalSpeed;

	public void Start() {
		// Recovers the other components
		_controller = GetComponent<CharacterControllerCustom>();

		// By default, the player starts facing right
		_isFacingRight = true;
	}

	public void Update() {
		HandleInput();

		float acceleration = _controller.State.IsGrounded ? accelerationOnGround : accelerationOnAir;
		_controller.SetHorizontalForce(Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * maxSpeed,  acceleration * Time.deltaTime));
	}

	private void HandleInput() {
		if (Input.GetKey(KeyCode.D)) {
			_normalizedHorizontalSpeed = 1;
			if (!_isFacingRight)
				Flip();
		}
		else if (Input.GetKey(KeyCode.A)) {
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight)
				Flip();
		}
		else {
			_normalizedHorizontalSpeed = 0;
		}

		if (_controller.CanJump && Input.GetKeyDown(KeyCode.Space))
			_controller.Jump();
	}

	private void Flip() {
		_isFacingRight = !_isFacingRight;
	}

	public void SetInput(float h_input) {
		// TODO
	}

	public void Jump() {
		// TODO
	}
}
