using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Communication point beetween the input system and the character controller.
/// </summary>
public class CharacterControllerCustomPlayer : MonoBehaviour {

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
	/// If the character has been sent flying and it has not stopped yet.
	/// </summary>
	public bool IsFlying { get; private set; }

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's CharacterControllerCustom component.
	/// </summary>
	private CharacterControllerCustom _controller;

	/// <summary>
	/// After been sent flying, specifies if the character will stop flying when
	/// it becomes grounded.
	/// </summary>
	private bool _stopFlyingWhenGrounded;

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
	}

	/// <summary>
	/// Unity's method called every frame.
	/// Sends the input to the controller.
	/// </summary>
	public void Update() {
		// Checks where the player is facing
		FacingDirection = new Vector3(HorizontalInput, VerticalInput, 0).normalized;

		// If the character is flying and it's grounded, stops it from flying
		if (IsFlying && _stopFlyingWhenGrounded && _controller.State.IsGrounded)
			StopFlying();

		// Adds the force to the character controller
		_controller.SetInputForce(HorizontalInput, VerticalInput);

		// Makes the character jump
		if (JumpInput > 0)
			_controller.Jump();
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

	/// <summary>
	/// Sends the character flying with a velocity. The character's current
	/// velocity will be replaced by the new one. The character won't stop
	/// and it will ignore the input.
	/// </summary>
	/// <param name="velocity">The new character's velocity</param>
	/// <param name="useMass">If the velocity change should consider the character's mass</param>
	/// <param name="restoreWhenGrounded">If the character should return to normal when grounded</param>
	public void SendFlying(Vector3 velocity, bool useMass = false, bool restoreWhenGrounded = true) {
		// Stops the character
		_controller.Stop();

		// Adds the velocity to the character
		ForceMode mode = useMass ? ForceMode.Impulse : ForceMode.VelocityChange;
		_controller.AddForce(velocity, mode);

		// Changes the character's parameters
		_controller.Parameters = CharacterControllerParameters.FlyingParameters;

		// Sets the flags
		IsFlying = true;
		_stopFlyingWhenGrounded = restoreWhenGrounded;
	}

	/// <summary>
	/// Stops the character from being flying. The character's parameters
	/// will be restored to their default values.
	/// </summary>
	public void StopFlying() {
		// Restores the character's parameters and sets down the flying flag
		_controller.Parameters = null;
		IsFlying = false;
	}

	#endregion
}
