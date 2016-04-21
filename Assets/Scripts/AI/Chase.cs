using UnityEngine;

[System.Serializable]
public struct ChaseParameters {
    /// <summary>
    /// The chasing speed
    /// </summary>
    public float speed;
}

public class Chase : StateMachineBehaviour {
    #region Attributes
    [HideInInspector]
    /// <summary>
    /// Parameters of the chase script
    /// </summary>
    public ChaseParameters parameters;
    [HideInInspector]
    public CommonParameters commonParameters;
    /// <summary>
    /// A reference to the entity's transform.
    /// </summary>
    private Transform _transform;
    #endregion

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _transform = commonParameters.enemy.transform;
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
            return;
        }

        if (commonParameters.drop == null) {
            return;
        }
        //move towards target
        _transform.position = Vector3.MoveTowards(_transform.position, commonParameters.drop.transform.position, parameters.speed * Time.deltaTime);

        //always face it
        faceTarget();

        //reached calculation is on specific IA
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

        //rotate us over time according to speed until we are in the required rotation
        enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation, _lookRotation, Time.deltaTime * commonParameters.RotationSpeed);
    }

}
