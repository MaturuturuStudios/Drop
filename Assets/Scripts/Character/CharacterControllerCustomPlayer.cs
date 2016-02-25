using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Communication point beetween the input system and the character controller.
/// </summary>
public class CharacterControllerCustomPlayer : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// The time the jump button will be considered pressed.
	/// </summary>
	public float jumpPressTolerance = 0.1f;

	/// <summary>
	/// Amount of friction applied to the perpendicular velocity while the
	/// player is on a wind tube.
	/// </summary>
	[Range(0, 1)]
	public float windTubeFriction = 0.05f;

	#endregion

	#region Properties

	/// <summary>
	/// Signed-normalized horizontal input the controller will receive this frame.
	/// </summary>
	public float HorizontalInput { get; set; }

	/// <summary>
	/// Signed-normalized vertical input the controller will receive this frame.
	/// </summary>
	public float VerticalInput { get; set; }

	/// <summary>
	/// Signed-normalized input for the jump control.
	/// </summary>
	public float JumpInput { get; set; }

	/// <summary>
	/// The direction the character is fancing, defined by the last input received.
	/// </summary>
	public Vector3 FacingDirection { get; private set; }

	/// <summary>
	/// The wind tube which the player is currently standing on.
	/// </summary>
	public WindTube CurrentWindTube { get; set; }

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's CharacterControllerCustom component.
	/// </summary>
	private CharacterControllerCustom _controller;

	/// <summary>
	/// Flag that indicates if the player has released the jump button.
	/// </summary>
	private bool _jumpReleased;

	/// <summary>
	/// Time since the jump button was pressed.
	/// </summary>
	private float _jumpPressTime;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called as soon as this entity is created.
	/// It will be called even if the entity is disabled.
	/// </summary>
	public void Awake() {
		// Recovers the other components
		_controller = GetComponent<CharacterControllerCustom>();

		// By default, the player starts facing right
		FacingDirection = Vector3.right;

		// Sets the jump button flag
		_jumpReleased = true;
	}

	/// <summary>
	/// Unity's method called every frame.
	/// Sends the input to the controller.
	/// </summary>
	public void Update() {
		// Decreses the timers
		_jumpPressTime -= Time.deltaTime;

		// Checks where the player is facing
		FacingDirection = new Vector3(HorizontalInput, VerticalInput, 0).normalized;

		if (CurrentWindTube == null)
			// Sets the input force of the character controller
			_controller.SetInputForce(HorizontalInput, VerticalInput);
		else {
			// The character is on a wind tube
			Vector3 movementForce = Vector3.Project(_controller.Velocity, CurrentWindTube.transform.right);

			// Adds a friction to the velocity
			movementForce *= (1 - windTubeFriction);

			// Adds the input force
			Vector3 inputForce = new Vector3(HorizontalInput, VerticalInput, 0);
			inputForce = Vector3.Project(inputForce, CurrentWindTube.transform.right);

			// Applys the final force
			Vector3 finalForce = Vector3.Project(_controller.Velocity, CurrentWindTube.transform.up);
			finalForce += movementForce + inputForce;
			_controller.SetForce(finalForce);
		}

		// Checks if the jump button has been recently pressed
		if (JumpInput > 0 && _jumpReleased) {
			_jumpReleased = false;
			_jumpPressTime = jumpPressTolerance;
		}
		else if (JumpInput <= 0)
			_jumpReleased = true;

		// Makes the character jump
		if (_jumpPressTime >= 0 && _controller.CanJump()) {
			_jumpPressTime = -1;
			_controller.Jump();
		}
	}

	/// <summary>
	/// Removes the input of the controller. If the optional parameter
	/// is received, instantly stops the entity, removing it's velocity.
	/// </summary>
	/// <param name="hardStop">If the entity velocity should be removed</param>
	public void Stop(bool hardStop = false) {
		// Removes the input
		HorizontalInput = 0;
		VerticalInput = 0;
		JumpInput = 0;

		// If it has to, hard stops the controller
		if (hardStop)
			_controller.Stop();
	}

	#endregion
}
