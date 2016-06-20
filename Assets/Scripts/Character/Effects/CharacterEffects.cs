using System;
using UnityEngine;

[RequireComponent(typeof(CharacterControllerCustom))]
[RequireComponent(typeof(CharacterSize))]
[RequireComponent(typeof(CharacterShoot))]
[RequireComponent(typeof(CharacterFusion))]
public class CharacterEffects : MonoBehaviour, CharacterShootListener, CharacterFusionListener, CharacterControllerListener {

	public MinSpeedEffectInformation land;

	public EffectInformation grow;

	public EffectInformation shoot;

    private CharacterControllerCustom _ccc;

	private CharacterSize _characterSize;

	private CharacterFusion _characterFusion;

	private CharacterShoot _characterShoot;

	private CharacterController _controller;

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

    public void OnBeginJump(CharacterControllerCustom ccc, float delay) {
        // Do nothing
    }

    public void OnPerformJump(CharacterControllerCustom ccc) {
        // Do nothing
    }

    public void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		if (hit.collider.CompareTag(Tags.Player))
			return;
		float minLandSpeed = land.GetMinSpeed(_characterSize.GetSize());
        if (Vector3.Project(ccc.BeforeCollisionVelocity, hit.normal).sqrMagnitude > minLandSpeed) {
            land.PlayEffect(hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal), _characterSize.GetSize());
        }
    }

    public void OnPreCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
        // Do nothing
    }

    public void OnWallJump(CharacterControllerCustom ccc) {
        // Do nothing
    }

	public void OnBeginFusion(CharacterFusion originalCharacter, GameObject fusingCharacter, ControllerColliderHit hit) {
		// Do nothing
	}

	public void OnEndFusion(CharacterFusion finalCharacter) {
		Transform characterTransform = finalCharacter.transform;
		grow.PlayEffect(characterTransform.position, characterTransform.rotation, finalCharacter.GetSize());
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
}
