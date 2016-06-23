using UnityEngine;

/// <summary>
/// Creates a particle system when a particle hits a collider.
/// The created particle system will match the collision normal.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class CreateParticleSystemOnCollision : MonoBehaviour {

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
	/// If enabled, the created particle system will be a
	/// child of the original particle system.
	/// </summary>
	public bool setParent = true;

	/// <summary>
	/// Reference to the particle system managing this event.
	/// </summary>
	private ParticleSystem _particleSystem;

	/// <summary>
	/// Reference to the system's Transform component.
	/// </summary>
	private Transform _transform;

	void Awake() {
		// Retrieves the desired components
		_particleSystem = GetComponent<ParticleSystem>();
		_transform = _particleSystem.transform;
    }

	void OnParticleCollision(GameObject other) {
		// Filters the layer
		if (((1 << other.layer) & layerFilter.value) == 0)
			return;

		// Gets this frame's collisions
		int safeLength = _particleSystem.GetSafeCollisionEventSize();
		ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[safeLength];
		int numCollisionEvents = _particleSystem.GetCollisionEvents(other, collisionEvents);

		// Creates a particle effect instance for each collision
		for (int i = 0; i < numCollisionEvents; i++) {
			GameObject effect = Instantiate(particleSystemPrefab, collisionEvents[i].intersection, Quaternion.LookRotation(Vector3.forward, collisionEvents[i].normal)) as GameObject;
			if (setParent) {
				// Parents the effect. The scale needs to be reset to inherit the parent's
				effect.transform.parent = _transform;
				effect.transform.localScale = Vector3.one;
			}
			Destroy(effect, duration);
		}
	}
}
