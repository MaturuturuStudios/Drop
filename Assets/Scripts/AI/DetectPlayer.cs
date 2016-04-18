using UnityEngine;

using System.Collections.Generic;
using System.Collections;

public class DetectPlayer : StateMachineBehaviour {

    /// <summary>
    /// Independent control to create or remove drops
    /// </summary>
    private GameControllerIndependentControl _independentControl;

    Collider triggerArea;
    

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController)
                                .GetComponent<GameControllerIndependentControl>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
       
        
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }


    //public void OnTriggerEnter(Collider ownCollider, Collider other) {
    //    if (other.tag == Tags.Player && _independentControl.IsUnderControl(other.gameObject)) {
    //        Debug.Log("Ajaaa!!");
    //    }
    //}
}
