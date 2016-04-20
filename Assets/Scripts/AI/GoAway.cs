﻿using UnityEngine;
using System.Collections.Generic;

public class GoAway : StateMachineBehaviour {
    #region Public and hidden in inspector attributes
    [HideInInspector]
    public GameObject enemy = null;


    [HideInInspector]
    /// <summary>
	/// A reference to the path this entity will follow.
	/// </summary>
	public PathDefinition endPoint;
    [HideInInspector]
    /// <summary>
	/// Defines how will the entity move to the next point in the path.
	/// </summary>
	public FollowType followType = FollowType.MoveTowards;
    [HideInInspector]
    /// <summary>
	/// Speed of the entity.
	/// </summary>
	public float speed = 10;
    [HideInInspector]
    /// <summary>
    /// Distance tolerance for the entity to look for a new point in the path.
    /// </summary>
    public float maxDistanceToGoal = 0.1f;
    [HideInInspector]
    /// <summary>
    /// If enabled, the entity will also rotate to fit the point's rotation.
    /// </summary>
    public bool useOrientation = false;
    [HideInInspector]
    /// <summary>
    /// Defines how the entity looks for the next point in the path.
    /// </summary>
    public PathType pathType = PathType.Loop;
    [HideInInspector]
    /// <summary>
    /// Is a terrain enemy? or maybe it fly?
    /// </summary>
    public bool onFloor = true;
    [HideInInspector]
    #endregion

    #region Private attribute
    /// <summary>
	/// A reference to the entity's transform.
	/// </summary>
	private Transform _transform;
    /// <summary>
	/// Enumerator of the path.
	/// </summary>
	private IEnumerator<Transform> _pathEnumerator;
    #endregion

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("GoAway", false);
        _transform = enemy.transform;
        if (_pathEnumerator == null) {
            // Selects the current path type
            switch (pathType) {
                case PathType.BackAndForward:
                    _pathEnumerator = endPoint.GetBackAndForwardEnumerator();
                    break;
                case PathType.Loop:
                    _pathEnumerator = endPoint.GetLoopEumerator();
                    break;
                case PathType.Random:
                    _pathEnumerator = endPoint.GetRandomEnumerator();
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
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //set the moving path
        if (_pathEnumerator == null || _pathEnumerator.Current == null)
            return;

        // Saves the original position
        Vector3 originalPosition = _transform.position;

        // Moves the entity using the right function
        switch (followType) {
            case FollowType.MoveTowards:
                _transform.position = Vector3.MoveTowards(_transform.position, _pathEnumerator.Current.position, speed * Time.deltaTime);
                break;
            case FollowType.Lerp:
                _transform.position = Vector3.Lerp(_transform.position, _pathEnumerator.Current.position, speed * Time.deltaTime);
                break;
            default:
                return;
        }

        //TODO orientation does not work as expected
        // Rotates the entity
        if (useOrientation) {
            float traveledDistance = (_transform.position - originalPosition).magnitude;
            float remainingDistance = (_pathEnumerator.Current.position - originalPosition).magnitude;
            if (remainingDistance > 0.01f)
                _transform.rotation = Quaternion.Lerp(_transform.rotation, _pathEnumerator.Current.rotation, traveledDistance / remainingDistance);
        }


        if (onFloor) {
            float yPosition = originalPosition.y;
            originalPosition = _transform.position;
            originalPosition.y = yPosition;
        }

        // Checks if the entity is close enough to the target point
        float squaredDistance = (_transform.position - _pathEnumerator.Current.position).sqrMagnitude;
        // The squared distance is used because a multiplication is cheaper than a square root
        if (squaredDistance < maxDistanceToGoal * maxDistanceToGoal)
            _pathEnumerator.MoveNext();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }
}
