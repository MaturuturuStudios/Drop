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
    #region Attributes
    /// <summary>
    /// Parameters of the chase script
    /// </summary>
    public ChaseParameters parameters;
    /// <summary>
    /// A reference to the entity's transform.
    /// </summary>
    private Transform _transform;
    #endregion

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _transform = parameters.enemy.transform;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
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

        //move towards target
        _transform.position = Vector3.MoveTowards(_transform.position, parameters.target.transform.position, parameters.speed * Time.deltaTime);

        //reached calculation is on specific IA
    }

}
