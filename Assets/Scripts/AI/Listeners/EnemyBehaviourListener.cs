using UnityEngine;

public enum AnimationState {
    IDDLE,
    WALK,
    CHASE,
    DETECT,
    ATTACK,
	SCARED,
    RUN_AWAY,
    HIDDE_RECOLECT
}

/// <summary>
/// Interface for the observers listening for the
/// enemies' actions' events.
/// </summary>
public interface EnemyBehaviourListener {

	/// <summary>
	/// Event called when the enemy begins chasing a character.
	/// </summary>
	/// <param name="enemy">The enemy</param>
	/// <param name="chasedObject">The chased character</param>
	void OnBeginChase(AIBase enemy, GameObject chasedObject);

	/// <summary>
	/// Event called when the enemy ends chasing a character.
	/// </summary>
	/// <param name="enemy">The enemy</param>
	/// <param name="chasedObject">The chased character</param>
	void OnEndChase(AIBase enemy, GameObject chasedObject);

	/// <summary>
	/// Event called when the enemy attacks a character.
	/// </summary>
	/// <param name="enemy">The enemy</param>
	/// <param name="attackedObject">The chased character</param>
	/// <param name="velocity">The velocity the character is hit with</param>
	void OnAttack(AIBase enemy, GameObject attackedObject, Vector3 velocity);

	/// <summary>
	/// Event called when the enemy is scared by a character.
	/// </summary>
	/// <param name="enemy">The enemy</param>
	/// <param name="scaringObject">The scaring character</param>
	/// <param name="scaringSize">The size for the enemy to be scared</param>
	void OnBeingScared(AIBase enemy, GameObject scaringObject, int scaringSize);

    /// <summary>
    /// Event called on every change of state
    /// </summary>
    /// <param name="previousState"></param>
    /// <param name="actualState"></param>
    void OnStateAnimationChange(AnimationState previousState, AnimationState actualState);
}
