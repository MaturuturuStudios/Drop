using UnityEngine;

/// <summary>
/// Interface for listeners to a JumpMushroom component.
/// </summary>
public interface JumpMushroomListener {

	/// <summary>
	/// Method called when an object bounces on the mushroom
	/// </summary>
	/// <param name="mushroom">The mushroom the object is bouncing on</param>
	/// <param name="bouncingCharacter">The object bouncing on the mushroom</param>
	/// <param name="bounceVelocity">The velocity of the bounce</param>
	void OnBounce(JumpMushroom mushroom, GameObject bouncingCharacter, Vector3 bounceVelocity, Vector3 collisionPoint, Vector3 collisionNormal);
}
