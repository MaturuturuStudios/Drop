using UnityEngine;
using System.Collections;

public class SpawnPlant : Irrigate {

	public GameObject particleGrow;

	public GameObject temporalObject;

    private GameObject _system;

	protected override void OnIrrigate() {
		//set the particles
		_system = Instantiate(particleGrow) as GameObject;
        _system.transform.position = transform.position;
		//play animation
		GetComponent<Animator>().SetTrigger("irrigate");
        Destroy(_system, _system.GetComponent<ParticleSystem>().startLifetime);
		//grow the plant
		StartCoroutine(EnableTheObject());
	}

	private IEnumerator EnableTheObject() {
		yield return new WaitForSeconds(3);
        temporalObject.SetActive(true);
	}
}
