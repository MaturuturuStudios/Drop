using UnityEngine;

public class Scared : StateMachineBehaviour {

	#region Public and hidden in inspector attributes

	[HideInInspector]
	public CommonParameters commonParameters;

	#endregion

	#region Methods

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// Resets the flag
        animator.SetBool("GoAway", false);

		// Notifies the listeners
		foreach (EnemyBehaviourListener listener in commonParameters.drop.GetComponents<EnemyBehaviourListener>())
			listener.OnBeingScared(commonParameters.AI, commonParameters.drop, commonParameters.sizeLimitDrop);
		foreach (EnemyBehaviourListener listener in commonParameters.AI.listeners)
			listener.OnBeingScared(commonParameters.AI, commonParameters.drop, commonParameters.sizeLimitDrop);
	}

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Resets all flags
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
        animator.SetBool("Near", false);
        animator.SetBool("Recolect", false);
    }  

    #endregion
}
