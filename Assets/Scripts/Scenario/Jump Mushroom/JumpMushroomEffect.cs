using UnityEngine;

/// <summary>
/// Listener to the mushroom's events that creates the bouncing effect.
/// </summary>
[RequireComponent(typeof(JumpMushroom))]
public class JumpMushroomEffect : MonoBehaviour, JumpMushroomListener {

	/// <summary>
	/// The effect created when something bounces on it.
	/// </summary>
	public GameObject effectPrefab;

	/// <summary>
	/// Time in seconds for the effect to be destroyed.
	/// </summary>
	public float effectDuration;

	void Start() {
		// Subscribes itself to thepublisher
		GetComponent<JumpMushroom>().AddListener(this);
	}

	public void OnBounce(JumpMushroom mushroom, GameObject bouncingCharacter, Vector3 bounceVelocity, Vector3 collisionPoint, Vector3 collisionNormal) {
		Object effect = Instantiate(effectPrefab, collisionPoint, Quaternion.LookRotation(Vector3.forward, collisionNormal));
		Destroy(effect, effectDuration);
	}
}
