using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Listener class which plays the multiple effects produced
/// by the character.
/// </summary>
[RequireComponent(typeof(CharacterControllerCustom))]
[RequireComponent(typeof(CharacterSize))]
[RequireComponent(typeof(CharacterShoot))]
[RequireComponent(typeof(CharacterFusion))]
public class CharacterEffects : MonoBehaviour, CharacterShootListener, CharacterFusionListener, CharacterControllerListener, IrrigateListener, EnemyBehaviourListener {

	/// <summary>
	/// Effect played while walking.
	/// </summary>
	public MinSpeedEffectInformation walk;

	/// <summary>
	/// Effect played when landing.
	/// </summary>
	public MinSpeedEffectInformation land;

	/// <summary>
	/// Effect played when jumping.
	/// </summary>
	public EffectInformation jump;

	/// <summary>
	/// Effect played when shooting.
	/// </summary>
	public EffectInformation shoot;

	/// <summary>
	/// Effect played when fusing.
	/// </summary>
	public EffectInformation fuse;

	/// <summary>
	/// Effect played while sliding.
	/// </summary>
	public EffectInformation slide;

	/// <summary>
	/// Effect played when wall jumping.
	/// </summary>
	public EffectInformation wallJump;

	/// <summary>
	/// Effect played when irrigating.
	/// </summary>
	public EffectInformation irrigate;

	/// <summary>
	/// Effect played when being hit.
	/// </summary>
	public EffectInformation hit;

	/// <summary>
	/// Mask for any collision point check.
	/// </summary>
	public LayerMask sceneMask;

	/// <summary>
	/// Reference to this entity's CharacterControllerCustom component.
	/// </summary>
	private CharacterControllerCustom _ccc;

	/// <summary>
	/// Reference to this entity's CharacterSize component.
	/// </summary>
	private CharacterSize _characterSize;

	/// <summary>
	/// Reference to this entity's CharacterFusion component.
	/// </summary>
	private CharacterFusion _characterFusion;

	/// <summary>
	/// Reference to this entity's CharacterShoot component.
	/// </summary>
	private CharacterShoot _characterShoot;

	/// <summary>
	/// Reference to this entity's CharacterController component.
	/// </summary>
	private CharacterController _controller;

	/// <summary>
	/// The walking effect used by this script.
	/// </summary>
	private Transform _walkEffect;

	/// <summary>
	/// Reference to the walking effect's particle systems.
	/// </summary>
	private Dictionary<ParticleSystem, float> _walkParticleEffects;

	/// <summary>
	/// The sliding effect used by this script.
	/// </summary>
	private Transform _slideEffect;

	/// <summary>
	/// Reference to the sliding effect's particle systems.
	/// </summary>
	private Dictionary<ParticleSystem, float> _slideParticleEffects;

	void Awake() {
		// Retrieves the desired components
        _ccc = GetComponent<CharacterControllerCustom>();
        _characterSize = GetComponent<CharacterSize>();
		_characterFusion = GetComponent<CharacterFusion>();
		_characterShoot = GetComponent<CharacterShoot>();
		_controller = GetComponent<CharacterController>();
	}

    void Start() {
		// Subscribes itself to the publishers
        _ccc.AddListener(this);
		_characterFusion.AddListener(this);
		_characterShoot.AddListener(this);

		// Creates and stops the walking effect
		_walkEffect = walk.PlayEffect(transform.position, Quaternion.identity).transform;
		_walkParticleEffects = new Dictionary<ParticleSystem, float>();
		foreach (ParticleSystem system in _walkEffect.GetComponentsInChildren<ParticleSystem>()) {
			_walkParticleEffects.Add(system, system.startSize);
            ParticleSystem.EmissionModule emission = system.emission;
			emission.enabled = false;
		}

		// Creates and stops the sliding effect
		_slideEffect = slide.PlayEffect(transform.position, Quaternion.identity).transform;
		_slideParticleEffects = new Dictionary<ParticleSystem, float>();
        foreach (ParticleSystem system in _slideEffect.GetComponentsInChildren<ParticleSystem>()) {
			_slideParticleEffects.Add(system, system.startSize);
			ParticleSystem.EmissionModule emission = system.emission;
			emission.enabled = false;
		}
	}

	void Update() {
		// Plays or stops the walking effect
		foreach (ParticleSystem system in _walkParticleEffects.Keys) {
			ParticleSystem.EmissionModule emission = system.emission;
			emission.enabled = _ccc.State.IsGrounded && Mathf.Abs(_ccc.GetNormalizedSpeed()) >= walk.GetMinSpeed(_characterSize.GetSize());
		}

		// Plays or stops the sliding effect
		foreach (ParticleSystem system in _slideParticleEffects.Keys) {
			ParticleSystem.EmissionModule emission = system.emission;
			emission.enabled = _ccc.State.IsSliding;
		}
	}

