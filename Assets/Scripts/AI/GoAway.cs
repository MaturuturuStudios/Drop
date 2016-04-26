using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GoAwayParameters {
    /// <summary>
    /// A reference to the path this entity will follow.
    /// </summary>
    public PathDefinition endPoint;
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
    /// If enabled, the entity will rotate to face the end point
    /// </summary>
    public bool useOrientation = false;
    /// <summary>
    /// 
    /// </summary>
    public float rotationVelocity = 150;
    /// <summary>
    /// Defines how the entity looks for the next point in the path.
    /// </summary>
    public PathType pathType = PathType.Loop;
}

public class GoAway : StateMachineBehaviour {
    #region Public and hidden in inspector attributes
    [HideInInspector]
    public GoAwayParameters parameters;
    [HideInInspector]
    public CommonParameters commonParameters;
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
    /// Minimum distance to goal
    /// </summary>
    private float _minimumDistance;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("GoAway", false);
        _controller = commonParameters.enemy.GetComponent<CharacterController>();
        _minimumDistance = GetMinimumDistance() + parameters.maxDistanceToGoal;
        if (_pathEnumerator == null) {
            // Selects the current path type
            switch (parameters.pathType) {
                case PathType.BackAndForward:
                    _pathEnumerator = parameters.endPoint.GetBackAndForwardEnumerator();
                    break;
                case PathType.Loop:
                    _pathEnumerator = parameters.endPoint.GetLoopEumerator();
                    break;
                case PathType.Random:
                    _pathEnumerator = parameters.endPoint.GetRandomEnumerator();
                    break;
                default:
                    Debug.LogError("Unrecognized path type!");
                    return;
            }
        }

        // Moves the enumerator to the first/next position
        _pathEnumerator.MoveNext();
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
        //set the moving path
        if (_pathEnumerator == null || _pathEnumerator.Current == null)
            return;
        
        // Saves the original position
        Vector3 originalPosition = commonParameters.enemy.transform.position;
        Vector3 finalPosition = originalPosition;
        Vector3 target = _pathEnumerator.Current.position;
        

        // Moves the entity using the right function
        switch (parameters.followType) {
            case FollowType.MoveTowards:
                finalPosition = Vector3.MoveTowards(originalPosition, target, parameters.speed * Time.deltaTime);
                break;
            case FollowType.Lerp:
                finalPosition = Vector3.Lerp(originalPosition, target, parameters.speed * Time.deltaTime);
                break;
            default:
                return;
        }

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

        // Checks if the entity is close enough to the target point
        Vector3 position = commonParameters.enemy.transform.position;
        //ignore axis if on floor
        if (commonParameters.onFloor)
            position.y = target.y;
        float squaredDistance = (position - target).sqrMagnitude;
        float distanceGoal = _minimumDistance * _minimumDistance;
        // The squared distance is used because a multiplication is cheaper than a square root
        if (squaredDistance < distanceGoal) { 
            Transform last = _pathEnumerator.Current;
            _pathEnumerator.MoveNext();
            if (last == _pathEnumerator.Current) {
                animator.SetTrigger("Recolect");
            }
        }
    }

    private float GetMinimumDistance() {
        float time = 360 / parameters.rotationVelocity;
        float longitude = parameters.speed * time;
        float radius = longitude / (2 * Mathf.PI);
        return radius;
    }

    #endregion
}
