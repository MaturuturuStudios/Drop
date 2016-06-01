using UnityEngine;

/// <summary>
/// Manages and plays the music of the game.
/// A copy of the object's AudioSource will be used as the
/// AudioSource for each music clip.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GameControllerMusic : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Music clips played at different character's sizes.
	/// </summary>
	public AudioClipOnSize[] musicClips;

	/// <summary>
	/// Volume of the music played.
	/// </summary>
	[Range(0, 1)]
	public float musicVolume = 1.0f;

	/// <summary>
	/// Time taken for the music to change whith the character's
	/// size.
	/// </summary>
	public float musicTransitionTime = 1.0f;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the object's GameControllerIndependentControl
	/// component.
	/// </summary>
	private GameControllerIndependentControl _gcic;

	/// <summary>
	/// Reference to the object's original AudioSource component which
	/// the values for the new ones will be copied from.
	/// </summary>
	private AudioSource _originalAudioSource;

	/// <summary>
	/// Reference to the cloned AudioSources used by each music.
	/// </summary>
	private AudioSource[] _audioSources;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_originalAudioSource = GetComponent<AudioSource>();
		_gcic = GetComponent<GameControllerIndependentControl>();

		// Creates the audio sources
		_audioSources = new AudioSource[musicClips.Length];

		// Checks if there is at least one clip
		if (musicClips.Length == 0) {
			Debug.LogWarning("Error: No clips where specified. Disabling the music.");
			enabled = false;
			return;
		}

		// The first audio source is the original one
		_audioSources[0] = _originalAudioSource;
		_audioSources[0].clip = musicClips[0].clip;

		// Creates a copy of the audio source for the other music clip
		for (int i = 1; i < musicClips.Length; i++) {
			_audioSources[i] = CopyAudioSource(_originalAudioSource);
			_audioSources[i].clip = musicClips[i].clip;
        }
	}

	/// <summary>
	/// Unity's method called right after the componenet becomes
	/// enabled.
	/// </summary>
	void OnEnable() {
		// Adjusts the volume of each music clip
		ComputeMusicVolumes(true);

		// Plays all the audio sources
		foreach (AudioSource audioSource in _audioSources)
			audioSource.Play();
	}

	/// <summary>
	/// Unity's method called right after the componenet becomes
	/// disabled.
	/// </summary>
	void OnDisable() {
		// Stops all the audio sources
		foreach (AudioSource audioSource in _audioSources)
			audioSource.Stop();
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Adjusts the volume of each music clip
		ComputeMusicVolumes();
	}

	/// <summary>
	/// Adjusts the volume of each audio source using the currently
	/// controlled character's size.
	/// </summary>
	/// <param name="instantaneus">If there should be a transition changing the volume</param>
	private void ComputeMusicVolumes(bool instantaneus = false) {
		// Retrieves the currently controlled character's size
		int characterSize = 0;
		if (_gcic.currentCharacter != null)
			characterSize = _gcic.currentCharacter.GetComponent<CharacterSize>().GetSize();

		// Checks which clip should be playing
		int index = 0;
		for (int i = 0; i < musicClips.Length; i++)
			if (characterSize >= musicClips[i].characterSize)
				index = i;
			else
				break;

		// Modifies the volume of each audio source
		for (int i = 0; i < _audioSources.Length; i++) {
			// Determines the volume of the clip
			float targetVolume = 0.0f;
            if (i == index)
				targetVolume = musicVolume;

			// Does the actual modification
			if (!instantaneus) {
				float transitionSpeed = 1.0f / musicTransitionTime;
                targetVolume = Mathf.MoveTowards(_audioSources[i].volume, targetVolume, transitionSpeed * Time.deltaTime);
            }
			_audioSources[i].volume = targetVolume;
		}
	}

	/// <summary>
	/// Creates a new AudioSource from the information of the provided
	/// one and adds it to the object.
	/// </summary>
	/// <param name="originalAudioSource">The AudioSource to be copied</param>
	/// <returns>The new copied AudioSource</returns>
	private AudioSource CopyAudioSource(AudioSource originalAudioSource) {
		AudioSource AS = gameObject.AddComponent<AudioSource>();

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
		AS.loop = _originalAudioSource.loop;

		return AS;
	}

	/// <summary>
	/// Modifies the music's volume.
	/// </summary>
	/// <param name="volume">Thew new volume</param>
	public void SetVolume(float volume) {
		musicVolume = volume;
		ComputeMusicVolumes(true);
	}

	#endregion
}