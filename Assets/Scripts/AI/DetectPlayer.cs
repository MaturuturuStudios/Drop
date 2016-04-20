using UnityEngine;

using System.Collections.Generic;
using System.Collections;

public class DetectPlayer : StateMachineBehaviour {
    [HideInInspector]
    public float timeWarning=0;
    /// <summary>
    /// Timer
    /// </summary>
    private float deltaTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("Detected");
        deltaTime = timeWarning;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("Stop detected");
        animator.SetBool("Timer", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        deltaTime -= Time.deltaTime;

        if (deltaTime <= 0) {
            animator.SetBool("Timer", true);
        }
    }
}
