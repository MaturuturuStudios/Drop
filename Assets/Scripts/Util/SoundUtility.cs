using UnityEngine;

/// <summary>
/// Utility class used to create differents sounds with a layer
/// of transparency on top of the AudioSource class.
/// </summary>
public static class SoundUtility {

	/// <summary>
	/// Plays a sound at the given position.
	/// </summary>
	/// <param name="clip">The sound to play</param>
	/// <param name="position">The position to play the sound</param>
	public static void PlaySoundAtPosition(AudioSource originalAudioSource, AudioClip clip, Vector3 position) {
		// Creates a new audio source
		GameObject container = new GameObject("Sound: " + clip.name);
		AudioSource audioSource = CopyAudioSource(originalAudioSource, container);
		GameControllerTemporal.AddTemporal(container);

		// Places the audio source and plays the sound
		audioSource.transform.position = position;
		audioSource.clip = clip;
		audioSource.Play();

		// Destroys the object
		Object.Destroy(container, clip.length);
	}

	/// <summary>
	/// Creates a new AudioSource from the information of the provided
	/// one and adds it to the object.
	/// </summary>
	/// <param name="originalAudioSource">The AudioSource to be copied</param>
	/// <param name="container">The object which the AudioSource will be attached to</param>
	/// <returns>The new copied AudioSource</returns>
	public static AudioSource CopyAudioSource(AudioSource originalAudioSource, GameObject container) {
		AudioSource AS = container.AddComponent<AudioSource>();

		AS.bypassEffects = originalAudioSource.bypassEffects;
		AS.bypassListenerEffects = originalAudioSource.bypassListenerEffects;
		AS.bypassReverbZones = originalAudioSource.bypassReverbZones;
		AS.dopplerLevel = originalAudioSource.dopplerLevel;
		AS.maxDistance = originalAudioSource.maxDistance;
		AS.minDistance = originalAudioSource.minDistance;
		AS.outputAudioMixerGroup = originalAudioSource.outputAudioMixerGroup;
		AS.panStereo = originalAudioSource.panStereo;
		AS.pitch = originalAudioSource.pitch;
		AS.playOnAwake = originalAudioSource.playOnAwake;
		AS.priority = originalAudioSource.priority;
		AS.reverbZoneMix = originalAudioSource.reverbZoneMix;
		AS.rolloffMode = originalAudioSource.rolloffMode;
		AS.spatialBlend = originalAudioSource.spatialBlend;
		AS.spatialize = originalAudioSource.spatialize;
		AS.spread = originalAudioSource.spread;
		AS.velocityUpdateMode = originalAudioSource.velocityUpdateMode;
		AS.volume = originalAudioSource.volume;

		return AS;
	}
}