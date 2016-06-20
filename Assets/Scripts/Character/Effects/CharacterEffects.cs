using System;
using UnityEngine;

[RequireComponent(typeof(CharacterControllerCustom))]
[RequireComponent(typeof(CharacterSize))]
[RequireComponent(typeof(CharacterFusion))]
public class CharacterEffects : MonoBehaviour, CharacterFusionListener, CharacterControllerListener {

	public MinSpeedEffectInformation land;

	public EffectInformation grow;

    private CharacterControllerCustom _ccc;

	private CharacterSize _characterSize;

	private CharacterFusion _characterFusion;

	void Awake() {
        _ccc = GetComponent<CharacterControllerCustom>();
        _characterSize = GetComponent<CharacterSize>();
		_characterFusion = GetComponent<CharacterFusion>();
	}

    void Start() {
        _ccc.AddListener(this);
		_characterFusion.AddListener(this);
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
}
