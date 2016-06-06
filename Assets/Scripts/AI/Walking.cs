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
    /// Velocity rotation when walking
    /// </summary>
    public float rotationVelocity = 150;
    /// <summary>
    /// Time to stay in detect state
    /// </summary>
    public float timeUntilIddle = 0;
    /// <summary>
    /// Particle system of walking effect
    /// </summary>
    public GameObject walkingFX;
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
    /// The next point to move toward
    /// </summary>
    private Vector3 _targetPosition;
    /// <summary>
    /// Minimum distance to goal
    /// </summary>
    private float _minimumDistance;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _controller = commonParameters.enemy.GetComponent<CharacterController>();
        //start timer
        _deltaTime = parameters.timeUntilIddle;
        _minimumDistance = GetMinimumDistance() + parameters.maxDistanceToGoal;

        // Start particle system
        parameters.walkingFX.SetActive(true);

        // Moves the enumerator to the first/next position
        GetNextTarget();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
        animator.SetBool("Near", false);

        // Stop particle system
        parameters.walkingFX.SetActive(false);
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
            Vector3 relativePos = finalPosition - originalPosition;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            finalRotation = Quaternion.RotateTowards(commonParameters.enemy.transform.rotation, rotation, parameters.rotationVelocity * Time.deltaTime);
            commonParameters.enemy.transform.rotation = finalRotation;
        }


        Vector3 move = commonParameters.enemy.transform.forward * parameters.speed * Time.deltaTime;
        //move the entity, and set the gravity
        if (commonParameters.onFloor) {
            move += (commonParameters.enemy.transform.up*-1) * 25 * Time.deltaTime;
        }
        
        _controller.Move(move);

        // Checks if the entity is close enough to the target point
        Vector3 position=commonParameters.enemy.transform.position;
        //ignore axis if on floor
        if (commonParameters.onFloor)
            position.y = _targetPosition.y;
        float squaredDistance = (position - _targetPosition).sqrMagnitude;
        float distanceGoal = _minimumDistance *_minimumDistance;
        // The squared distance is used because a multiplication is cheaper than a square root
        if (squaredDistance < distanceGoal)
            GetNextTarget();
    }

    private void GetNextTarget() {
        if (parameters.usePath) {
			parameters.path.MoveNext();
            _targetPosition = parameters.path.Current.position;
        } else {
            //select random point in the area
            _targetPosition = parameters.walkArea.GetRandomPoint() + commonParameters.rootEntityPosition.position;
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
