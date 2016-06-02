using UnityEngine;
using System.Collections;

public class SpawnPlant : MonoBehaviour {

	public ParticleSystem particleGrow;

	public GameObject temporalObject;

	public void EnableObject() {
		//set the particles
		GameObject particleSystem = Instantiate(particleGrow.gameObject) as GameObject;
		Vector3 position = transform.position;
		particleSystem.GetComponent<Transform>().position = position;
		//play animation
		GetComponent<Animator>().SetTrigger("irrigate");
		//destroy system
		Destroy(particleSystem, particleGrow.startLifetime * 2);
		//grow the plant
		StartCoroutine(EnableTheObject());
	}

	private IEnumerator EnableTheObject() {
		yield return new WaitForSeconds(3);
		temporalObject.SetActive(true);
	}
}
