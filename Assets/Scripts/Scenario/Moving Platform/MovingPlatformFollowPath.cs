using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Moves an entity according to a MovingPlatformPathDefinition script.
/// </summary>
public class MovingPlatformFollowPath : MonoBehaviour {

	#region Custom Enumerations

	/// <summary>
	/// Defines how will the platform move to the next point in the path.
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
	/// Defines how the platform looks for the next point in the path.
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
		Loop
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// Defines how will the platform move to the next point in the path.
	/// </summary>
	public FollowType followType = FollowType.MoveTowards;

	/// <summary>
	/// Defines how the platform looks for the next point in the path.
	/// </summary>
	public PathType pathType = PathType.BackAndForward;

	/// <summary>
	/// A reference to the path this platform will follow.
	/// </summary>
	public MovingPlatformPathDefinition path;

	/// <summary>
	/// Speed of the platform.
	/// </summary>
	public float speed = 10;

	/// <summary>
	/// Distance tolerance for the platform to look a new point in the path.
	/// </summary>
	public float maxDistanceToGoal = 0.1f;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Enumerator of the path.
	/// </summary>
	private IEnumerator<Transform> _pathEnumerator;

	/// <summary>
	/// A reference to the entity's rigidbody.
	/// </summary>
	private Rigidbody _rigidbody;

	/// <summary>
	/// A reference to the entity's transform.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the first frame this entity is enabled.
	/// Recovers all the desired components and initialices the platform.
	/// </summary>
	public void Start() {
		// A path is required
		if (path == null) {
			Debug.LogError("Path cannot be null!", gameObject);
			return;
		}

		// Recovers the rigidbody component and the transform
		_rigidbody = GetComponent<Rigidbody>();
		_transform = transform;

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

		// Moves the entity using the right function
		switch (followType) {
			case FollowType.MoveTowards:
				_rigidbody.MovePosition(Vector3.MoveTowards(_transform.position, _pathEnumerator.Current.position, speed * Time.deltaTime));
				break;
			case FollowType.Lerp:
				_rigidbody.MovePosition(Vector3.Lerp(_transform.position, _pathEnumerator.Current.position, speed * Time.deltaTime));
				break;
			default:
				return;
		}

		// Checks if the entity is close enough to the target point
		float squaredDistance = (_transform.position - _pathEnumerator.Current.position).sqrMagnitude;
		// The squared distance is used becouse a multiplication is cheaper than a square root
		if (squaredDistance < maxDistanceToGoal * maxDistanceToGoal)
			_pathEnumerator.MoveNext();
	}

	#endregion
}
