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
    /// Time for the sound end fading in.
    /// </summary>
    public float soundFadeInTime = 0.05f;

    /// <summary>
    /// Time for the sound end fading out.
    /// </summary>
    public float soundFadeOutTime = 0.5f;

	/// <summary>
	/// Reference to this object's AudioSource component.
	/// </summary>
	private AudioSource _audioSource;

	/// <summary>
	/// Reference to this object's Animator component.
	/// </summary>
	private Animator _animator;

    /// <summary>
    /// Target volume for the sound playing.
    /// </summary>
    private float _targetVolume;

	void Start() {
		// Retrieves the desired components
		_audioSource = GetComponent<AudioSource>();
		_animator = GetComponent<Animator>();

		// Subscribes to the events
		GetComponent<FollowPath>().AddListener(this);

		// Stops the particle effects
		groundParticleEffect.DisableParticleSystems();
		topParticleEffect.DisableParticleSystems();

        // Sets the sound volume
        _targetVolume = 0;
        _audioSource.volume = _targetVolume;
	}

    void Update() {
        // Sets the sound volume to the target one
        if (_audioSource.volume < _targetVolume) {
            // Increases the volume
            if (soundFadeInTime == 0)
                // Instantly
                _audioSource.volume = _targetVolume;
            else
                // Over soundFadeInTime seconds
                _audioSource.volume = Mathf.Min(_targetVolume, _audioSource.volume + Time.deltaTime / soundFadeInTime);
        }
        else {
            // Decreases the volume
            if (soundFadeOutTime == 0)
                // Instantly
                _audioSource.volume = _targetVolume;
            else
                // Over soundFadeOutTime seconds
                _audioSource.volume = Mathf.Max(_targetVolume, _audioSource.volume - Time.deltaTime / soundFadeOutTime);
        }
    }

	public void OnKeepMoving(Vector3 position, Vector3 destination, Vector3 velocity) {
		// Do nothing
	}

	public void OnStartMoving(Vector3 position, Vector3 destination, Vector3 velocity) {
        // Fades in the sound
        _targetVolume = 1;

		// Plays the particle effects
		groundParticleEffect.EnableParticleSystems();
		topParticleEffect.EnableParticleSystems();

		// Plays the animation
		_animator.SetBool("tremble", true);
	}

	public void OnStopMoving(Vector3 position) {
        // Fades out the sound
        _targetVolume = 0;

        // Plays the particle effects
        groundParticleEffect.DisableParticleSystems();
		topParticleEffect.DisableParticleSystems();

		// Plays the animation
		_animator.SetBool("tremble", false);
	}
}
