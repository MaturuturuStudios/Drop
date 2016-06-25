using UnityEngine;

/// <summary>
/// Listener to the water events which plays a sound whenever a
/// character enters or leaves it.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(WaterRepulsion))]
public class WaterRepulsionSound : MonoBehaviour, WaterRepulsionListener {

	/// <summary>
	/// Sound clip played whenever a character enters the water.
	/// </summary>
	public AudioClip enterSound;

	/// <summary>
	/// Sound clip played whenever a character exits the water.
	/// </summary>
	public AudioClip exitSound;

	/// <summary>
	/// Reference to the entity's water repulsion component.
	/// </summary>
	private WaterRepulsion _waterRepulsion;

	/// <summary>
	/// Reference to the original AudioSource component which will be
	/// cloned to play every sound.
	/// </summary>
	private AudioSource _originalAudioSource;

	void Awake() {
		// Retrieves the desired components
		_waterRepulsion = GetComponent<WaterRepulsion>();
		_originalAudioSource = GetComponent<AudioSource>();
	}

	void Start() {
		// Subscribes itself to the water's events
		_waterRepulsion.AddListener(this);
	}

	public void OnWaterEnter(WaterRepulsion water, GameObject character) {
		PlaySoundAtPosition(enterSound, character.transform.position);
	}

	public void OnWaterExit(WaterRepulsion water, GameObject character, Vector3 repulsionVelocity) {
		PlaySoundAtPosition(exitSound, character.transform.position);
	}

	/// <summary>
	/// Plays a sound at the given position.
	/// </summary>
	/// <param name="clip">The sound to play</param>
	/// <param name="position">The position to play the sound</param>
	private void PlaySoundAtPosition(AudioClip clip, Vector3 position) {
		// Creates a new audio source
		GameObject container = new GameObject("Sound: " + clip.name);
		AudioSource audioSource = CopyAudioSource(_originalAudioSource, container);

		// Places the audio source and plays the sound
		audioSource.transform.position = position;
		audioSource.clip = clip;
		audioSource.Play();

		// Destroys the object
		Destroy(container, clip.length);
	}
	
	/// <summary>
	 /// Creates a new AudioSource from the information of the provided
	 /// one and adds it to the object.
	 /// </summary>
	 /// <param name="originalAudioSource">The AudioSource to be copied</param>
	 /// <param name="container">The object which the AudioSource will be attached to</param>
	 /// <returns>The new copied AudioSource</returns>
	private AudioSource CopyAudioSource(AudioSource originalAudioSource, GameObject container) {
		AudioSource AS = container.AddComponent<AudioSource>();

		AS.bypassEffects = _originalAudioSource.bypassEffects;
		AS.bypassListenerEffects = _originalAudioSource.bypassListenerEffects;
		AS.bypassReverbZones = _originalAudioSource.bypassReverbZones;
		AS.dopplerLevel = _originalAudioSource.dopplerLevel;
		AS.maxDistance = _originalAudioSource.maxDistance;
		AS.minDistance = _originalAudioSource.minDistance;
		AS.outputAudioMixerGroup = _originalAudioSource.outputAudioMixerGroup;
		AS.panStereo = _originalAudioSource.panStereo;
		AS.pitch = _originalAudioSource.pitch;
		AS.playOnAwake = _originalAudioSource.playOnAwake;
		AS.priority = _originalAudioSource.priority;
		AS.reverbZoneMix = _originalAudioSource.reverbZoneMix;
		AS.rolloffMode = _originalAudioSource.rolloffMode;
		AS.spatialBlend = _originalAudioSource.spatialBlend;
		AS.spatialize = _originalAudioSource.spatialize;
		AS.spread = _originalAudioSource.spread;
		AS.velocityUpdateMode = _originalAudioSource.velocityUpdateMode;
		AS.volume = _originalAudioSource.volume;

		return AS;
	}
}