    public void OnBeginJump(CharacterControllerCustom ccc, float delay) {
        // Do nothing
    }

    public void OnPerformJump(CharacterControllerCustom ccc) {
		// Looks for the ground and plays the jump effect
		RaycastHit hit;
		if (Physics.SphereCast(ccc.transform.position, _controller.radius * _characterSize.GetSize(), ccc.Parameters.Gravity, out hit, 10, sceneMask))
			jump.PlayEffect(hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal), _characterSize.GetSize());
	}

    public void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		if (hit.collider.CompareTag(Tags.Player))
			return;

		// Plays the landing effect
		float minLandSpeed = land.GetMinSpeed(_characterSize.GetSize());
		Quaternion normalRotation = Quaternion.LookRotation(Vector3.forward, hit.normal);
        if (Vector3.Project(ccc.BeforeCollisionVelocity, hit.normal).sqrMagnitude > minLandSpeed)
            land.PlayEffect(hit.point, normalRotation, _characterSize.GetSize());

		// Calculates some values for the particles
		Vector3 eulerRotation = new Vector3(Vector3.Angle(Vector3.left, hit.normal) * Mathf.Deg2Rad, Mathf.PI / 2, 0);
		float sizeFactor = _characterSize.GetSize();

		// Positions the walking effect
		if (ccc.State.IsGrounded) {
			_walkEffect.position = hit.point;
			_walkEffect.rotation = normalRotation;
			foreach (KeyValuePair<ParticleSystem, float> system in _walkParticleEffects) {
				system.Key.startSize = system.Value * sizeFactor;
                system.Key.startRotation3D = eulerRotation;
			}
		}

		// Positions the sliding effect
		if (ccc.State.IsSliding) {
			_slideEffect.position = hit.point;
			_slideEffect.rotation = normalRotation;
			foreach (KeyValuePair<ParticleSystem, float> system in _slideParticleEffects) {
				system.Key.startSize = system.Value * sizeFactor;
				system.Key.startRotation3D = eulerRotation;
			}
		}
	}

    public void OnPreCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
        // Do nothing
    }

    public void OnWallJump(CharacterControllerCustom ccc) {
		// Since to wall jump the character needs to be sliding, uses the slide effect position
		if (_slideEffect != null)
			wallJump.PlayEffect(_slideEffect.transform.position, _slideEffect.transform.rotation, _characterSize.GetSize());
    }

	public void OnBeginFusion(CharacterFusion originalCharacter, GameObject fusingCharacter, ControllerColliderHit hit) {
		// Do nothing
	}

	public void OnEndFusion(CharacterFusion finalCharacter) {
		// Plays the fusion effect
		Transform characterTransform = finalCharacter.transform;
		fuse.PlayEffect(characterTransform.position, characterTransform.rotation, finalCharacter.GetSize());
    }

	public void OnEnterShootMode(CharacterShoot character) {
		// Do nothing
	}

	public void OnExitShootMode(CharacterShoot character) {
		// Do nothing
	}

	public void OnShoot(CharacterShoot shootingCharacter, GameObject shotCharacter, Vector3 velocity) {
		// Plays the shooting effect
		Transform characterTransform = shotCharacter.transform;
		int size = shootingCharacter.GetComponent<CharacterSize>().GetSize() + shotCharacter.GetComponent<CharacterSize>().GetSize();
        float radius = _controller.radius * size;
		shoot.PlayEffect(characterTransform.position + radius * velocity.normalized, Quaternion.LookRotation(Vector3.forward, velocity), size);
	}

	public void OnIrrigate(Irrigate irrigated, GameObject irrigating, int dropsConsumed) {
		// Plays the irrigation effect
		Transform characterTransform = irrigating.transform;
		irrigate.PlayEffect(characterTransform.position, characterTransform.rotation, _characterSize.GetSize());
	}

	public void OnBeginChase(AIBase enemy, GameObject chasedObject) {
		// Do nothing
	}

	public void OnEndChase(AIBase enemy, GameObject chasedObject) {
		// Do nothing
	}

	public void OnAttack(AIBase enemy, GameObject attackedObject, Vector3 velocity) {
		// Plays the hit effect
		if (attackedObject == gameObject) {
			Transform characterTransform = attackedObject.transform;
			hit.PlayEffect(characterTransform.position, characterTransform.rotation, _characterSize.GetSize());
		}
	}

	public void OnBeingScared(AIBase enemy, GameObject scaringObject, int scaringSize) {
		// Do nothing
	}

	public void OnStateAnimationChange(AnimationState previousState, AnimationState actualState) {
		// Do nothing
	}
}
