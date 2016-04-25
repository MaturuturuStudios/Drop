using UnityEngine;

public class CharacterModelController : MonoBehaviour {

	public Transform hatRoot;

	public float rotationAngle = 45.0f;

	public float rotationAngularSpeed = 720.0f;

	public float speedTolerance = 0.1f;

	private CharacterControllerCustom _ccc;

	private CharacterSize _characterSize;

	private Transform _transform;

	void Awake() {
		// Retrieves the desired components
		_ccc = GetComponentInParent<CharacterControllerCustom>();
		_characterSize = GetComponentInParent<CharacterSize>();
		_transform = transform;

		// Sets the right orientation to the object
		_transform.rotation = Quaternion.Euler(0, -rotationAngle, 0);
	}

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
}
