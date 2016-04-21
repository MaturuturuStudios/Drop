using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WalkingParameters {
    /// <summary>
    /// The entity to move
    /// </summary>
    [HideInInspector]
    public GameObject enemy = null;
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
    /// <summary>
    /// Is a terrain enemy? or maybe it fly?
    /// </summary>
    public bool onFloor = true;
    /// <summary>
    /// Time to stay in detect state
    /// </summary>
    public float timeUntilIddle = 0;
}

public class Walking : StateMachineBehaviour {
    #region Public and hidden in inspector attributes
    [HideInInspector]
    public WalkingParameters parameters;
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


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _transform = parameters.enemy.transform;
        _deltaTime = parameters.timeUntilIddle;
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
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //check if have condition to change state
        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = animator.GetInteger("LimitSizeDrop");
        if (sizeLimit > 0 && size >= sizeLimit) {
            animator.SetBool("GoAway", true);
        } else if (sizeLimit <= 0 || (size < sizeLimit && size > 0)) {
            animator.SetBool("Detect", true);
            return;
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
        Vector3 originalPosition = _transform.position;

        // Moves the entity using the right function
        switch (parameters.followType) {
            case FollowType.MoveTowards:
                _transform.position = Vector3.MoveTowards(_transform.position, _pathEnumerator.Current.position, parameters.speed * Time.deltaTime);
                break;
            case FollowType.Lerp:
                _transform.position = Vector3.Lerp(_transform.position, _pathEnumerator.Current.position, parameters.speed * Time.deltaTime);
                break;
            default:
                return;
        }
        
        //TODO orientation does not work as expected
        // Rotates the entity
        if (parameters.useOrientation) {
            float traveledDistance = (_transform.position - originalPosition).magnitude;
            float remainingDistance = (_pathEnumerator.Current.position - originalPosition).magnitude;
            if (remainingDistance > 0.01f)
                _transform.rotation = Quaternion.Lerp(_transform.rotation, _pathEnumerator.Current.rotation, traveledDistance / remainingDistance);
        }

        
        if (parameters.onFloor) {
            float yPosition = originalPosition.y;
            originalPosition = _transform.position;
            originalPosition.y = yPosition;
        }

        // Checks if the entity is close enough to the target point
        float squaredDistance = (_transform.position - _pathEnumerator.Current.position).sqrMagnitude;
        // The squared distance is used because a multiplication is cheaper than a square root
        if (squaredDistance < parameters.maxDistanceToGoal * parameters.maxDistanceToGoal)
            _pathEnumerator.MoveNext();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }


}
