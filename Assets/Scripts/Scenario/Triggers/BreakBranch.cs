using UnityEngine;

/// <summary>
/// Breaks a branch enabling the physiscs simulation on it.
/// Also plays a sound and a particle effect.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class BreakBranch : MonoBehaviour {

	/// <summary>
	/// The particle effect played when the branch is broken.
	/// </summary>
	public EffectInformation breakEffect;

	/// <summary>
	/// The position and rotation for the particle effect.
	/// </summary>
	public Transform particleEffectPosition;
	
	/// <summary>
	/// Breaks the branch, playing the sound and the particle effect.
	/// </summary>
	public void Break() {
		// Plays the sound
		GetComponent<AudioSource>().Play();

		// Plays the particle effect
		breakEffect.PlayEffect(particleEffectPosition.position, particleEffectPosition.rotation);

		// Sets the Rigidbody's kinematic flag
		GetComponent<Rigidbody>().isKinematic = false;
	}
}
