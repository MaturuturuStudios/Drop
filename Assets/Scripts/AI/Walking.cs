﻿using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WalkingParameters {
    /// <summary>
    /// True if use the path to walk or false if use the area instead
    /// </summary>
    public bool usePath=true;
    /// <summary>
	/// A reference to the path this entity will follow.
	/// </summary>
	public PathDefinition path;
    /// <summary>
    /// Area where entity will stay walking
    /// </summary>
    public Region walkArea;

    /// <summary>
	/// Defines how will the entity move to the next point in the path.
	/// </summary>
	public FollowType followType = FollowType.MoveTowards;
    /// <summary>
	/// Speed of the entity.
	/// </summary>
	public float speed = 10;
    /// <summary>
    /// Distance tolerance for the entity to look for a new point in the path.
    /// </summary>
    public float maxDistanceToGoal = 0.1f;
    /// <summary>
    /// If enabled, the entity will also rotate to fit the point's rotation.
    /// </summary>
    public bool useOrientation = false;
    /// <summary>
    /// Defines how the entity looks for the next point in the path.
    /// </summary>
    public PathType pathType = PathType.Random;
    /// <summary>
    /// Time to stay in detect state
    /// </summary>
    public float timeUntilIddle = 0;
}

public class Walking : StateMachineBehaviour {
    #region Public and hidden in inspector attributes
    [HideInInspector]
    public WalkingParameters parameters;
    [HideInInspector]
    public CommonParameters commonParameters;
    /// <summary>
    /// Timer
    /// </summary>
    private float _deltaTime;
    #endregion

    #region Private attribute
    /// <summary>
	/// A reference to the entity's controller.
	/// </summary>
	private CharacterController _controller;
    /// <summary>
	/// Enumerator of the path.
	/// </summary>
	private IEnumerator<Transform> _pathEnumerator;
    /// <summary>
    /// The next point to move toward
    /// </summary>
    private Vector3 _targetPosition;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _controller = commonParameters.enemy.GetComponent<CharacterController>();
        //start timer
        _deltaTime = parameters.timeUntilIddle;
        //get path
        if (parameters.usePath && _pathEnumerator == null) {
            // Selects the current path type
            switch (parameters.pathType) {
                case PathType.BackAndForward:
                    _pathEnumerator = parameters.path.GetBackAndForwardEnumerator();
                    break;
                case PathType.Loop:
                    _pathEnumerator = parameters.path.GetLoopEumerator();
                    break;
                case PathType.Random:
                    _pathEnumerator = parameters.path.GetRandomEnumerator();
                    break;
                default:
                    Debug.LogError("Unrecognized path type!");
                    return;
            }
        }

        // Moves the enumerator to the first/next position
        getNextTarget();
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
        //check if have condition to change state
        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = commonParameters.sizeLimitDrop;
        if (sizeLimit > 0 && size >= sizeLimit)  {
            animator.SetBool("GoAway", true);
        } else if ((sizeLimit <= 0 || size < sizeLimit) && size > 0) { 
            animator.SetBool("Detect", true);
        }

        //check timer
        if (parameters.timeUntilIddle > 0) {
            _deltaTime -= Time.deltaTime;
            if (_deltaTime <= 0) {
                animator.SetBool("Timer", true);
                return;
            }
        }

        //set the moving path
        if (parameters.usePath && (_pathEnumerator == null || _pathEnumerator.Current == null))
            return;

        // Saves the original position
        Vector3 originalPosition = commonParameters.enemy.transform.position;
        Vector3 finalPosition = originalPosition;

        // Moves the entity using the right function
        switch (parameters.followType) {
            case FollowType.MoveTowards:
                finalPosition = Vector3.MoveTowards(originalPosition, _targetPosition, parameters.speed * Time.deltaTime);
                break;
            case FollowType.Lerp:
                finalPosition = Vector3.Lerp(originalPosition, _targetPosition, parameters.speed * Time.deltaTime);
                break;
            default:
                return;
        }

        if (commonParameters.onFloor)
            finalPosition.y = originalPosition.y;

        // Rotates the entity
        Quaternion finalRotation = Quaternion.identity;
        if (parameters.useOrientation) {
            //faceTarget(finalPosition);
            Vector3 relativePos = finalPosition - originalPosition;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            finalRotation = Quaternion.RotateTowards(commonParameters.enemy.transform.rotation, rotation, 150 * Time.deltaTime);
            commonParameters.enemy.transform.rotation = finalRotation;
        }

        //move the entity, and set the gravity
        if(commonParameters.onFloor)
            finalPosition.y -= 25 * Time.deltaTime;
        Vector3 move = finalPosition - originalPosition;
        
        //Vector3 os = finalPosition - originalPosition;
        //os = os.normalized;
        //os *= parameters.speed*Time.deltaTime;
        //os = finalRotation * os;
        _controller.Move(commonParameters.enemy.transform.forward*parameters.speed*Time.deltaTime);

        // Checks if the entity is close enough to the target point
        Vector3 position=commonParameters.enemy.transform.position;
        //ignore axis if on floor
        if (commonParameters.onFloor)
            position.y = _targetPosition.y;
        float squaredDistance = (position - _targetPosition).sqrMagnitude;
        // The squared distance is used because a multiplication is cheaper than a square root
        if (squaredDistance < parameters.maxDistanceToGoal * parameters.maxDistanceToGoal)
            getNextTarget();
    }

    void getNextTarget() {
        if (parameters.usePath) {
            _pathEnumerator.MoveNext();
            _targetPosition = _pathEnumerator.Current.position;
        } else {
            //select random point in the area
            _targetPosition = parameters.walkArea.GetRandomPoint() + commonParameters.rootEntityPosition.position;
        }
    }

    private void faceTarget(Vector3 finalPosition) {
        Quaternion _lookRotation;
        Vector3 _direction;
        Transform enemyTransform = commonParameters.enemy.transform;

        //find the vector pointing from our position to the target
        _direction = (_targetPosition - finalPosition).normalized;

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
