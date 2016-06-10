using UnityEngine;

/// <summary>
/// Controls the animator and maps the right
/// parameters.
/// </summary>
[RequireComponent(typeof(Animator))]
public class CharacterAnimatorController : MonoBehaviour, CharacterControllerListener {
	
	/// <summary>
	/// A reference to the CharacterControllerCustom on
	/// this entity's parent.
	/// </summary>
	private CharacterControllerCustom _ccc;

	/// <summary>
	/// A reference to the CharacterSize script of the entity.
	/// </summary>
	private CharacterSize _characterSize;

	/// <summary>
	/// A reference to the animator of the object (and the entire hierarchy).
	/// </summary>
	private Animator _animator;

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_ccc = GetComponentInParent<CharacterControllerCustom>();
		_characterSize = GetComponentInParent<CharacterSize>();
		_animator = GetComponent<Animator>();
	}

	/// <summary>
	/// Unity's method called at the beginning of the first
	/// frame this object is active.
	/// </summary>
	void Start() {
		// Subscribes itself to the controller events
		_ccc.AddListener(this);
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Updates the size
		float sqrtSize = Mathf.Sqrt(_characterSize.GetSize());
		_animator.SetFloat(CharacterAnimatorParameters.SizeFactor, 1.0f / sqrtSize);

		// Updates the speed
		float normalizedSpeed = Mathf.Abs(_ccc.Velocity.x / (_ccc.Parameters.maxSpeed * sqrtSize));
		_animator.SetFloat(CharacterAnimatorParameters.Speed, normalizedSpeed);

		// Updates the falling speed
		float fallingSpeed = Mathf.Abs(_ccc.Velocity.y);
		_animator.SetFloat(CharacterAnimatorParameters.FallingSpeed, fallingSpeed);

		// Updates the grounded state
		_animator.SetBool(CharacterAnimatorParameters.Grounded, _ccc.State.IsGrounded);
	}

	public void OnPreCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		// Updates the collision speed
		float collisionSpeed = -_ccc.Velocity.y;
		_animator.SetFloat(CharacterAnimatorParameters.CollisionSpeed, collisionSpeed);
	}

	public void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		// Do nothing
	}

	public void OnBeginJump(CharacterControllerCustom ccc, float delay) {
		// Sets the jump animation's delay on the animator
		_animator.SetFloat(CharacterAnimatorParameters.JumpDelay, 1.0f / delay);

		// Sets the jump trigger on the animator
		_animator.SetTrigger(CharacterAnimatorParameters.BeginJump);
	}

	public void OnPerformJump(CharacterControllerCustom ccc) {
		// Sets the jump trigger on the animator
		_animator.SetTrigger(CharacterAnimatorParameters.PerformJump);
	}

	public void OnWallJump(CharacterControllerCustom ccc) {
		// Do nothing
	}
}