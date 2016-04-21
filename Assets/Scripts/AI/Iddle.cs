using UnityEngine;
using System.Collections;

[System.Serializable]
public class IddleParameters {
    /// <summary>
    /// Time the entity will be in iddle state
    /// </summary>
    public float timeInIddle=0;
}

public class Iddle : StateMachineBehaviour {
    #region Attributes
    /// <summary>
    /// Parameters
    /// </summary>
    [HideInInspector]
    public IddleParameters parameters;
    /// <summary>
    /// Timer
    /// </summary>
    private float _deltaTime;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //start timer
        _deltaTime = parameters.timeInIddle;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
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
        } else if ((sizeLimit <= 0 || size < sizeLimit) && size > 0) {
            animator.SetBool("Detect", true);
        }

        _deltaTime -= Time.deltaTime;
        if (_deltaTime <= 0) {
            animator.SetBool("Timer", true);
        }
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
    #endregion
}
