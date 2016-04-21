using UnityEngine;

public class Attack : StateMachineBehaviour {
    [HideInInspector]
    public CommonParameters commonParameters;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //OnStateEnter is called on the first frame of the state being played.
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //OnStateExit is called on the last frame of a transition to another state.
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //get the drop and do some action

        //OnStateUpdate is called after MonoBehaviour Updates on every frame whilst the animator is playing the state this behaviour belongs to.
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //OnStateMove is called before OnAnimatorMove would be called in MonoBehaviours for every frame the state is playing.
        //When OnStateMove is called, it will stop OnAnimatorMove from being called in MonoBehaviours.
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //OnStateIK is called after OnAnimatorIK on MonoBehaviours for every frame the while the state is being played.
        //It is important to note that OnStateIK will only be called if the state is on a layer that has an IK pass. 
        //By default, layers do not have an IK pass and so this function will not be called. 
        //For more information on IK see the information linked below.
    }
}
