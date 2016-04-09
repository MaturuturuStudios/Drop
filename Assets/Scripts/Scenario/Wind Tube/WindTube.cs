using UnityEngine;
using System.Collections;

/// <summary>
/// Creates an area where the gravity fill be changed, simulating
/// a place where the wind pushes the player.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WindTube : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Defines the force of the wind.
	/// </summary>
	public float windForce = 25.0f;

	/// <summary>
	/// The width of the area where the wind force is applied.
	/// </summary>
	public float width = 1.0f;

	/// <summary>
	/// The height of the area where the wind force is applied.
	/// </summary>
	public float length = 5.0f;

	/// <summary>
	/// If enabled, gravity will not affect the entity while floating
	/// on the wind tube.
	/// </summary>
	public bool ignoreGravity = true;

	/// <summary>
	/// If enabled, the force will ignore the mass of the entity
	/// </summary>
	public bool ignoreMass = false;

	/// <summary>
	/// Defines the particle emission rate when the width of the tube
	/// is 1.
	/// </summary>
	public float baseParticleEmissionRate = 25.0f;

	/// <summary>
	/// Defines the particle initial speed when the wind force is 1.
	/// </summary>
	public float baseParticleInitialSpeed = 0.4f;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's transform component.
	/// </summary>
	private Transform _transform;

	/// <summary>
	/// Reference to the particle system attached to the component.
	/// </summary>
	private ParticleSystem _particleSystem;

	/// <summary>
	/// Reference to the collider where the wind force will be applied.
	/// </summary>
	private BoxCollider _collider;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
	public void Awake() {
		// Retrieves the components of the entity.
		_transform = transform;
		_particleSystem = GetComponentInChildren<ParticleSystem>();
		_collider = gameObject.GetComponent<BoxCollider>();
	}

	/// <summary>
	/// Unity's method called each frame.
	/// Modifies the size of the collider and 
	/// </summary>
	public void Update() {
		// Sets the size of the collider
		_collider.size = new Vector3(width, length, 0.5f);

		// Sets the center of the collider
		_collider.center = new Vector3(0, length / 2, 0);

		// Modifies the particle system to fit the collider. Modifies the shape
		ParticleSystem.ShapeModule shape = _particleSystem.shape;
		shape.box = new Vector3(width, 0, 0);

		// Modifies the particle velocity
		_particleSystem.startSpeed = baseParticleInitialSpeed * windForce;

		// Modifies the particle lifetime according to the velocity and  length
		_particleSystem.startLifetime = length / _particleSystem.startSpeed;

		// Modifies the emission rate according to the lifetime and width
		ParticleSystem.EmissionModule emission = _particleSystem.emission;
		ParticleSystem.MinMaxCurve rate = new ParticleSystem.MinMaxCurve();
		rate.mode = ParticleSystemCurveMode.Constant;
		float emissionRate = width * baseParticleEmissionRate / _particleSystem.startLifetime;
		rate.constantMax = emissionRate;
		rate.constantMin = emissionRate;
		emission.rate = rate;
	}

	/// <summary>
	/// Unity's method called when a trigger enters it's volume.
	/// Notifies the player that it's on a wind tube.
	/// </summary>
	/// <param name="other">The collider entering the volume</param>
	public void OnTriggerEnter(Collider other) {
		// Checks if the entity has a player component
		CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
		if (cccp != null)
			cccp.CurrentWindTube = this;
	}

	/// <summary>
	/// Unity's method called when a trigger exits it's volume.
	/// Notifies the player that it's on a wind tube.
	/// </summary>
	/// <param name="other">The collider exiting the volume</param>
	public void OnTriggerExit(Collider other) {
		// Checks if the entity has a player component
		CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
		if (cccp != null)
			cccp.CurrentWindTube = null;
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

			// If the gravity is ignored, substracts it
			if (ignoreGravity)
				rb.AddForce(-Physics.gravity, ForceMode.Acceleration);

			return;
		}

		// Checks if the entity has a custom character controller
		CharacterControllerCustom ccc = other.gameObject.GetComponent<CharacterControllerCustom>();
		if (ccc != null) {
			ccc.AddForce(force, mode);

			// If the gravity is ignored, substracts it
			if (ignoreGravity)
				ccc.AddForce(-ccc.Parameters.Gravity, ForceMode.Acceleration);

			return;
		}
	}

	/// <summary>
	/// Unity's method called by the editor in order to draw the gizmos.
	/// Draws the volume on the editor.
	/// </summary>
	public void OnDrawGizmos() {
		// Calls the configuration functions
		if (!Application.isPlaying) {
			Awake();
			Update();
		}

		// Defines the color of the gizmo
		Color color = Color.cyan;
		color.a = 0.25f;
		Gizmos.color = color;

		// Draws the cube
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.up * _collider.size.y / 2, _collider.size);
	}

	#endregion
}
