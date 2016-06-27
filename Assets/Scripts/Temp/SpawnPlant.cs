using UnityEngine;

/// <summary>
/// Temporal.
/// </summary>
public class SpawnPlant : Irrigate {

	public GameObject plant;

	public float timeToEnable = 3.0f;

	public GameObject particleGrow;

	public GameObject interactEffect;

	public float effectDuration = 3.0f;

	protected override void OnIrrigate() {
		//set the particles
		GameObject system = Instantiate(particleGrow) as GameObject;
		system.transform.position = transform.position;
		system.transform.rotation = transform.rotation;

		// Plays the sound
		GetComponent<AudioSource>().Play();

		// Stops the interact effect
		float effectDuration = 0;
		foreach (ParticleSystem sys in interactEffect.GetComponentsInChildren<ParticleSystem>()) {
			ParticleSystem.EmissionModule emission = sys.emission;
			emission.enabled = false;
			effectDuration = Mathf.Max(effectDuration, sys.startLifetime);
        }
		Destroy(interactEffect, effectDuration);

		//play animation
		foreach (Animator animator in GetComponentsInChildren<Animator>())
			animator.SetTrigger("irrigate");
        Destroy(system, effectDuration);

		//grow the plant
		plant.SetActive(true);
		foreach (Animator animator in plant.GetComponentsInChildren<Animator>())
			animator.SetTrigger("irrigate");
	}
}
