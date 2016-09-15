using UnityEngine;

/// <summary>
/// Breaks a branch enabling the physiscs simulation on it.
/// Also plays a sound and a particle effect.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FollowPath))]
public class RockPilar : MonoBehaviour, FollowPathListener {

	/// <summary>
	/// The particle effect played at the base of the pilar.
	/// </summary>
	public GameObject groundParticleEffect;

	/// <summary>
	/// The particle effect played at the top of the pilar.
	/// </summary>
	public GameObject topParticleEffect;

	/// <summary>
	/// Reference to this object's AudioSource component.
	/// </summary>
	private AudioSource _audioSource;

	/// <summary>
	/// Reference to this object's Animator component.
	/// </summary>
	private Animator _animator;

	void Start() {
		// Retrieves the desired components
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();

		// Subscribes to the events
		GetComponent<FollowPath>().AddListener(this);

		// Stops the particle effects
		groundParticleEffect.DisableParticleSystems();
		topParticleEffect.DisableParticleSystems();
	}

	public void OnKeepMoving(Vector3 position, Vector3 destination, Vector3 velocity) {
		// Do nothing
	}

	public void OnStartMoving(Vector3 position, Vector3 destination, Vector3 velocity) {
		// Plays the sound
		_audioSource.Play();

		// Plays the particle effects
		groundParticleEffect.EnableParticleSystems();
		topParticleEffect.EnableParticleSystems();

		// Plays the animation
		_animator.SetBool("tremble", true);
	}

	public void OnStopMoving(Vector3 position) {
		// Stops the sound
		_audioSource.Stop();

		// Plays the particle effects
		groundParticleEffect.DisableParticleSystems();
		topParticleEffect.DisableParticleSystems();

		// Plays the animation
		_animator.SetBool("tremble", false);
	}
}
