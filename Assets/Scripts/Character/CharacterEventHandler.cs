using UnityEngine;

/// <summary>
/// Simple listener class which sends the events to the
/// character's subobjects' scripts.
/// </summary>
public class CharacterEventHandler : MonoBehaviour, EnemyBehaviourListener {

	public void OnAttack(AIBase enemy, GameObject attackedObject, Vector3 velocity) {
		foreach (EnemyBehaviourListener listener in GetComponentsInChildren<EnemyBehaviourListener>())
			if (listener != this)
				listener.OnAttack(enemy, attackedObject, velocity);
	}

	public void OnBeginChase(AIBase enemy, GameObject chasedObject) {
		foreach (EnemyBehaviourListener listener in GetComponentsInChildren<EnemyBehaviourListener>())
			if (listener != this)
				listener.OnBeginChase(enemy, chasedObject);
	}

	public void OnBeingScared(AIBase enemy, GameObject scaringObject, int scaringSize) {
		foreach (EnemyBehaviourListener listener in GetComponentsInChildren<EnemyBehaviourListener>())
			if (listener != this)
				listener.OnBeingScared(enemy, scaringObject, scaringSize);
	}

	public void OnEndChase(AIBase enemy, GameObject chasedObject) {
		foreach (EnemyBehaviourListener listener in GetComponentsInChildren<EnemyBehaviourListener>())
			if (listener != this)
				listener.OnEndChase(enemy, chasedObject);
	}

	public void OnStateAnimationChange(AnimationState previousState, AnimationState actualState) {
		foreach (EnemyBehaviourListener listener in GetComponentsInChildren<EnemyBehaviourListener>())
			if (listener != this)
				listener.OnStateAnimationChange(previousState, actualState);
	}
}
