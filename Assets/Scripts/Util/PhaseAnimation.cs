using UnityEngine;

/// <summary>
/// Applies a phase and modifies the speed of a special
/// animation. The animator must have an animation state
/// accessed with the trigger "randomize". This transition
/// is what will be phased.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PhaseAnimation : MonoBehaviour {

	/// <summary>
	/// Key used for the animator trigger.
	/// </summary>
	public static readonly string AnimatorKey = "randomize";

	/// <summary>
	/// Min phase applied to the animation.
	/// </summary>
	public float minPhase = 0;

	/// <summary>
	/// Max phase applied to the animation.
	/// </summary>
	public float maxPhase = 0;

	/// <summary>
	/// Min speed for the animation to play.
	/// </summary>
	public float minSpeed = 1;

	/// <summary>
	/// Max speed for the animation to play.
	/// </summary>
	public float maxSpeed = 1;

	/// <summary>
	/// Phase actually applied to the animation.
	/// </summary>
	public float Phase { get; set; }

	/// <summary>
	/// Speed actually applied to the animation.
	/// </summary>
	public float Speed { get; set; }

	/// <summary>
	/// Time elapsed since the animation started.
	/// Used only to phase the animation.
	/// </summary>
	private float _elapsedTime;

	/// <summary>
	/// Reference to the object's Animator component.
	/// </summary>
	private Animator _animator;

	void Start() {
		// Retrieves the desired components
		_animator = GetComponent<Animator>();

		// Generates the random values
		Phase = Random.Range(minPhase, maxPhase);
		Speed = Random.Range(minSpeed, maxSpeed);

		// Starts the timer
		_elapsedTime = 0;
	}

	void Update() {
		// Updates the timer
		_elapsedTime += Time.deltaTime;

		// Applies the phase to the animation
		if (_elapsedTime >= Phase) {
			// Triggers the animation and applies the speed
			_animator.SetTrigger(AnimatorKey);
			_animator.speed = Speed;

			// Disables itself (no longer necessary)
			enabled = false;
		}
	}
}
