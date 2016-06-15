using System;
using UnityEngine;

[RequireComponent(typeof(CharacterControllerCustom))]
[RequireComponent(typeof(CharacterSize))]
public class CharacterEffects : MonoBehaviour, CharacterControllerListener {

    [Header("Land")]
    public GameObject landEffectPrefab;

    public float landEffectDuration = 1.5f;

    public float landMinSpeed = 3.0f;

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
        if (Vector3.Project(ccc.BeforeCollisionVelocity, hit.normal).sqrMagnitude > landMinSpeed * landMinSpeed) {
            GameObject effect = Instantiate(landEffectPrefab, hit.point, Quaternion.LookRotation(Vector3.forward, hit.normal)) as GameObject;
            effect.transform.localScale = Vector3.one * _characterSize.GetSize();
            Destroy(effect, landEffectDuration);
        }
    }

    public void OnPreCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
        // Do nothing
    }

    public void OnWallJump(CharacterControllerCustom ccc) {
        // Do nothing
    }
}
