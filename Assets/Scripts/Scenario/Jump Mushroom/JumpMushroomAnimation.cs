using UnityEngine;

/// <summary>
/// Listener to the mushroom's events that plays an animation on the mushroom.
/// </summary>
[RequireComponent(typeof(JumpMushroom))]
public class JumpMushroomAnimation : MonoBehaviour, JumpMushroomListener {

	/// <summary>
	/// Reference to the affected animators.
	/// </summary>
	private Animator[] _animators;

	/// <summary>
	/// The key string to the bounce trigger on the animator.
	/// </summary>
	private static readonly string ANIMATOR_TRIGGER_STRING = "bounce";

	void Awake() {
		// Retrieves the desired components
		_animators = GetComponentsInChildren<Animator>();
	}

	void Start() {
		// Subscribes itself to thepublisher
		GetComponent<JumpMushroom>().AddListener(this);
	}

	public void OnBounce(JumpMushroom mushroom, GameObject bouncingCharacter, Vector3 bounceVelocity, Vector3 collisionPoint, Vector3 collisionNormal) {
		foreach (Animator animator in _animators)
			animator.SetTrigger(ANIMATOR_TRIGGER_STRING);
	}
}
