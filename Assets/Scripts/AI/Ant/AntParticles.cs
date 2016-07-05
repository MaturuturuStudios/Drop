using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AIAnt))]
public class AntParticles : MonoBehaviour, EnemyBehaviourListener {
    /// <summary>
    /// Particle system of walking effect
    /// </summary>
    public GameObject walkingFX;

    /// <summary>
    /// Check if the particles was set
    /// </summary>
    private bool settedFX = true;

    /// <summary>
	/// Reference to the ant's AI component.
	/// </summary>
	private AIAnt _ai;

    void Awake() {
        // Retrieves the desired components
        _ai = GetComponent<AIAnt>();
    }

    void Start() {
        // Subscribes itself to the AI events
        _ai.AddListener(this);
    }

    public void OnStateAnimationChange(AnimationState previousState, AnimationState actualState) {
        bool change = false;
        switch (actualState) {
            case AnimationState.WALK:
            case AnimationState.CHASE:
            case AnimationState.RUN_AWAY:
                if (settedFX) break;
                settedFX = true;
                change = true;
                break;

            default:
                if (!settedFX) break;
                settedFX = false;
                change = true;
                break;
        }

        if (change) {
            foreach (ParticleSystem system in walkingFX.GetComponentsInChildren<ParticleSystem>()) {
                ParticleSystem.EmissionModule emission = system.emission;                             
                emission.enabled = settedFX;
            }
        }
    }

    public void OnBeginChase(AIBase enemy, GameObject chasedObject) {
    }

    public void OnEndChase(AIBase enemy, GameObject chasedObject) {
    }

    public void OnAttack(AIBase enemy, GameObject attackedObject, Vector3 velocity) {
    }

    public void OnBeingScared(AIBase enemy, GameObject scaringObject, int scaringSize) {
    }
}
