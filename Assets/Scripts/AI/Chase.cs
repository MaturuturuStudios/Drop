using UnityEngine;

[System.Serializable]
public class ChaseParameters {
    /// <summary>
    /// The chasing speed
    /// </summary>
    public float speed=10;
    /// <summary>
    /// 
    /// </summary>
    public float rotationVelocity=170;
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
    /// A reference to the entity's controller.
    /// </summary>
    private CharacterController _controller;
    /// <summary>
    /// Minimum distance to goal
    /// </summary>
    private float _minimumDistance;
    #endregion

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _controller = commonParameters.enemy.GetComponent<CharacterController>();
		_minimumDistance = commonParameters.toleranteDistanceAttack; //GetMinimumDistance() +
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
        animator.SetBool("Near", false);
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

        // Saves the original position
        Vector3 originalPosition = commonParameters.enemy.transform.position;
        Vector3 finalPosition = originalPosition;
        Vector3 target = commonParameters.drop.transform.position;

        //move towards target
        finalPosition = Vector3.MoveTowards(originalPosition, target, parameters.speed * Time.deltaTime);

        if (commonParameters.onFloor)
            finalPosition.y = originalPosition.y;
        

        // Rotates the entity
        Vector3 relativePos = finalPosition - originalPosition;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        Quaternion finalRotation = Quaternion.RotateTowards(commonParameters.enemy.transform.rotation, rotation, parameters.rotationVelocity * Time.deltaTime);
        commonParameters.enemy.transform.rotation = finalRotation;
        


        Vector3 move = commonParameters.enemy.transform.forward * parameters.speed * Time.deltaTime;
        //move the entity, and set the gravity
        if (commonParameters.onFloor) {
            move += (commonParameters.enemy.transform.up * -1) * 25 * Time.deltaTime;
        }

        _controller.Move(move);

        //reached calculation is on specific IA
        DropReached(animator);
    }

    private void DropReached(Animator animator) {
        if (commonParameters.drop == null) {
            return;
        }

        // Checks if the entity is close enough to the target point
        float squaredDistance = (commonParameters.drop.transform.position - commonParameters.enemy.transform.position).sqrMagnitude;
        // The squared distance is used becouse a multiplication is cheaper than a square root
        float distanceTolerance = animator.GetInteger("SizeDrop");
        distanceTolerance += _minimumDistance;
        distanceTolerance *= distanceTolerance;
        if (squaredDistance < distanceTolerance)
            animator.SetBool("Reached", true);
    }

    private float GetMinimumDistance() {
        float time = 360 / parameters.rotationVelocity;
        float longitude = parameters.speed * time;
        float radius = longitude / (2 * Mathf.PI);
        return radius;
    }


}
