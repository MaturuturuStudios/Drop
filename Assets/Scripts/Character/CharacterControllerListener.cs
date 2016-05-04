using UnityEngine;

/// <summary>
/// Interface for the observers listening for the
/// character controller's events.
/// </summary>
public interface CharacterControllerListener {

	/// <summary>
	/// Event called when the character jumps.
	/// </summary>
	/// <param name="ccc">The character controller</param>
	void OnJump(CharacterControllerCustom ccc);

	/// <summary>
	/// Event called at the start of a collision.
	/// </summary>
	/// <param name="ccc">The character controller</param>
	/// <param name="hit">The information about the collision</param>
	void OnPreCollision(CharacterControllerCustom ccc, ControllerColliderHit hit);

	/// <summary>
	/// Event called at the end of a collision.
	/// </summary>
	/// <param name="ccc">The character controller</param>
	/// <param name="hit">The information about the collision</param>
	void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit);
}
