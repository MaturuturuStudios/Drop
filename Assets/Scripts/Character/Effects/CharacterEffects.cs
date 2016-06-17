using UnityEngine;

[RequireComponent(typeof(CharacterControllerCustom))]
[RequireComponent(typeof(CharacterSize))]
public class CharacterEffects : MonoBehaviour, CharacterControllerListener {

	public MinSpeedEffectInformation land;

    private CharacterControllerCustom _ccc;

    private CharacterSize _characterSize;

    void Awake() {
        _ccc = GetComponent<CharacterControllerCustom>();
        _characterSize = GetComponent<CharacterSize>();
    }

    void Start() {
        _ccc.AddListener(this);
    }

    public void OnBeginJump(CharacterControllerCustom ccc, float delay) {
        // Do nothing
    }

    public void OnPerformJump(CharacterControllerCustom ccc) {
        // Do nothing
    }

    public void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
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
}
