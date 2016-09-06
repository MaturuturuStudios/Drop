using UnityEngine;

/// <summary>
/// Base class for help information items.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class ShootUnableIndicator : MonoBehaviour {

	/// <summary>
	/// Amount of time the element will be shown.
	/// </summary>
	public float timeToHide = 1.5f;

	/// <summary>
	/// Reference to the character's Animator component.
	/// </summary>
	public Animator characterAnimator;

	/// <summary>
	/// Reference to the object's Animator component.
	/// </summary>
	private Animator _animator;

	/// <summary>
	/// Reference to the object's AudioSource component.
	/// </summary>
	private AudioSource _audioSource;

	/// <summary>
	/// Reference to this entity's Transform component.
	/// </summary>
	private Transform _transform;

	/// <summary>
	/// Flag which indicates if the element is currently shown.
	/// </summary>
	private bool _shown;

	/// <summary>
	/// Timer for the element to be hidden.
	/// </summary>
	private float _hideTimer;

	/// <summary>
	/// Unity's method called right after the object
	/// is created.
	/// </summary>
	void Awake() {
		_transform = transform;
		_animator = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
		_shown = false;
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Checks if the element should be hidden
		_hideTimer -= Time.deltaTime;
		if (_shown && _hideTimer <= 0)
			Hide();
	}

    /// <summary>
    /// Unity's method called at the end of each frame.
    /// </summary>
	void LateUpdate() {
		// Orientates the object to the camera
		_transform.rotation = Camera.main.transform.rotation;
    }

    /// <summary>
    /// Shows the information item.
    /// </summary>
	public void Show() {
		// Plays the effect animation
        _animator.SetTrigger("show");

		// Sets the flag and starts the timer
		_shown = true;
		_hideTimer = timeToHide;

		// Plays the sound
		_audioSource.Play();

		// Plays the animation on the character
		characterAnimator.SetTrigger(CharacterAnimatorParameters.ShootUnable);
	}

    /// <summary>
    /// Hides the information item.
    /// </summary>
    public void Hide() {
		// Checks if the object has been destroyed
		if (this == null || !gameObject.activeInHierarchy)
			return;

		// Plays the effect animation
		_animator.SetTrigger("hide");

		// Sets the flag and stops the timer
		_shown = false;
		_hideTimer = -1;
	}
}
