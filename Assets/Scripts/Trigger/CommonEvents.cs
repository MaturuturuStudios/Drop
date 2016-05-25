using UnityEngine;
using System.Collections;

/// <summary>
/// Provides a set of methods usefull for triggers to activate.
/// </summary>
public class CommonEvents : MonoBehaviour {

	/// <summary>
	/// Destroys an object.
	/// </summary>
	/// <param name="o">The object to destroy</param>
	public new void Destroy(Object o) {
		Object.Destroy(o);
	}

	/// <summary>
	/// Destroys the object.
	/// </summary>
	/// <param name="delay">A delay beetween this object is disabled and destroyed</param>
	public IEnumerator DestroySelf(float delay = 0.0f) {
		foreach (Renderer renderer in GetComponents<Renderer>())
			renderer.enabled = false;
		if (delay > 0.0f)
			yield return new WaitForSeconds(delay);
		Destroy(gameObject);
	}

	/// <summary>
	/// Plays the sound on the object's audio source component..
	/// </summary>
	/// <param name="sound">The new sound</param>
	public void PlayAudioSource() {
		GetComponent<AudioSource>().Play();
	}
}
