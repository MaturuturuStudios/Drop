using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Walking : StateMachineBehaviour {
    [HideInInspector]
    public GameObject enemy = null;
    [HideInInspector]
    public List<Vector3> path;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("Walking");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("Stop wlaking");
        animator.SetBool("ChangeState", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = animator.GetInteger("LimitSizeDrop");

        if (sizeLimit<=0 || (size < sizeLimit && size>0)) {
            Debug.Log("Changed state");
            animator.SetBool("ChangeState", true);
        }

        //set the moving path
       
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
