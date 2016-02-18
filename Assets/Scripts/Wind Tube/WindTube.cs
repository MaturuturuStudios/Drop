using UnityEngine;
using System.Collections;

/// <summary>
/// Creates an area where the gravity fill be changed, simulating
/// a place where the wind pushes the player.
/// </summary>
public class WindTube : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Defines the force of the wind.
	/// </summary>
	public float windForce = 25;

	/// <summary>
	/// If enabled, gravity will not affect the entity while floating
	/// on the wind tube.
	/// </summary>
	public bool negateGravity = true;

	/// <summary>
	/// If enabled, the force will ignore the mass of the entity
	/// </summary>
	public bool ignoreMass = false;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's transform component.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
	public void Awake() {
		// Retrieves the components of the entity.
		_transform = transform;
	}

	/// <summary>
	/// Unity's method called when a trigger stays on it's volume.
	/// Adds force to it's character controller or rigidbody on the
	/// desired direction.
	/// </summary>
	/// <param name="other">The collider staying on the volume</param>
	public void OnTriggerStay(Collider other) {
		// Determines the force to add
		Vector3 force = _transform.up * windForce;
		ForceMode mode = ignoreMass ? ForceMode.Acceleration : ForceMode.Force;

		// Checks if the entity has a rigidbody
		Rigidbody rb = other.attachedRigidbody;
		if (rb != null) {
			rb.AddForce(force, mode);

			// If the gravity is negated, substracts it
			if (negateGravity)
				rb.AddForce(-Physics.gravity, ForceMode.Acceleration);

			return;
		}

		// Checks if the entity has a custom character controller
		CharacterControllerCustom ccc = other.gameObject.GetComponent<CharacterControllerCustom>();
		if (ccc != null) {
			ccc.AddForce(force, mode);

			// If the gravity is negated, substracts it
			if (negateGravity)
				ccc.AddForce(-ccc.Parameters.gravity, ForceMode.Acceleration);

			return;
		}
	}

	/// <summary>
	/// Unity's method called by the editor in order to draw the gizmos.
	/// Draws the volume on the editor.
	/// </summary>
	public void OnDrawGizmos() {
		// Defines the color of the gizmo
		Color color = Color.cyan;
		color.a = 0.25f;
		Gizmos.color = color;

		// Draws the cube
		BoxCollider collider = GetComponent<BoxCollider>();
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.up * collider.size.y / 2, collider.size);
	}

	#endregion
}
