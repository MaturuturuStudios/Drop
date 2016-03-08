using UnityEngine;

/// <summary>
/// Controller for a free camera.
/// </summary>
public class FreeCameraController : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Movement speed of the camera.
	/// </summary>
	public float movementSpeed = 10;

	/// <summary>
	/// Rotation speed of the camera.
	/// </summary>
	public float rotationSpeed = 10;

	/// <summary>
	/// If enabled, the vertical rotation (X) will be inverted.
	/// </summary>
	public bool invertVerticalRotation = true;

	/// <summary>
	/// Amount of rotation in the Z axis.
	/// </summary>
	public float zRotationScale = 0.25f;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's transform component.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the beginning of the first frame
	/// this object is active.
	/// </summary>
	void Start() {
		// Retrieves the desired components
		_transform = transform;
    }

    /// <summary>
    /// Moves the camera.
    /// </summary>
    public void Move(Vector3 movement) {
		_transform.Translate(movement * movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Rotates the camera.
    /// </summary>
    public void Rotate(Vector3 rotation) {
		// Inverts the X rotation and scales the Z rotation
		if (invertVerticalRotation)
			rotation.x = -rotation.x;
		rotation.z *= zRotationScale;

		// Does the actual rotation
		_transform.localEulerAngles = _transform.localEulerAngles + rotation * rotationSpeed;
    }

	#endregion
}
