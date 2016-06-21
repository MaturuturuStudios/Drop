using UnityEngine;

/// <summary>
/// Makes an object with a rigidbody attached inherit the
/// velocity from other object's movement.
/// </summary>
public class InheritVelocity : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Velocity multiplication factor.
	/// </summary>
	public float forceFactor = 1;

	/// <summary>
	/// The object providing the velocity..
	/// </summary>
	public Transform reference;

	#endregion

	#region Private Attributes

	/// <summary>
	/// A reference to this entity's Rigidbody component.
	/// </summary>
	private Rigidbody _rigidbody;

	/// <summary>
	/// Object's position on the last frame.
	/// </summary>
	private Vector3 _lastFramePosition;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_rigidbody = GetComponent<Rigidbody>();
		if (_rigidbody == null) {
			Debug.LogError("Error: No Rigidbody attached to the entity!");
			enabled = false;
		}
		if (reference == null) {
			Debug.LogError("Error: The referenced object was not specified!");
			enabled = false;
		}
	}

	/// <summary>
	/// Unity's method called when the script or it's object becomes enabled.
	/// </summary>
	void OnEnable() {
		// Initialices the reference position
		_lastFramePosition = reference.position;
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// If the game is paused, abort this operation
		if (Time.deltaTime == 0)
			return;

		// Adds force to the object using the object's displacement
		Vector3 displacement = reference.position - _lastFramePosition;
		Vector3 force = -forceFactor * displacement / Time.deltaTime;
		_rigidbody.AddForce(force);
		_lastFramePosition = reference.position;
	}

	#endregion
}
