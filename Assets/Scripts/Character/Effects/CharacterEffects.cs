using UnityEngine;

[RequireComponent(typeof(CharacterControllerCustom))]
[RequireComponent(typeof(CharacterSize))]
[RequireComponent(typeof(CharacterShoot))]
[RequireComponent(typeof(CharacterFusion))]
public class CharacterEffects : MonoBehaviour, CharacterShootListener, CharacterFusionListener, CharacterControllerListener, IrrigateListener, EnemyBehaviourListener {

	public MinSpeedEffectInformation land;

	public EffectInformation jump;

	public EffectInformation shoot;

	public EffectInformation fuse;

	public EffectInformation slide;

	public EffectInformation wallJump;

	public EffectInformation irrigate;

	public EffectInformation hit;

	public LayerMask sceneMask;

	private CharacterControllerCustom _ccc;

	private CharacterSize _characterSize;

	private CharacterFusion _characterFusion;

	private CharacterShoot _characterShoot;

	private CharacterController _controller;

	private GameObject _slideEffect;

	private static readonly float SLIDE_EFFECT_DURATION = 2.0f;

	void Awake() {
        _ccc = GetComponent<CharacterControllerCustom>();
        _characterSize = GetComponent<CharacterSize>();
		_characterFusion = GetComponent<CharacterFusion>();
		_characterShoot = GetComponent<CharacterShoot>();
		_controller = GetComponent<CharacterController>();
    }

    void Start() {
        _ccc.AddListener(this);
		_characterFusion.AddListener(this);
		_characterShoot.AddListener(this);
	}

	void Update() {
		if (_ccc.State.IsSliding) {
			if (_slideEffect == null)
				_slideEffect = slide.PlayEffect(transform.position, Quaternion.identity, _characterSize.GetSize());
		}
		else if (_slideEffect != null) {
			foreach (ParticleSystem system in _slideEffect.GetComponentsInChildren<ParticleSystem>()) {
				ParticleSystem.EmissionModule emission = system.emission;
				emission.enabled = false;
			}
			Destroy(_slideEffect, SLIDE_EFFECT_DURATION);
			_slideEffect = null;
		}
	}

    public void OnBeginJump(CharacterControllerCustom ccc, float delay) {
        // Do nothing
    }

    public void OnPerformJump(CharacterControllerCustom ccc) {
		RaycastHit hit;
		if (Physics.SphereCast(ccc.transform.position, _controller.radius * _characterSize.GetSize(), ccc.Parameters.Gravity, out hit, 10, sceneMask)) {
			jump.PlayEffect(hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal), _characterSize.GetSize());
		}
	}

    public void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		if (hit.collider.CompareTag(Tags.Player))
			return;

		float minLandSpeed = land.GetMinSpeed(_characterSize.GetSize());
		Quaternion normalRotation = Quaternion.LookRotation(Vector3.forward, hit.normal);
        if (Vector3.Project(ccc.BeforeCollisionVelocity, hit.normal).sqrMagnitude > minLandSpeed) {
            land.PlayEffect(hit.point, normalRotation, _characterSize.GetSize());
        }

		if (ccc.State.IsSliding && _slideEffect != null) {
			_slideEffect.transform.position = hit.point;
			_slideEffect.transform.rotation = normalRotation;
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
		Transform characterTransform = shotCharacter.transform;
		int size = shootingCharacter.GetComponent<CharacterSize>().GetSize() + shotCharacter.GetComponent<CharacterSize>().GetSize();
        float radius = _controller.radius * size;
		shoot.PlayEffect(characterTransform.position + radius * velocity.normalized, Quaternion.LookRotation(Vector3.forward, velocity), size);
	}

	public void OnIrrigate(Irrigate irrigated, GameObject irrigating, int dropsConsumed) {
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
