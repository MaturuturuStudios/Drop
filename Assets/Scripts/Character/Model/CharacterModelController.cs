using UnityEngine;

/// <summary>
/// Manages the orientation of the character's model.
/// </summary>
public class CharacterModelController : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// The angle this entity will face when walking
	/// sideways.
	/// </summary>
	public float rotationAngle = 60.0f;

	/// <summary>
	/// The angular velocity the character will use
	/// to turn.
	/// </summary>
	public float rotationAngularSpeed = 720.0f;

	/// <summary>
	/// Minimum speed to start rotating.
	/// </summary>
	public float speedTolerance = 0.1f;

	#endregion

	#region Private Attributes

	/// <summary>
	/// A reference to the CharacterControllerCustom on
	/// this entity's parent.
	/// </summary>
	private CharacterControllerCustom _ccc;

	/// <summary>
	/// A reference to the CharacterSize component on
	/// this entity's parent.
	/// </summary>
	private CharacterSize _characterSize;

	/// <summary>
	/// A reference to this entity's Transform component.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_ccc = GetComponentInParent<CharacterControllerCustom>();
		_characterSize = GetComponentInParent<CharacterSize>();
		_transform = transform;

		// Sets the right orientation to the object
		_transform.rotation = Quaternion.Euler(0, -rotationAngle, 0);
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Makes the object face the right direction
		float speed = _ccc.Velocity.x;
		if (Mathf.Abs(speed) >= speedTolerance) {
			float desiredAngle = speed > 0 ? -rotationAngle : rotationAngle;
			Quaternion desiredRotation = Quaternion.Euler(0, desiredAngle, 0);
			float turnSpeed = rotationAngularSpeed * Mathf.Abs(speed) / (_ccc.Parameters.maxSpeed * _characterSize.GetSize());
			_transform.rotation = Quaternion.RotateTowards(_transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
		}
	}

	#endregion
}
