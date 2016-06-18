using UnityEngine;

[System.Serializable]
public class DetectParameters{
    /// <summary>
    /// Time to stay in detect state
    /// before chasing player
    /// </summary>
    public float timeWarningDetect = 0;
    /// <summary>
    /// if true, the axis will be fixed at rotation
    /// </summary>
    public AxisBoolean fixedRotation;
}

public class DetectPlayer : StateMachineBehaviour, CollisionListener {
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


    private Animator _animator;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //start timer
        _animator = animator;
        commonParameters.colliders.AddListener(this);
        _deltaTime = parameters.timeWarningDetect;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
        animator.SetBool("Near", false);
        commonParameters.colliders.RemoveListener(this);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
        _deltaTime -= Time.deltaTime;
        if (_deltaTime <= 0) {
            animator.SetBool("Timer", true);
        }

        if (commonParameters.drop == null) return;

        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = commonParameters.sizeLimitDrop;
        if (sizeLimit > 0 && size >= sizeLimit) {
            animator.SetBool("GoAway", true);
        }

        //face target
        Vector3 originalPosition = commonParameters.enemy.transform.position;
        Vector3 finalPosition = commonParameters.drop.transform.position;
        AIMethods.RotateEnemySlerp(commonParameters.enemy, parameters.fixedRotation, commonParameters.initialRotationEnemy, 
                                commonParameters.RotationSpeed, originalPosition, finalPosition);
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == Tags.Player) {
            if (_animator == null) return;
            _animator.SetBool("Reached", true);
        }
    }

    public void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == Tags.Player) {
            if (_animator == null) return;
            _animator.SetBool("Reached", true);
        }

    }
    #endregion
}
