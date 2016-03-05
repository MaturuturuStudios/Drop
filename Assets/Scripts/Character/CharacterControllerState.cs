using System;
using UnityEngine;
using System.Text;

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
	/// If the character has been sent flying and it has not stopped yet.
	/// </summary>
	public bool IsFlying { get; set; }

	/// <summary>
	/// The object the character is grounded on.
	/// </summary>
	public GameObject GroundedObject { get; set; }

	/// <summary>
	/// The velocity of the platform the character is standing on.
	/// </summary>
	public Vector3 PlatformVelocity { get; set; }

	/// <summary>
	/// If the character was standing on a slope the last frame.
	/// </summary>
	public bool IsOnSlope { get; set; }

	/// <summary>
	/// If the character was sliding along a slope the last frame.
	/// A character sliding will always be on a slope.
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
		IsFlying = false;
		GroundedObject = null;
		PlatformVelocity = Vector3.zero;
		IsOnSlope = false;
		IsSliding = false;
		SlopeAngle = 0;
	}

	/// <summary>
	/// Overrides the ToString method with a more detailed information string.
	/// </summary>
	/// <returns>Detailed information about the state</returns>
	override public String ToString() {
		StringBuilder sb = new StringBuilder();
		sb.Append("Has Collisions: ").Append(HasCollisions).Append("\n");
		sb.Append("Is Grounded: ").Append(IsGrounded).Append("\n");
		sb.Append("Is Falling: ").Append(IsFalling).Append("\n");
		sb.Append("Is Flying: ").Append(IsFlying).Append("\n");
		sb.Append("Grounded Object: ").Append(GroundedObject.name).Append("\n");
		sb.Append("Platform Velocity: ").Append(PlatformVelocity).Append("\n");
		sb.Append("Is On Slope: ").Append(IsOnSlope).Append("\n");
		sb.Append("Is Sliding: ").Append(IsSliding).Append("\n");
		sb.Append("Slope Angle: ").Append(SlopeAngle);
		return sb.ToString();
	}

	#endregion
}
