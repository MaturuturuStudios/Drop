using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GodJump : MonoBehaviour {

	void Start() {
		Collider collider = GetComponent<Collider>();
		if (collider == null)
			Debug.LogWarning("WARNING: No Collider Attached!");
		else if (!collider.isTrigger)
			Debug.LogWarning("WARNING: The Collider is not a Trigger!");
	}

	void OnTriggerEnter(Collider other) {
		CharacterControllerCustom ccc = other.GetComponent<CharacterControllerCustom>();
		if (ccc != null)
			ccc.GodJump = true;
	}

	void OnTriggerExit(Collider other) {
		CharacterControllerCustom ccc = other.GetComponent<CharacterControllerCustom>();
		if (ccc != null)
			ccc.GodJump = false;
	}
}