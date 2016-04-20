using UnityEngine;
using System.Collections;

public class Iddle : StateMachineBehaviour {
    /// <summary>
    /// Time to stay in detect state
    /// </summary>
    [HideInInspector]
    public float timeInIddle = 0;
    /// <summary>
    /// Timer
    /// </summary>
    private float _deltaTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _deltaTime = timeInIddle;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //check if have condition to change state
        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = animator.GetInteger("LimitSizeDrop");
        if (sizeLimit > 0 && size >= sizeLimit) {
            animator.SetBool("GoAway", true);
        } else if (sizeLimit <= 0 || (size < sizeLimit && size > 0)) {
            animator.SetBool("Detect", true);
            return;
        }

        _deltaTime -= Time.deltaTime;
        if (_deltaTime <= 0) {
            animator.SetBool("Timer", true);
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
