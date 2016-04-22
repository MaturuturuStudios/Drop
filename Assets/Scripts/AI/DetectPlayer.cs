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
    [HideInInspector]
    public CommonParameters commonParameters;
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
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = commonParameters.sizeLimitDrop;
        if (sizeLimit > 0 && size >= sizeLimit) {
            animator.SetBool("GoAway", true);
        }

        //always face target
        faceTarget();

        _deltaTime -= Time.deltaTime;
        if (_deltaTime <= 0) {
            animator.SetBool("Timer", true);
        }
    }

    private void faceTarget() {
        Quaternion _lookRotation;
        Vector3 _direction;
        Transform targetTransform = commonParameters.drop.transform;
        Transform enemyTransform = commonParameters.enemy.transform;

        //find the vector pointing from our position to the target
        _direction = (targetTransform.position - enemyTransform.position).normalized;

        if (_direction == Vector3.zero) return;
        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation(_direction);

        if (commonParameters.onFloor) {
            _lookRotation.x = 0;
            _lookRotation.z = 0;
        }
        //rotate us over time according to speed until we are in the required rotation
        enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation, _lookRotation, Time.deltaTime * commonParameters.RotationSpeed);
    }
    #endregion
}
