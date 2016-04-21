using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WalkingParameters {
    /// <summary>
	/// A reference to the path this entity will follow.
	/// </summary>
	public PathDefinition path;
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
    [HideInInspector]
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
	/// A reference to the entity's transform.
	/// </summary>
	private Transform _transform;
    /// <summary>
	/// Enumerator of the path.
	/// </summary>
	private IEnumerator<Transform> _pathEnumerator;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _transform = commonParameters.enemy.transform;
        //start timer
        _deltaTime = parameters.timeUntilIddle;
        //get path
        if (_pathEnumerator == null) {
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
        _pathEnumerator.MoveNext();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
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
        if (_pathEnumerator == null || _pathEnumerator.Current == null)
            return;

        // Saves the original position
        //float y = _transform.position.y;
        //float x = _transform.position.x;
        //Vector3 originalPosition = _transform.position;
        Vector3 target = _pathEnumerator.Current.position;
        if (commonParameters.onFloor) 
            target.y = _transform.position.y;

        // Moves the entity using the right function
        switch (parameters.followType) {
            case FollowType.MoveTowards:
                _transform.position = Vector3.MoveTowards(_transform.position, target, parameters.speed * Time.deltaTime);
                break;
            case FollowType.Lerp:
                _transform.position = Vector3.Lerp(_transform.position, target, parameters.speed * Time.deltaTime);
                break;
            default:
                return;
        }

        if (commonParameters.onFloor) {
            //float yPosition = originalPosition.y;
            //originalPosition = _transform.position;
            //originalPosition.y = y;
            //originalPosition.x = x;
            //_transform.position = originalPosition;
        }

        // Rotates the entity
        if (parameters.useOrientation) {
            faceTarget();
        }

        // Checks if the entity is close enough to the target point
        float squaredDistance = (_transform.position - _pathEnumerator.Current.position).sqrMagnitude;
        // The squared distance is used because a multiplication is cheaper than a square root
        if (squaredDistance < parameters.maxDistanceToGoal * parameters.maxDistanceToGoal)
            _pathEnumerator.MoveNext();
    }

    private void faceTarget() {
        Quaternion _lookRotation;
        Vector3 _direction;
        Transform targetTransform = _pathEnumerator.Current.transform;
        Transform enemyTransform = commonParameters.enemy.transform;

        //find the vector pointing from our position to the target
        _direction = (targetTransform.position - enemyTransform.position).normalized;

        if (_direction == Vector3.zero) return;

        //create the rotation we need to be in to look at the target
        _lookRotation = Quaternion.LookRotation(_direction);

        _lookRotation.x = 0;
        _lookRotation.z = 0;
        //rotate us over time according to speed until we are in the required rotation
        enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation, _lookRotation, Time.deltaTime * commonParameters.RotationSpeed);
    }
    #endregion

}
