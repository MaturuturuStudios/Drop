using System;
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
    /// <summary>
    /// if true, the axis will be fixed at rotation
    /// </summary>
    public AxisBoolean fixedRotation;
    /// <summary>
    /// Particle system of walking effect
    /// </summary>
    public GameObject walkingFX;
}

public class Chase : StateMachineBehaviour, CollisionListener {
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
    /// Keep the drop the enemy is chasing to give it to the listeners at the end of chase
    /// </summary>
    private GameObject _dropChased;
    

    private Animator _animator;
    #endregion

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _controller = commonParameters.enemy.GetComponent<CharacterController>();
        commonParameters.colliders.AddListener(this);
        _animator = animator;
		_dropChased = commonParameters.drop;

		//Call listeners
		foreach (EnemyBehaviourListener listener in commonParameters.AI.listeners)
            listener.OnBeginChase(commonParameters.AI, _dropChased);

        // Starts particle system
        foreach (ParticleSystem system in parameters.walkingFX.GetComponentsInChildren<ParticleSystem>()) {
            ParticleSystem.EmissionModule emission = system.emission;
            emission.enabled = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
        animator.SetBool("Near", false);
        commonParameters.colliders.RemoveListener(this);
        
		//Call listeners
		foreach (EnemyBehaviourListener listener in _dropChased.GetComponents<EnemyBehaviourListener>())
			listener.OnEndChase(commonParameters.AI, _dropChased);
		foreach (EnemyBehaviourListener listener in commonParameters.AI.listeners)
            listener.OnEndChase(commonParameters.AI, _dropChased);

        // Stops particle system
        foreach (ParticleSystem system in parameters.walkingFX.GetComponentsInChildren<ParticleSystem>()) {
            ParticleSystem.EmissionModule emission = system.emission;
            emission.enabled = false;
        }
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
        _dropChased = commonParameters.drop;

        // Saves the original position
        Vector3 move = Vector3.zero;
        Vector3 originalPosition = commonParameters.enemy.transform.position;
        Vector3 finalPosition = AIMethods.MoveEnemy(originalPosition, commonParameters.drop.transform.position,
                                    FollowType.MoveTowards, commonParameters.onFloor, parameters.speed);
        AIMethods.RotateEnemyTowards(commonParameters.enemy, parameters.fixedRotation, commonParameters.initialRotationEnemy, 
                            parameters.rotationVelocity, originalPosition, finalPosition);

        Vector3 direction = (finalPosition - originalPosition);
        //give preference on y axis to not let the drop run away
        direction.x *= 0.5f;
        direction.y *= 1.5f;
        direction=direction.normalized;

        //move the entity
        move = direction * parameters.speed * Time.deltaTime;
        //set gravity and move
        if (commonParameters.onFloor) {
            move += (commonParameters.enemy.transform.up * -1) * 25 * Time.deltaTime;
        }
        _controller.Move(move);
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
}
