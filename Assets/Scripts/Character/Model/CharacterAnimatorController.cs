using UnityEngine;

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

	void Awake() {
		// Retrieves the desired components
		_ccc = GetComponentInParent<CharacterControllerCustom>();
		_characterSize = GetComponentInParent<CharacterSize>();
		_animator = GetComponent<Animator>();
	}

	void Start() {
		// Subscribes itself to the controller events
		_ccc.AddListener(this);
	}

	void Update() {
		// Updates the size
		float sqrtSize = Mathf.Sqrt(_characterSize.GetSize());
		_animator.SetFloat(CharacterAnimatorParameters.SizeFactor, 1.0f / sqrtSize);

		// Updates the speed
		float normalizedSpeed = _ccc.Velocity.x / (_ccc.Parameters.maxSpeed * sqrtSize);
		_animator.SetFloat(CharacterAnimatorParameters.Speed, normalizedSpeed);

		// Updates the falling speed
		float fallingSpeed = -_ccc.Velocity.y;
		_animator.SetFloat(CharacterAnimatorParameters.FallingSpeed, fallingSpeed);

		// Updates the grounded state
		_animator.SetBool(CharacterAnimatorParameters.Grounded, _ccc.State.IsGrounded);
	}

	public void OnJump(CharacterControllerCustom ccc) {
		// Sets the jump trigger on the animator
		_animator.SetTrigger(CharacterAnimatorParameters.Jump);
	}

	public void OnPreCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		// Updates the collision speed
		float collisionSpeed = -_ccc.Velocity.y;
		_animator.SetFloat(CharacterAnimatorParameters.CollisionSpeed, collisionSpeed);
	}

	public void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		// Do nothing
	}
}