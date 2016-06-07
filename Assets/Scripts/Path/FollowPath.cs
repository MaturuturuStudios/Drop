using UnityEngine;

/// <summary>
/// Moves an entity according to a PathDefinition script.
/// </summary>
public class FollowPath : MonoBehaviour {

	#region Custom Enumerations

	/// <summary>
	/// Defines how will the entity move from one point to the next one.
	/// </summary>
	public enum MovementType {
		/// <summary>
		/// The entity will move at a certain speed to the path target.
		/// </summary>
		SPEED,

		/// <summary>
		/// The entity will take some time to reach the path target.
		/// </summary>
		DELAY
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// A reference to the path this entity will follow.
	/// </summary>
	public PathDefinition path;

	/// <summary>
	/// Defines how will the entity move from one point to the next one.
	/// </summary>
	public MovementType movementType = MovementType.SPEED;

	/// <summary>
	/// Movement speed of the entity;
	/// </summary>
	public float speed = 10;

	/// <summary>
	/// Time the entity will take to move from one point to
	/// the next.
	/// </summary>
	public float delay = 1;

	/// <summary>
	/// If enabled, the speed will be eased in and out.
	/// </summary>
	public bool smooth = false;

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

	/// <summary>
	/// The position of the last point in the path.
	/// </summary>
	private Vector3 _lastPosition;

	/// <summary>
	/// The rotation of the last point in the path.
	/// </summary>
	private Quaternion _lastRotation;

	/// <summary>
	/// Current position of the element in the interpolation.
	/// </summary>
	private float _linearFactor;

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
		_lastPosition = _transform.position;
		_linearFactor = 0;
	}

	/// <summary>
	/// Unity's method called every physics frame.
	/// Moves the platform close to the next point in the path and checks
	/// if it's close enough to it.
	/// </summary>
	public void FixedUpdate() {
		// Increases the interpolation factor
		if (movementType == MovementType.DELAY)
			_linearFactor += Time.fixedDeltaTime / delay;
		else if (movementType == MovementType.SPEED) {
			float distance = (path.Current.position - _lastPosition).magnitude;
			_linearFactor += Time.fixedDeltaTime * speed / distance;
		}

		// Smooths the factor
		float factor = _linearFactor;
		if (smooth)
			factor = Mathf.SmoothStep(0, 1, _linearFactor);

		// Moves the entity
		_transform.position = Vector3.Lerp(_lastPosition, path.Current.position, factor);

		// Rotates the entity
		if (useOrientation)
			_transform.rotation = Quaternion.Lerp(_lastRotation, path.Current.rotation, factor);

		// Automatically changes to the next point in the path
		if (automatic) {
			// Checks if the entity is close enough to the target point
			float squaredDistance = (_transform.position - path.Current.position).sqrMagnitude;
			// The squared distance is used becouse a multiplication is cheaper than a square root
			if (squaredDistance < MIN_DISTANCE_TO_CHANGE * MIN_DISTANCE_TO_CHANGE)
				Next();
		}
	}

	/// <summary>
	/// Changes the path to the next point.
	/// </summary>
	public void Next() {
		SaveLastPosition();
		path.MoveNext();
	}

	/// <summary>
	/// Changes the path to the previous point.
	/// </summary>
	public void Previous() {
		SaveLastPosition();
		path.Previous();
	}

	/// <summary>
	/// Changes the path to a random point.
	/// </summary>
	public void Random() {
		SaveLastPosition();
		path.Random();
	}

	/// <summary>
	/// Changes the path to the specified index's point.
	/// </summary>
	public void Set(int index) {
		SaveLastPosition();
		path.SetIndex(index);
	}

	/// <summary>
	/// Saves the needed information before changing the path target.
	/// </summary>
	private void SaveLastPosition() {
		_lastPosition = _transform.position;
		_lastRotation = _transform.rotation;
		_linearFactor = 0;
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
