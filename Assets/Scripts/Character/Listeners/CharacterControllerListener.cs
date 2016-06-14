using UnityEngine;

/// <summary>
/// Interface for the observers listening for the
/// character controller's events.
/// </summary>
public interface CharacterControllerListener {

	/// <summary>
	/// Event called when the character's jump animation begins.
	/// </summary>
	/// <param name="ccc">The character controller</param>
	/// <param name="delay">Duration of the animation</param>
	void OnBeginJump(CharacterControllerCustom ccc, float delay);

	/// <summary>
	/// Event called when the character jumps.
	/// </summary>
	/// <param name="ccc">The character controller</param>
	void OnPerformJump(CharacterControllerCustom ccc);

	/// <summary>
	/// Event called when the character jumps.
	/// </summary>
	/// <param name="ccc">The character controller</param>
	void OnWallJump(CharacterControllerCustom ccc);

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

/// <summary>
/// Adapter for the CharacterControllerListener interface used to
/// avoid forcing each class to implement all it's methods.
/// </summary>
public class CharacterControllerAdapter : MonoBehaviour, CharacterControllerListener {

	public void OnBeginJump(CharacterControllerCustom ccc, float delay) {
		// Do nothing
	}

	public void OnPerformJump(CharacterControllerCustom ccc) {
		// Do nothing
	}

	public void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		// Do nothing
	}

	public void OnPreCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		// Do nothing
	}

	public void OnWallJump(CharacterControllerCustom ccc) {
		// Do nothing
	}
}
