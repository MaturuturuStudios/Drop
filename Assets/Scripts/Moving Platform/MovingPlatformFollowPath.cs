using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingPlatformFollowPath : MonoBehaviour {

	// Custom enumerations
	public enum FollowType {
		MoveTowards,
		Lerp
	}

	public enum PathType {
		BackAndForward,
		Loop
	}

	// Public attributes
	public FollowType followType = FollowType.MoveTowards;
	public PathType pathType = PathType.BackAndForward;

	public MovingPlatformPathDefinition path;
	public float speed = 10;
	public float maxDistanceToGoal = 0.1f;

	// Private variables
	private IEnumerator<Transform> _pathEnumerator;

	public void Start() {
		// A path is required
		if (path == null) {
			Debug.LogError("Path cannot be null!", gameObject);
			return;
		}

		// Selects the current path type
		switch (pathType) {
			case PathType.BackAndForward:
				_pathEnumerator = path.GetBackAndForwardEumerator();
				break;
			case PathType.Loop:
				_pathEnumerator = path.GetLoopEumerator();
				break;
			default:
				Debug.LogError("Unrecognized path type!", gameObject);
				return;
		}
		// Moves the enumerator to the first position and sets the position of the entity
		_pathEnumerator.MoveNext();
		if (_pathEnumerator.Current == null)
			return;
		transform.position = _pathEnumerator.Current.position;
	}

	public void Update() {
		if (_pathEnumerator == null || _pathEnumerator.Current == null)
			return;

		// Moves the entity using the right function
		switch (followType) {
			case FollowType.MoveTowards:
				transform.position = Vector3.MoveTowards(transform.position, _pathEnumerator.Current.position, speed * Time.deltaTime);
				break;
			case FollowType.Lerp:
				transform.position = Vector3.Lerp(transform.position, _pathEnumerator.Current.position, speed * Time.deltaTime);
				break;
			default:
				return;
		}

		// Checks if the entity is close enough to the target point
		float squaredDistance = (transform.position - _pathEnumerator.Current.position).sqrMagnitude;
		// The squared distance is used becouse a multiplication is cheaper than a square root
		if (squaredDistance < maxDistanceToGoal * maxDistanceToGoal)
			_pathEnumerator.MoveNext();
	}
}
