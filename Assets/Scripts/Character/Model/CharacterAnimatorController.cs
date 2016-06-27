using UnityEngine;

/// <summary>
/// Controls the animator and maps the right
/// parameters.
/// </summary>
[RequireComponent(typeof(Animator))]
public class CharacterAnimatorController : MonoBehaviour, CharacterControllerListener, CharacterFusionListener, CharacterShootListener {
	
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
	/// A reference to the CharacterFusion script of the entity.
	/// </summary>
	private CharacterFusion _characterFusion;

	/// <summary>
	/// A reference to the CharacterShoot script of the entity.
	/// </summary>
	private CharacterShoot _characterShoot;

	/// <summary>
	/// A reference to the animator of the object (and the entire hierarchy).
	/// </summary>
	private Animator _animator;

	/// <summary>
	/// A reference to the game controller's GameControllerIndependentControl
	/// component.
	/// </summary>
	private GameControllerIndependentControl _gcic;

	/// <summary>
	/// Reference to this object's parent object, the main object.
	/// </summary>
	private GameObject _drop;

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_ccc = GetComponentInParent<CharacterControllerCustom>();
		_characterSize = GetComponentInParent<CharacterSize>();
		_characterFusion = GetComponentInParent<CharacterFusion>();
		_characterShoot = GetComponentInParent<CharacterShoot>(); 
		_animator = GetComponent<Animator>();
		_gcic = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();
		_drop = transform.parent.gameObject;
	}

	/// <summary>
	/// Unity's method called at the beginning of the first
	/// frame this object is active.
	/// </summary>
	void Start() {
		// Subscribes itself to the controller events
		_ccc.AddListener(this);
		_characterFusion.AddListener(this);
		_characterShoot.AddListener(this);
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

		// Updates the sliding state
		_animator.SetBool(CharacterAnimatorParameters.Sliding, _ccc.State.IsSliding);

		// Updates the shoot mode state
		_animator.SetBool(CharacterAnimatorParameters.ShootMode, _characterShoot.shootmode);

		// Updates the under control state
		_animator.SetBool(CharacterAnimatorParameters.Controlled, _gcic.IsUnderControl(_drop));
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
		// Sets the wall jump trigger on the animator
		_animator.SetTrigger(CharacterAnimatorParameters.WallJump);
	}

	public void OnBeginFusion(CharacterFusion originalCharacter, GameObject fusingCharacter, ControllerColliderHit hit) {
		// Sets the fusion trigger on the animator
		_animator.SetTrigger(CharacterAnimatorParameters.Fusion);
	}

	public void OnEndFusion(CharacterFusion finalCharacter) {
		// Do nothing
	}

	public void OnEnterShootMode(CharacterShoot character) {
		// Do nothing (mapped on update)
	}

	public void OnExitShootMode(CharacterShoot character) {
		// Do nothing (mapped on update)
	}

	public void OnShoot(CharacterShoot shootingCharacter, GameObject shotCharacter, Vector3 velocity) {
		// Sets the shoot trigger on the animator
		_animator.SetTrigger(CharacterAnimatorParameters.Shoot);
	}
}