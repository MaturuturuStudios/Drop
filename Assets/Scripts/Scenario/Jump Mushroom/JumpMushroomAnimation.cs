using UnityEngine;

/// <summary>
/// Listener to the mushroom's events that creates plays an animation on the mushroom.
/// </summary>
[RequireComponent(typeof(JumpMushroom))]
public class JumpMushroomAnimation : MonoBehaviour, JumpMushroomListener {

	/// <summary>
	/// The effect created when something bounces on it.
	/// </summary>
	private Animator _animator;

	/// <summary>
	/// The key string to the bounce trigger on the animator.
	/// </summary>
	private static readonly string ANIMATOR_TRIGGER_STRING = "Bounce";

	void Awake() {
		// Retrieves the desired components
		_animator = GetComponentInChildren<Animator>();
	}

	void Start() {
		// Subscribes itself to thepublisher
		GetComponent<JumpMushroom>().AddListener(this);
	}

	public void OnBounce(JumpMushroom mushroom, GameObject bouncingCharacter, Vector3 bounceVelocity, Vector3 collisionPoint, Vector3 collisionNormal) {
		if (_animator != null)
			_animator.SetTrigger(ANIMATOR_TRIGGER_STRING);
	}
}
