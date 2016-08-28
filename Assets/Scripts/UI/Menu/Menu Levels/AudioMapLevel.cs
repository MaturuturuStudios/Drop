using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioMapLevel : MonoBehaviour, MapLevelListener {
    /// <summary>
    /// Sound between worlds
    /// </summary>
    public SingleSoundAudioInformation audioBetweenWorlds;
    /// <summary>
    /// Sound between levels
    /// </summary>
    public SingleSoundAudioInformation audioBetweenLevels;
    /// <summary>
    /// Sound when selecting level
    /// </summary>
    public SingleSoundAudioInformation audioSelectionLevel;


    private MenuMapLevel3D map;
    /// <summary>
	/// Reference to the object's original AudioSource component which
	/// the values for the new ones will be copied from.
	/// </summary>
	private AudioSource _originalAudioSource;

    public void OnEnable() {
        map.AddListener(this);
    }

    public void OnDisable() {
        map.RemoveListener(this);
    }

    public void Awake() {
        map = GetComponent<MenuMapLevel3D>();

        // Retrieves the desired components
        _originalAudioSource = GetComponent<AudioSource>();

        // This' audio source is the original one
        if (audioBetweenWorlds != null)
            audioBetweenWorlds.audioSource = _originalAudioSource;

        // Creates a copy of the audio source for the other sounds
        if (audioBetweenLevels != null)
            audioBetweenLevels.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
        if (audioSelectionLevel != null)
            audioSelectionLevel.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
    }

    public void OnChangeWorld(int previousWorld, int newWorld) {
        if (audioBetweenWorlds == null) return;
        audioBetweenWorlds.PlayAudio();
    }

    public void OnChangeLevel(LevelInfo previous, LevelInfo actual) {
        if (audioBetweenLevels == null) return;
        audioBetweenLevels.PlayAudio();
    }

    public void OnSelectedLevel(LevelInfo selected) {
        if (audioSelectionLevel == null) return;
        audioSelectionLevel.PlayAudio();
    }
}
