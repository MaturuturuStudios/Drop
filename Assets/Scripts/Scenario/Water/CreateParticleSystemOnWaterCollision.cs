using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Creates a particle system when a particle hits a collider.
/// The created particle system will match the collision normal.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class CreateParticleSystemOnWaterCollision : MonoBehaviour {

	/// <summary>
	/// The particle system to create.
	/// </summary>
	public GameObject particleSystemPrefab;

	/// <summary>
	/// The layers the particle will be spawned when colliding
	/// with.
	/// </summary>
	public LayerMask layerFilter;

	/// <summary>
	/// The duration of the particle system.
	/// </summary>
	public float duration = 3.0f;

	/// <summary>
	/// Reference to the collider
	/// </summary>
	private BoxCollider _collider;

	public void Awake() {
		// Retrieves the desired components
		_collider = GetComponent<BoxCollider>();
	}

	public void OnParticleCollision(GameObject other) {
		// Filters the layer
		if (((1 << other.layer) & layerFilter.value) == 0)
			return;

		ParticleSystem particleSystem = other.GetComponents<ParticleSystem>()[0];
		// Gets this frame's collisions
		int safeLength = particleSystem.maxParticles;
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[safeLength];
		int numParticles = particleSystem.GetParticles(particles);
        
		// Creates a particle effect instance for each collision
		for (int i = 0; i < numParticles; i++) {
			Vector3 position = other.transform.TransformPoint(particles[i].position);
			if (!_collider.bounds.Contains(position)) continue;
			PutEffect(position);
			//delete particle
			particles[i].remainingLifetime = 0;
		}

		//reasign array of particles
		particleSystem.SetParticles(particles, numParticles);
	}

	private void PutEffect(Vector3 position, int size=1) {
		GameObject effect = Instantiate(particleSystemPrefab, position, Quaternion.identity) as GameObject;

		ParticleSystem[] systems = effect.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem sys in systems) {
			// sys.randomSeed = (uint)UnityEngine.Random.Range(0, int.MaxValue);
			sys.Simulate(0, true, true);
			sys.Play();
		}

		effect.transform.localScale = size * Vector3.one;


		GameControllerTemporal.AddTemporal(effect);
		Destroy(effect, duration);
	}
}
