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
    /// If enabled, the entity will rotate to face the end point
    /// </summary>
    public bool useOrientation = false;
    public bool useOrientationFinalPosition = true;
    /// <summary>
    /// 
    /// </summary>
    public float rotationVelocity = 150;
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
    /// Minimum distance to goal
    /// </summary>
    //private float _minimumDistance;
    /// <summary>
    /// Tell if arrived to the target point (but maybe not at the desired rotation)
    /// </summary>
    private bool _positionTargeted;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("GoAway", false);
        _controller = commonParameters.enemy.GetComponent<CharacterController>();
        //_minimumDistance = GetMinimumDistance() + parameters.maxDistanceToGoal;

		// Moves the enumerator to the first/next position
		parameters.endPoint.MoveNext();
        _positionTargeted = false;
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
        // Saves the original position
        Vector3 move = Vector3.zero;
        //only move if not reached the point
        if (!_positionTargeted) {
            Vector3 originalPosition = commonParameters.enemy.transform.position;
            Vector3 finalPosition = MoveEnemy(originalPosition);
            RotateEnemy(originalPosition, finalPosition);

            //move the entity
            move = commonParameters.enemy.transform.forward * parameters.speed * Time.deltaTime;
        }

        //set gravity and move
        if (commonParameters.onFloor) {
            move += (commonParameters.enemy.transform.up * -1) * 25 * Time.deltaTime;
        }
        _controller.Move(move);

        CheckTargetPoint(animator);
    }

    /// <summary>
    /// Check if reached the desired point
    /// </summary>
    /// <param name="animator"></param>
    private void CheckTargetPoint(Animator animator) {
        //reached point!
        if (commonParameters.AI.CheckTargetPoint(parameters.endPoint.Current.position, commonParameters.minimumWalkingDistance)) {
            Transform last = parameters.endPoint.Current;
            int lastPoint=parameters.endPoint.points.Length-1;

            //end of path?
            if (last != parameters.endPoint.points[lastPoint]) {
                //don't want to stay in the point, continue!
                _positionTargeted = false;
                parameters.endPoint.MoveNext();
            }
                //if not...
            else {
                //don't move, only rotate
                _positionTargeted = true;

                if (parameters.useOrientationFinalPosition) {
                    //check if rotation is already targeted
                    Quaternion targetRotation = parameters.endPoint.Current.rotation;
                    if (commonParameters.AI.CheckTargetRotation(targetRotation, commonParameters.toleranceDegreeToGoal)) {
                        //yes? change to "recolect"
                        animator.SetTrigger("Recolect");
                    } else {
                        //no? rotate it
                        RotateEnemy(targetRotation);
                    }
                } else {
                    animator.SetTrigger("Recolect");
                }

            }

        } else {
            _positionTargeted = false;
        }
    }

    /// <summary>
    /// Move the enemy
    /// </summary>
    /// <param name="originalPosition"></param>
    /// <returns></returns>
    private Vector3 MoveEnemy(Vector3 originalPosition) {
        Vector3 finalPosition = originalPosition;
        Vector3 target = parameters.endPoint.Current.position;

        // Moves the entity using the right function
        switch (parameters.followType) {
            case FollowType.MoveTowards:
                finalPosition = Vector3.MoveTowards(originalPosition, target, parameters.speed * Time.deltaTime);
                break;
            case FollowType.Lerp:
                finalPosition = Vector3.Lerp(originalPosition, target, parameters.speed * Time.deltaTime);
                break;
        }

        if (commonParameters.onFloor)
            finalPosition.y = originalPosition.y;

        return finalPosition;
    }

    /// <summary>
    /// Rotate the entity to target
    /// The target is the new position of the entity after moving
    /// </summary>
    /// <param name="originalPosition">position the entity is</param>
    /// <param name="finalPosition">target point</param>
    private void RotateEnemy(Vector3 originalPosition, Vector3 finalPosition) {
        if (parameters.useOrientation) {
            Quaternion finalRotation = Quaternion.identity;

            Vector3 relativePos = finalPosition - originalPosition;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            finalRotation = Quaternion.RotateTowards(commonParameters.enemy.transform.rotation, rotation, parameters.rotationVelocity * Time.deltaTime);

            commonParameters.enemy.transform.rotation = finalRotation;
        }
    }

    private void RotateEnemy(Quaternion target) {
        if (parameters.useOrientation) {
            //float maxDegrees = 360 / parameters.rotationVelocity;
            Quaternion finalRotation = Quaternion.identity;
            finalRotation = Quaternion.RotateTowards(commonParameters.enemy.transform.rotation, target, parameters.rotationVelocity * Time.deltaTime);
            commonParameters.enemy.transform.rotation = finalRotation;
        }
    }

    #endregion
}
