using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
/// Constains the information from an audio which will
/// be played on an AudioSource.
/// Usefull when a component has many AudioSources.
/// </summary>
[Serializable]
public class AudioInformation {

	/// <summary>
	/// Reference to the associated AudioSource.
	/// </summary>
	[HideInInspector]
	public AudioSource audioSource;

	/// <summary>
	/// The output mixer for the sound.
	/// </summary>
	public AudioMixerGroup output;

	/// <summary>
	/// Volume of the sounds played.
	/// </summary>
	[Range(0, 1)]
	public float volume = 1.0f;

	/// <summary>
	/// If the played sounds should be looped.
	/// </summary>
	public bool loop = false;

	/// <summary>
	/// If not enabled, the pitch won't be modified in any case.
	/// </summary>
	public bool modifyPitch = true;

	/// <summary>
	/// Modifies the pitch of the sound times this factor.
	/// </summary>
	[Range(0, 2)]
	public float pitchFactor = 1.0f;

	/// <summary>
	/// Plays the selected sound using this object's information.
	/// </summary>
	/// <param name="sound">The sound to play</param>
	/// <param name="pitch">The pitch the sound should be played with (optional)</param>
	public void PlayAudio(AudioClip sound, float pitch = 1.0f) {
		// Sets the sound
		audioSource.clip = sound;
		audioSource.outputAudioMixerGroup = output;

		// Sets the audio information
		audioSource.volume = volume;
		audioSource.loop = loop;

		// Sets the pitch factor
		if (modifyPitch) {
			audioSource.pitch = pitch;
			audioSource.pitch *= pitchFactor;
		}

		// Plays the sound
		audioSource.Play();
	}

	/// <summary>
	/// Stops the currently playing sound.
	/// </summary>
	public void StopAudio() {
		audioSource.Stop();
	}
}

/// <summary>
/// Extension class for AudioInformation usefull for AudioSources which
/// will only play one sound.
/// </summary>
[Serializable]
public class SingleSoundAudioInformation : AudioInformation {

	/// <summary>
	/// The sound to be played on the audio source.
	/// </summary>
	public AudioClip sound;
	
	/// <summary>
	/// Plays the stored sound using this object's information.
	/// </summary>
	/// <param name="pitch">The pitch the sound should be played with (optional)</param>
	public void PlayAudio(float pitch = 1.0f) {
		PlayAudio(sound, pitch);
	}
}