using UnityEngine;

[System.Serializable]
public class AttackParameters {
    [Range(0,1)]
    ///<summary>
    /// in which moment of the animation should mke effective the attack
    /// </summary>
    public float attackMoment;
    /// <summary>
    /// the destination point the drop will be launched
    /// </summary>
    public LaunchCharacter launcher = new LaunchCharacter();
    /// <summary>
    /// if true, the axis will be fixed at rotation
    /// </summary>
    public AxisBoolean fixedRotation;
    /// <summary>
    /// if not zero, during attack the enemy still moves toward the drop
    /// </summary>
    public float speed;
    /// <summary>
    /// desired rotation while attacking
    /// </summary>
    public Vector3 attackRotation;
}

public class Attack : StateMachineBehaviour {
    [HideInInspector]
    public CommonParameters commonParameters;
    public AttackParameters parameters;

    /// <summary>
	/// A reference to the entity's controller.
	/// </summary>
	private CharacterController _controller;

    private bool _attackDone;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //OnStateEnter is called on the first frame of the state being played.
        _attackDone = false;

        _controller = commonParameters.enemy.GetComponent<CharacterController>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //OnStateExit is called on the last frame of a transition to another state.
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
        animator.SetBool("Near", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (commonParameters.drop == null) {
            return;
        }
        //faceTarget
        Vector3 originalPosition = commonParameters.enemy.transform.position;
        Vector3 finalPosition = commonParameters.drop.transform.position;
        Quaternion rotation = Quaternion.Euler(parameters.attackRotation);
        AIMethods.RotateEnemySlerp(commonParameters.enemy, parameters.fixedRotation, rotation,
                                commonParameters.RotationSpeed, originalPosition, finalPosition);

        //if move, move it
        if (parameters.speed != 0) {
            Move();
        }


        float percentAnimation = stateInfo.normalizedTime;
        if (!_attackDone && percentAnimation > parameters.attackMoment) {
            CharacterControllerCustom controller = commonParameters.drop.GetComponent<CharacterControllerCustom>();
            Transform origin= commonParameters.drop.transform;
            Vector3 destiny = parameters.launcher.pointTarget.position;
            Vector3 fly = AIMethods.RepelDrop(origin, destiny, parameters.launcher);
            controller.Stop();
            controller.SendFlying(fly);
            _attackDone = true;
            
            //Call listeners
            foreach (EnemyBehaviourListener listener in commonParameters.AI.listeners)
                listener.OnAttack(commonParameters.AI, commonParameters.drop, fly);
        }
    }
    

    private void Move() {
        // Saves the original position
        Vector3 move = Vector3.zero;
        Vector3 target = commonParameters.drop.transform.position;
        Vector3 originalPosition = commonParameters.enemy.transform.position;
        Vector3 finalPosition = AIMethods.MoveEnemy(originalPosition, target, FollowType.MoveTowards,
                                                    commonParameters.onFloor, parameters.speed);
        AIMethods.RotateEnemyTowards(commonParameters.enemy, parameters.fixedRotation, commonParameters.initialRotationEnemy,
                            commonParameters.RotationSpeed, originalPosition, finalPosition);

        Vector3 direction = (finalPosition - originalPosition).normalized;

        //move the entity
        move = direction * parameters.speed * Time.deltaTime;


        //set gravity and move
        if (commonParameters.onFloor) {
            move += (commonParameters.enemy.transform.up * -1) * 25 * Time.deltaTime;
        }
        _controller.Move(move);
    }

}
