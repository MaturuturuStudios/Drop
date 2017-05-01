using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Creates an area where the gravity fill be changed, simulating
/// a place where the wind pushes the player.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WindTube : MonoBehaviour {

	#region Custom Structs

	/// <summary>
	/// Saved information from a particle system's state.
	/// </summary>
	struct ParticleSystemState {

		/// <summary>
		/// Emission rate.
		/// </summary>
		public ParticleSystem.MinMaxCurve rate;

		/// <summary>
		/// Initial color.
		/// </summary>
		public Color color;

		/// <summary>
		/// Initial speed.
		/// </summary>
		public float startSpeed;

		/// <summary>
		/// Velocity X.
		/// </summary>
		public ParticleSystem.MinMaxCurve velocityX;

		/// <summary>
		/// Velocity Y.
		/// </summary>
		public ParticleSystem.MinMaxCurve velocityY;

		/// <summary>
		/// Velocity Z.
		/// </summary>
		public ParticleSystem.MinMaxCurve velocityZ;

		/// <summary>
		/// Force X.
		/// </summary>
		public ParticleSystem.MinMaxCurve forceX;

		/// <summary>
		/// Force Y.
		/// </summary>
		public ParticleSystem.MinMaxCurve forceY;

		/// <summary>
		/// Force Z.
		/// </summary>
		public ParticleSystem.MinMaxCurve forceZ;
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// Defines the force of the wind.
	/// </summary>
	[Header("Wind")]
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
	public bool ignoreMass = true;

	/// <summary>
	/// Amount of movement friction the character will suffer while
	/// floating on the wind tube.
	/// </summary>
	[Range(0, 5)]
	public float characterFrictionFactor = 1.0f;

	/// <summary>
	/// The effect that will be used for the wind's shape.
	/// </summary>
	[Header("Effect")]
	public GameObject windEffect;

	/// <summary>
	/// Multiplies particle systems' base emission rate.
	/// </summary>
	public float particleEmissionRateMultiplier = 1.0f;

	/// <summary>
	/// Multiplies particle systems' base speed.
	/// </summary>
	public float particleSpeedMultiplier = 1.0f;

	/// <summary>
	/// Multiplies particle system0s base color.
	/// </summary>
	public Color particleColorMultiplier = Color.white;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the entity's transform component.
	/// </summary>
	private Transform _transform;

	/// <summary>
	/// Reference to the collider where the wind force will be applied.
	/// </summary>
	private BoxCollider _collider;

	/// <summary>
	/// Reference to the instantiated wind effect.
	/// </summary>
	private GameObject _windEffect;

	/// <summary>
	/// Reference to the particle systems attached to the component.
	/// </summary>
	private ParticleSystem[] _particleSystems;

	/// <summary>
	/// Saved information from the initial particle systems' states.
	/// </summary>
	private ParticleSystemState[] _initialParticleSystemStates;

	/// <summary>
	/// Base wind force the particle effect was calculated with.
	/// </summary>
	private const float _baseWindForce = 5.0f;

	/// <summary>
	/// List of listeners registered to this component's events.
	/// </summary>
	private List<WindTubeListener> _listeners = new List<WindTubeListener>();

	#endregion

	#region Methods

	/// <summary>
	/// Subscribes a listener to the components's events.
	/// Returns false if the listener was already subscribed.
	/// </summary>
	/// <param name="listener">The listener to subscribe</param>
	/// <returns>If the listener was successfully subscribed</returns>
	public bool AddListener(WindTubeListener listener) {
		if (_listeners.Contains(listener))
			return false;
		_listeners.Add(listener);
		return true;
	}

	/// <summary>
	/// Unsubscribes a listener to the components's events.
	/// Returns false if the listener wasn't subscribed yet.
	/// </summary>
	/// <param name="listener">The listener to unsubscribe</param>
	/// <returns>If the listener was successfully unsubscribed</returns>
	public bool RemoveListener(WindTubeListener listener) {
		if (!_listeners.Contains(listener))
			return false;
		_listeners.Remove(listener);
		return true;
	}

	/// <summary>
	/// Unity's method called when the entity is created.
	/// Recovers the desired componentes of the entity.
	/// </summary>
	public void Awake() {
		// Retrieves the components of the entity.
		_transform = transform;
		_collider = gameObject.GetComponent<BoxCollider>();

		// Creates the wind effect
		_windEffect = (GameObject) Instantiate(windEffect, _transform.position, _transform.rotation);
		_windEffect.transform.parent = _transform;

		// Retrieves all the particle systems
		_particleSystems = _windEffect.GetComponentsInChildren<ParticleSystem>();
		if (_particleSystems.Length == 0)
			Debug.LogError("Couldn't find children's Particle Systems!");

		// Saves the initial information from all the particle systems
		_initialParticleSystemStates = new ParticleSystemState[_particleSystems.Length];
		for (int i = 0; i < _particleSystems.Length; i++) {
			ParticleSystemState state = new ParticleSystemState();
			state.rate = _particleSystems[i].emission.rate;
			state.color = _particleSystems[i].startColor;
			state.velocityX = _particleSystems[i].velocityOverLifetime.x;
			state.velocityY = _particleSystems[i].velocityOverLifetime.y;
			state.velocityZ = _particleSystems[i].velocityOverLifetime.z;
			state.forceX = _particleSystems[i].forceOverLifetime.x;
			state.forceY = _particleSystems[i].forceOverLifetime.y;
			state.forceZ = _particleSystems[i].forceOverLifetime.z;
			_initialParticleSystemStates[i] = state;

			// _particleSystems[i].randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
			_particleSystems[i].Simulate(0, true, true);
			_particleSystems[i].Play();
		}

		// Disables the particle system's emission as they will be renabled if needed
		OnDisable();
	}

	/// <summary>
	/// Unity's method called when the script is enabled.
	/// </summary>
	public void OnEnable() {
		// Enables all particle systems' emissions
		foreach (ParticleSystem particleSystem in _particleSystems) {
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = true;
		}
	}

	/// <summary>
	/// Unity's method called when the script is disabled.
	/// </summary>
	public void OnDisable() {
		// Disables all particle systems' emissions
		foreach (ParticleSystem particleSystem in _particleSystems) {
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = false;
		}
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

		// Modifies each particle system to fit the collider
		for (int i = 0; i < _particleSystems.Length; i++) {
			// Modifies the emissor's shape
			ParticleSystem.ShapeModule shape = _particleSystems[i].shape;
			shape.box = new Vector3(width * _transform.lossyScale.x, 0, 1);

			// Modifies the particle color
			_particleSystems[i].startColor = _initialParticleSystemStates[i].color * particleColorMultiplier;

			// Modifies the particle speed
			float speedFactor = particleSpeedMultiplier * _transform.lossyScale.y * windForce / _baseWindForce;
			_particleSystems[i].startSpeed = _initialParticleSystemStates[i].startSpeed * speedFactor;

			// Modifies the particle velocity
			if (_particleSystems[i].velocityOverLifetime.enabled) {
				ParticleSystem.MinMaxCurve x = _initialParticleSystemStates[i].velocityX;
				x.constantMax *= speedFactor;
				x.constantMin *= speedFactor;
				x.curveMultiplier *= speedFactor;
				ParticleSystem.MinMaxCurve y = _initialParticleSystemStates[i].velocityY;
				y.constantMax *= speedFactor;
				y.constantMin *= speedFactor;
				y.curveMultiplier *= speedFactor;
				ParticleSystem.MinMaxCurve z = _initialParticleSystemStates[i].velocityZ;
				z.constantMax *= speedFactor;
				z.constantMin *= speedFactor;
				z.curveMultiplier *= speedFactor;
				ParticleSystem.VelocityOverLifetimeModule velocity = _particleSystems[i].velocityOverLifetime;
				velocity.x = x;
				velocity.y = y;
				velocity.z = z;
			}

			// Modifies the particle force
			if (_particleSystems[i].forceOverLifetime.enabled) {
				ParticleSystem.MinMaxCurve x = _initialParticleSystemStates[i].forceX;
				x.constantMax *= speedFactor;
				x.constantMin *= speedFactor;
				x.curveMultiplier *= speedFactor;
				ParticleSystem.MinMaxCurve y = _initialParticleSystemStates[i].forceY;
				y.constantMax *= speedFactor;
				y.constantMin *= speedFactor;
				y.curveMultiplier *= speedFactor;
				ParticleSystem.MinMaxCurve z = _initialParticleSystemStates[i].forceZ;
				z.constantMax *= speedFactor;
				z.constantMin *= speedFactor;
				z.curveMultiplier *= speedFactor;
				ParticleSystem.ForceOverLifetimeModule force = _particleSystems[i].forceOverLifetime;
				force.x = x;
				force.y = y;
				force.z = z;
			}

			// Modifies the emission rate according to the lifetime and width
			if (_particleSystems[i].emission.enabled) {
				float emissionRateFactor = width * particleEmissionRateMultiplier;
				ParticleSystem.MinMaxCurve rate = _initialParticleSystemStates[i].rate;
				rate.constantMax *= emissionRateFactor;
				rate.constantMin *= emissionRateFactor;
				rate.curveMultiplier *= emissionRateFactor;
				ParticleSystem.EmissionModule emission = _particleSystems[i].emission;
				emission.rate = rate;
			}

			// Modifies the particle lifetime according to their velocity and the volume's length
			float particleSpeed = _particleSystems[i].startSpeed;
			if (_particleSystems[i].velocityOverLifetime.enabled) {
				ParticleSystem.MinMaxCurve curve = _particleSystems[i].velocityOverLifetime.y;
				if (curve.mode == ParticleSystemCurveMode.Constant || curve.mode == ParticleSystemCurveMode.TwoConstants)
					particleSpeed = Mathf.Max(curve.constantMax, curve.constantMin);
				else
					particleSpeed = curve.curveMultiplier * curve.curveMax.Evaluate(0);
			}
			_particleSystems[i].startLifetime = length * _transform.lossyScale.y / particleSpeed;
		}
	}

	/// <summary>
	/// Unity's method called when a trigger enters it's volume.
	/// Notifies the player that it's on a wind tube.
	/// </summary>
	/// <param name="other">The collider entering the volume</param>
	public void OnTriggerEnter(Collider other) {
		if (!enabled)
			return;

		// Checks if the entity has a player component
		CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
		if (cccp != null)
			cccp.CurrentWindTube = this;

		// Notifies the listeners
		foreach (WindTubeListener listener in other.GetComponents<WindTubeListener>())
			listener.OnWindTubeEnter(this, other.gameObject);
		foreach (WindTubeListener listener in _listeners)
			listener.OnWindTubeEnter(this, other.gameObject);
	}

	/// <summary>
	/// Unity's method called when a trigger exits it's volume.
	/// Notifies the player that it's on a wind tube.
	/// </summary>
	/// <param name="other">The collider exiting the volume</param>
	public void OnTriggerExit(Collider other) {
		if (!enabled)
			return;

		// Checks if the entity has a player component
		CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
		if (cccp != null)
			cccp.CurrentWindTube = null;

		// Notifies the listeners
		foreach (WindTubeListener listener in other.GetComponents<WindTubeListener>())
			listener.OnWindTubeExit(this, other.gameObject);
		foreach (WindTubeListener listener in _listeners)
			listener.OnWindTubeExit(this, other.gameObject);
	}

	/// <summary>
	/// Unity's method called when a trigger stays on it's volume.
	/// Adds force to it's character controller or rigidbody on the
	/// desired direction.
	/// </summary>
	/// <param name="other">The collider staying on the volume</param>
	public void OnTriggerStay(Collider other) {
		if (!enabled) {
			CharacterControllerCustomPlayer cccp = other.gameObject.GetComponent<CharacterControllerCustomPlayer>();
			if (cccp != null)
				cccp.CurrentWindTube = null;
			return;
		}

		// Determines the force to add
		Vector3 force = _transform.up * windForce;
		ForceMode mode = ignoreMass ? ForceMode.Acceleration : ForceMode.Force;

		// Checks if the entity has a rigidbody
		Rigidbody rb = other.attachedRigidbody;
		if (rb != null) {
			rb.AddForce(force, mode);

			// If the gravity is ignored, substracts it
			if (ignoreGravity)
				rb.AddForceAtPosition(-Physics.gravity, _transform.position, ForceMode.Acceleration);

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

		// Notifies the listeners
		foreach (WindTubeListener listener in other.GetComponents<WindTubeListener>())
			listener.OnWindTubeStay(this, other.gameObject, force);
		foreach (WindTubeListener listener in _listeners)
			listener.OnWindTubeStay(this, other.gameObject, force);
	}

	/// <summary>
	/// Unity's method called by the editor in order to draw the gizmos.
	/// Draws the volume on the editor.
	/// </summary>
	public void OnDrawGizmos() {
		if (Application.isPlaying)
			return;

		// Sets the size and center of the collider
		BoxCollider collider = GetComponent<BoxCollider>();
		collider.size = new Vector3(width, length, 0.5f);
		collider.center = new Vector3(0, length / 2, 0);

		// Defines the color of the gizmo
		Color color = Color.cyan;
		color.a = 0.25f;
		Gizmos.color = color;

		// Draws the collider
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.up * collider.size.y / 2, collider.size);
	}

	/// <summary>
	/// Enables or disables the script.
	/// </summary>
	/// <param name="enabled">If the script should be enabled</param>
	public void SetEnabled(bool enabled)
	{
		this.enabled = enabled;
	}

	/// <summary>
	/// Allows to change the length parameter
	/// </summary>
	/// <param name="length">The new value for length</param>
	public void SetLength(float length)
	{

		this.length = length;
	}

	#endregion
}
