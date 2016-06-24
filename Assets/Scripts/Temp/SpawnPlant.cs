using UnityEngine;
using System.Collections;

/// <summary>
/// Temporal.
/// </summary>
public class SpawnPlant : Irrigate {

	public GameObject temporalObject;

	public float timeToEnable = 3.0f;

	public GameObject particleGrow;

	public float effectDuration = 3.0f;

	protected override void OnIrrigate() {
		//set the particles
		GameObject system = Instantiate(particleGrow) as GameObject;
		system.transform.position = transform.position;
		system.transform.rotation = transform.rotation;
		//play animation
		foreach (Animator animator in GetComponentsInChildren<Animator>())
			animator.SetTrigger("irrigate");
        Destroy(system, effectDuration);
		//grow the plant
		StartCoroutine(EnableTheObject());
	}

	private IEnumerator EnableTheObject() {
		yield return new WaitForSeconds(timeToEnable);
        temporalObject.SetActive(true);
	}
}
