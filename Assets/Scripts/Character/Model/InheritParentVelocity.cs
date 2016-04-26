using UnityEngine;

/// <summary>
/// Makes an object with a rigidbody attached inherit the
/// velocity from it's parent movement.
/// </summary>
public class InheritParentVelocity : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Velocity multiplication factor.
	/// </summary>
	public float forceFactor = 1;

	#endregion

	#region Private Attributes

	/// <summary>
	/// A reference to the entity's Transform component.
	/// </summary>
	private Transform _transform;

	/// <summary>
	/// A reference to this entity's parent.
	/// </summary>
	private Transform _parent;

	/// <summary>
	/// A reference to this entity's Rigidbody component.
	/// </summary>
	private Rigidbody _rigidbody;

	/// <summary>
	/// Parent's position on the last frame.
	/// </summary>
	private Vector3 _lastParentPosition;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_transform = transform;
		_parent = _transform.parent;
		_rigidbody = GetComponent<Rigidbody>();
		if (_rigidbody == null) {
			Debug.LogError("Error: No Rigidbody attached to the entity!");
			enabled = false;
		}
		if (_parent == null) {
			Debug.LogError("Error: This entity has no parent!");
			enabled = false;
		}
		else
			_lastParentPosition = _parent.position;
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Adds force to the object using the parent's displacement
		Vector3 displacement = _parent.position - _lastParentPosition;
		_rigidbody.AddForce(- forceFactor * displacement / Time.deltaTime);
		_lastParentPosition = _parent.position;
	}

	#endregion
}
