using UnityEngine;

[System.Serializable]
public class DetectParameters{
    /// <summary>
    /// Time to stay in detect state
    /// </summary>
    public float timeWarningDetect = 0;
}

public class DetectPlayer : StateMachineBehaviour {
    #region Attributes
    [HideInInspector]
    ///<summary>
    /// Parameters of the script
    ///</summary>
    public DetectParameters parameters;
    /// <summary>
    /// Timer
    /// </summary>
    private float _deltaTime;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //start timer
        _deltaTime = parameters.timeWarningDetect;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //check if have condition to change state
        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = animator.GetInteger("LimitSizeDrop");
        if (sizeLimit > 0 && size >= sizeLimit) {
            animator.SetBool("GoAway", true);
        }

        //TODO: always face to the target

        _deltaTime -= Time.deltaTime;
        if (_deltaTime <= 0) {
            animator.SetBool("Timer", true);
        }
    }
    #endregion
}
