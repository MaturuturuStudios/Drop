using UnityEngine;

using System.Collections.Generic;
using System.Collections;

public class DetectPlayer : StateMachineBehaviour {
    /// <summary>
    /// Time to stay in detect state
    /// </summary>
    [HideInInspector]
    public float timeWarning=0;
    /// <summary>
    /// Timer
    /// </summary>
    private float deltaTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        deltaTime = timeWarning;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Timer", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        deltaTime -= Time.deltaTime;
        if (deltaTime <= 0) {
            animator.SetBool("Timer", true);
        }
    }
}
