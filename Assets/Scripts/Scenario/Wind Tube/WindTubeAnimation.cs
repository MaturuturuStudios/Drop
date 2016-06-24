using UnityEngine;

/// <summary>
/// Listener to the wind tube's events that plays an animation on the plant.
/// </summary>
[RequireComponent(typeof(WindTube))]
public class WindTubeAnimation : MonoBehaviour {

	/// <summary>
	/// Sets de velocity for the played animations.
	/// </summary>
	public float animationSpeed = 1.0f;

	/// <summary>
	/// Reference to the affected animators.
	/// </summary>
	private Animator[] _animators;

	/// <summary>
	/// Reference to the entity's WindTube component.
	/// </summary>
	private WindTube _windTube;

	/// <summary>
	/// The key string to the active flag on the animator.
	/// </summary>
	private static readonly string ANIMATOR_ACTIVE_STRING = "active";

	/// <summary>
	/// The key string to the animation speed factor on the animator.
	/// </summary>
	private static readonly string ANIMATOR_SPEED_STRING = "animation_speed";

	void Awake() {
		// Retrieves the desired components
		_animators = GetComponentsInChildren<Animator>();
		_windTube = GetComponent<WindTube>();
    }

	void Update() {
		foreach (Animator animator in _animators) {
			animator.SetFloat(ANIMATOR_SPEED_STRING, animationSpeed);
			animator.SetBool(ANIMATOR_ACTIVE_STRING, _windTube.isActiveAndEnabled);
		}
	}
}
