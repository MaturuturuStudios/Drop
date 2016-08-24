using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FlyingParameters {
    /// <summary>
    /// True if use the path to walk or false if use the area instead
    /// </summary>
    public bool usePath = true;
    /// <summary>
	/// A reference to the path this entity will follow.
	/// </summary>
	public PathDefinition path;
    /// <summary>
    /// Area where entity will stay walking
    /// </summary>
    public Region walkArea;
    /// <summary>
    /// The margin between the previous and the new point on the walk area
    /// </summary>
    public float marginRandomWalkArea=2;

    /// <summary>
	/// Defines how will the entity move to the next point in the path.
	/// </summary>
	public FollowType followType = FollowType.MoveTowards;
    /// <summary>
	/// Speed of the entity.
	/// </summary>
	public float speed = 10;
    /// <summary>
    /// if true, the axis will be fixed at rotation
    /// </summary>
    public AxisBoolean fixedRotation;
    /// <summary>
    /// Velocity rotation when walking
    /// </summary>
    public float rotationVelocity = 150;
}

public class Flying : StateMachineBehaviour {
    #region Public and hidden in inspector attributes
    [HideInInspector]
    public FlyingParameters parameters;
    [HideInInspector]
    public CommonParameters commonParameters;
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
    /// Tell if arrived to the target point (but maybe not at the desired rotation)
    /// </summary>
    private bool _positionTargeted;
    
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _controller = commonParameters.enemy.GetComponent<CharacterController>();

        commonParameters.minimumWalkingDistance = AIMethods.GetMinimumDistance(parameters.speed, parameters.rotationVelocity);
        commonParameters.minimumWalkingDistance += commonParameters.toleranceDistanceToGoal;

        // Moves the enumerator to the first/next position
        _positionTargeted = false;
        GetNextTarget();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
        AIMethods.ClearAnimatorParameters(animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Saves the original position
        Vector3 move = Vector3.zero;
        //only move if not reached the point
        if (!_positionTargeted) {
            Vector3 originalPosition = commonParameters.enemy.transform.position;
            Vector3 finalPosition = AIMethods.MoveEnemy(originalPosition, _targetPosition, parameters.followType,
                                                        commonParameters.onFloor, parameters.speed);
            AIMethods.RotateEnemyTowards(commonParameters.enemy, parameters.fixedRotation, commonParameters.initialRotationEnemy,
                                parameters.rotationVelocity, originalPosition, finalPosition);

            Vector3 direction = (finalPosition - originalPosition).normalized;

            //move the entity
            move = direction * parameters.speed * Time.deltaTime;
        }

        //set gravity and move
        if (commonParameters.onFloor) {
            move += (commonParameters.enemy.transform.up * -1) * 25 * Time.deltaTime;
        }
        _controller.Move(move);

        CheckTargetPoint(animator);
    }

    /// <summary>
    /// Check if reached the desired point and rotate if needed when
    /// arrived to the point
    /// </summary>
    /// <param name="animator"></param>
    private void CheckTargetPoint(Animator animator) {
        //reached point!
        if (AIMethods.CheckTargetPoint(commonParameters.enemy, _targetPosition, 
            commonParameters.onFloor, commonParameters.minimumWalkingDistance)) {
                //don't want to stay in the point, continue!
                _positionTargeted = false;
                GetNextTarget();

        } else {
            _positionTargeted = false;
        }
    }
    
    /// <summary>
    /// Get the next target
    /// </summary>
    private void GetNextTarget() {
        //if not walking at all, the target is where the enemy actually is
        if (!commonParameters.walking) {
            _targetPosition = commonParameters.initialPositionEnemy;

            //choose next point
        } else if (parameters.usePath) {
            parameters.path.MoveNext();
            _targetPosition = parameters.path.Current.position;
        } else {
            //select random point in the area
            _targetPosition = parameters.walkArea.GetRandomPoint(_targetPosition, parameters.marginRandomWalkArea) + commonParameters.rootEntityPosition.position;
        }
    }

    #endregion

}
