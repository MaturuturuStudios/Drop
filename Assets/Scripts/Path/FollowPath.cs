using UnityEngine;

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
		Lerp,
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// A reference to the path this entity will follow.
	/// </summary>
	public PathDefinition path;

	/// <summary>
	/// Defines how will the entity move to the next point in the path.
	/// </summary>
	public FollowType followType = FollowType.MoveTowards;

	/// <summary>
	/// Speed the entity will have while following the path.
	/// </summary>
	public float speed = 10;

	/// <summary>
	/// If enabled, the entity will start moving to the next point as soon as it
	/// reaches the current one.
	/// </summary>
	public bool automatic = true;

	/// <summary>
	/// If enabled, the entity will also rotate to fit the point's rotation.
	/// </summary>
	public bool useOrientation = false;

	#endregion

	#region Private Attributes

	/// <summary>
	/// A reference to the entity's transform.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Costants

	/// <summary>
	/// Distance tolerance for the entity to look for a new point in the path.
	/// </summary>
	private static readonly float MIN_DISTANCE_TO_CHANGE = 0.1f;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the first frame this entity is enabled.
	/// Retrieves all the desired components and initialices the platform.
	/// </summary>
	public void Start() {
		// Recovers the transform component
		_transform = transform;

		// Moves the enumerator to the first position and sets the position of the entity
		path.MoveNext();
		_transform.position = path.Current.position;
	}

	/// <summary>
	/// Unity's method called every frame.
	/// Moves the platform close to the next point in the path and checks
	/// if it's close enough to it.
	/// </summary>
	public void Update() {
		// Saves the original position
		Vector3 originalPosition = _transform.position;

		// Moves the entity using the right function
		switch (followType) {
			case FollowType.MoveTowards:
				_transform.position = Vector3.MoveTowards(_transform.position, path.Current.position, speed * Time.deltaTime);
				break;
			case FollowType.Lerp:
				_transform.position = Vector3.Lerp(_transform.position, path.Current.position, speed * Time.deltaTime);
				break;
			default:
				return;
		}

		// Rotates the entity
		if (useOrientation) {
			float traveledDistance = (_transform.position - originalPosition).magnitude;
			float remainingDistance = (path.Current.position - originalPosition).magnitude;
			if (remainingDistance > 0.01f)
				_transform.rotation = Quaternion.Lerp(_transform.rotation, path.Current.rotation, traveledDistance / remainingDistance);
		}

		// Automatically changes to the next point in the path
		if (automatic) {
			// Checks if the entity is close enough to the target point
			float squaredDistance = (_transform.position - path.Current.position).sqrMagnitude;
			// The squared distance is used becouse a multiplication is cheaper than a square root
			if (squaredDistance < MIN_DISTANCE_TO_CHANGE * MIN_DISTANCE_TO_CHANGE)
				path.MoveNext();
		}
	}

	/// <summary>
	/// Makes the path move automatically to the next point as
	/// soon as the current one is reached.
	/// </summary>
	/// <param name="automatic">If the path should change to next point</param>
    public void SetAutomatic(bool automatic) {
        this.automatic = automatic;
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
