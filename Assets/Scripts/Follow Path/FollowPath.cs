using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Moves an entity according to a PathDefinition script.
/// </summary>
public class FollowPath : MonoBehaviour {

	#region Custom Enumerations

	/// <summary>
	/// Defines how will the entity move to the next point in the path.
	/// </summary>
	public enum FollowType {

		/// <summary>
		/// Moves towards the points at constant speed.
		/// </summary>
		MoveTowards,

		/// <summary>
		/// Moves towards the points at decreasing speed.
		/// </summary>
		Lerp
	}

	/// <summary>
	/// Defines how the entity looks for the next point in the path.
	/// </summary>
	public enum PathType {

		/// <summary>
		/// After reaching the end, returns using the reverse path.
		/// </summary>
		BackAndForward,

		/// <summary>
		/// After reaching the end, returns to the first point in the path
		/// and starts again in the same order.
		/// </summary>
		Loop,
        /// <summary>
        /// Start and continue to random points
        /// </summary>
        Random
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// Defines how will the entity move to the next point in the path.
	/// </summary>
	public FollowType followType = FollowType.MoveTowards;

	/// <summary>
	/// Defines how the entity looks for the next point in the path.
	/// </summary>
	public PathType pathType = PathType.BackAndForward;

	/// <summary>
	/// A reference to the path this entity will follow.
	/// </summary>
	public PathDefinition path;

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

	#endregion

	#region Private Attributes

	/// <summary>
	/// Enumerator of the path.
	/// </summary>
	private IEnumerator<Transform> _pathEnumerator;

	/// <summary>
	/// A reference to the entity's transform.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the first frame this entity is enabled.
	/// Retrieves all the desired components and initialices the platform.
	/// </summary>
	public void Start() {
		// A path is required
		if (path == null) {
			Debug.LogError("Path cannot be null!", gameObject);
			return;
		}

		// Recovers the transform component
		_transform = transform;

		// Selects the current path type
		switch (pathType) {
			case PathType.BackAndForward:
				_pathEnumerator = path.GetBackAndForwardEnumerator();
				break;
			case PathType.Loop:
				_pathEnumerator = path.GetLoopEumerator();
				break;
            case PathType.Random:
                _pathEnumerator = path.GetRandomEnumerator();
                break;
			default:
				Debug.LogError("Unrecognized path type!", gameObject);
				return;
		}
		// Moves the enumerator to the first position and sets the position of the entity
		_pathEnumerator.MoveNext();
		if (_pathEnumerator.Current == null)
			return;
		_transform.position = _pathEnumerator.Current.position;
	}

	/// <summary>
	/// Unity's method called every frame.
	/// Moves the platform close to the next point in the path and checks
	/// if it's close enough to it.
	/// </summary>
	public void Update() {
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

		// Rotates the entity
		if (useOrientation) {
			float traveledDistance = (_transform.position - originalPosition).magnitude;
			float remainingDistance = (_pathEnumerator.Current.position - originalPosition).magnitude;
			if (remainingDistance > 0.01f)
				_transform.rotation = Quaternion.Lerp(_transform.rotation, _pathEnumerator.Current.rotation, traveledDistance / remainingDistance);
		}

		// Checks if the entity is close enough to the target point
		float squaredDistance = (_transform.position - _pathEnumerator.Current.position).sqrMagnitude;
		// The squared distance is used becouse a multiplication is cheaper than a square root
		if (squaredDistance < maxDistanceToGoal * maxDistanceToGoal)
			_pathEnumerator.MoveNext();
	}

    /// <summary>
	/// Unity's method called by the editor in order to draw the gizmos.
	/// Draws the path on the editor.
	/// </summary>
	public void OnDrawGizmos() {
        path.OnDrawGizmos();
    }

    #endregion
}
