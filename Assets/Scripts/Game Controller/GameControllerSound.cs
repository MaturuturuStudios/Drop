using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Manages the sounds and plays the music of the game.
/// A copy of the object's AudioSource will be used as the
/// AudioSource for each music clip.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class GameControllerSound : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// The audio mixer used in the game.
	/// </summary>
	public AudioMixer audioMixer;

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

	#region Constants

	/// <summary>
	/// The minimum volume the mixer can output.
	/// </summary>
	private static readonly float MIN_VOLUME = -80;

	/// <summary>
	/// The maximum volume the mixer can output.
	/// </summary>
	private static readonly float MAX_VOLUME = 0;

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
			_audioSources[i] = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
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

		// Modifies the volume of each audio source
		bool atLeastOnePlaying = false;
		for (int i = 0; i < _audioSources.Length; i++) {
			// Determines the volume of the clip
			float targetVolume = 0.0f;
            if (!atLeastOnePlaying || musicClips[i].characterSize <= characterSize) {
				atLeastOnePlaying = true;
				targetVolume = musicVolume;
			}

			// Does the actual modification
			if (!instantaneus) {
				float transitionSpeed = 1.0f / musicTransitionTime;
                targetVolume = Mathf.MoveTowards(_audioSources[i].volume, targetVolume, transitionSpeed * Time.deltaTime);
            }
			_audioSources[i].volume = targetVolume;
		}
	}

	#region Mixer Methods

	/// <summary>
	/// Modifies the music's volume.
	/// </summary>
	/// <param name="volume">Thew new volume</param>
	public void SetMasterVolume(float volume) {
		volume = CalculateVolume(volume);
		audioMixer.SetFloat(AudioMixerParameters.MasterVolume, volume);
	}

	/// <summary>
	/// Modifies the music's volume.
	/// </summary>
	/// <param name="volume">Thew new volume</param>
	public void SetMusicVolume(float volume) {
		volume = CalculateVolume(volume);
		audioMixer.SetFloat(AudioMixerParameters.MusicVolume, volume);
	}

	/// <summary>
	/// Modifies the sound ambient's volume.
	/// </summary>
	/// <param name="volume">Thew new volume</param>
	public void SetAmbientVolume(float volume) {
		volume = CalculateVolume(volume);
		audioMixer.SetFloat(AudioMixerParameters.AmbientVolume, volume);
	}

	/// <summary>
	/// Modifies the sound effects's volume.
	/// </summary>
	/// <param name="volume">Thew new volume</param>
	public void SetEffectsVolume(float volume) {
		volume = CalculateVolume(volume);
		audioMixer.SetFloat(AudioMixerParameters.EffectsVolume, volume);
	}

	/// <summary>
	/// Takes a value between 0 and 1 and returns the right
	/// volume for that value.
	/// </summary>
	/// <param name="value">Normalized volume</param>
	/// <returns>Real volume</returns>
	private float CalculateVolume(float value) {
		// Since the fall in volume is exponential, uses the logarithm of the value
		value = Mathf.Lerp(1, 10, value);
		return Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, Mathf.Log10(value));
	}

	#endregion

	#endregion
}