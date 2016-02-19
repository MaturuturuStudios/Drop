using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Contains information about the state of the character controller on the last frame.
/// </summary>
public class CharacterControllerState {

	#region Properties

	/// <summary>
	/// If the character has collided with anything the last frame.
	/// </summary>
	public bool HasCollisions { get; set; }

	/// <summary>
	/// If the character was grounded at the end of the last frame.
	/// </summary>
	public bool IsGrounded { get; set; }

	/// <summary>
	/// If the character was falling at the end of the last frame.
	/// </summary>
	public bool IsFalling { get; set; }

	/// <summary>
	/// The object the character is grounded on.
	/// </summary>
	public GameObject GroundedObject { get; set; }

	/// <summary>
	/// The velocity of the platform the character is standing on.
	/// </summary>
	public Vector3 PlatformVelocity { get; set; }

	/// <summary>
	/// If the character was sliding along a slope the last frame.
	/// </summary>
	public bool IsSliding { get; set; }

	/// <summary>
	/// The angle of the slope the character is stanging on. This value will be stored 
	/// even if the entity is not grounded on the object.
	/// </summary>
	public float SlopeAngle { get; set; }

	#endregion

	#region Methods

	/// <summary>
	/// Resets the value of all the attributes of the state.
	/// </summary>
	public void Reset() {
		HasCollisions = false;
		IsGrounded = false;
		IsFalling = false;
		GroundedObject = null;
		PlatformVelocity = Vector3.zero;
		IsSliding = false;
		SlopeAngle = 0;
	}

	#endregion
}
