using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ChaseParameters {
    [HideInInspector]
    /// <summary>
    /// The entity to be chased
    /// </summary>
    public GameObject target;
    [HideInInspector]
    /// <summary>
    /// The entity to chase the target
    /// </summary>
    public GameObject enemy;
    /// <summary>
    /// The chasing speed
    /// </summary>
    public float speed;
}

public class Chase : StateMachineBehaviour {
    /// <summary>
    /// Parameters of the chase script
    /// </summary>
    public ChaseParameters parameters;

    #region Private attribute
    /// <summary>
    /// A reference to the entity's transform.
    /// </summary>
    private Transform _transform;
    #endregion

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _transform = parameters.enemy.transform;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Reached", false);
        animator.SetBool("GoAway", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //check if have condition to change state
        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = animator.GetInteger("LimitSizeDrop");
        if (sizeLimit > 0 && size >= sizeLimit) {
            animator.SetBool("GoAway", true);
            return;
        }

        //move toward target
        _transform.position = Vector3.Lerp(_transform.position, parameters.target.transform.position, parameters.speed * Time.deltaTime);

        //reached calculation is on specific IA
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

}
