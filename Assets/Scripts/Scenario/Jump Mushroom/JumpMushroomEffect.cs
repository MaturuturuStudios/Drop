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
	/// The effect will scale with the character's size.
	/// </summary>
	public bool scaleWithSize = true;

	/// <summary>
	/// Time in seconds for the effect to be destroyed.
	/// </summary>
	public float effectDuration;

	void Start() {
		// Subscribes itself to thepublisher
		GetComponent<JumpMushroom>().AddListener(this);
	}

	public void OnBounce(JumpMushroom mushroom, GameObject bouncingCharacter, Vector3 bounceVelocity, Vector3 collisionPoint, Vector3 collisionNormal) {
		GameObject effect = (GameObject) Instantiate(effectPrefab, collisionPoint, Quaternion.LookRotation(Vector3.forward, collisionNormal));
		GameControllerTemporal.AddTemporal(effect);
		if (scaleWithSize) {
			CharacterSize characterSize = bouncingCharacter.GetComponent<CharacterSize>();
			effect.transform.localScale = Vector3.one;
			if (characterSize != null)
				effect.transform.localScale *= Mathf.Sqrt(characterSize.GetSize());
        }
		Destroy(effect, effectDuration);
	}
}
