using UnityEngine;
using System.Collections;

[System.Serializable]
public enum AudioMenuType {
    ON_CLICK = 0, //make sure start at zero
    ON_SELECT,
    BACK_BUTTON
}

[RequireComponent(typeof(AudioSource))]
public class AudioMenu : MonoBehaviour {
    private static int numberAudioMenuType = 3;

    public SingleSoundAudioInformation onSelectButton;
    public SingleSoundAudioInformation onClick;
    public SingleSoundAudioInformation backButton;

    /// <summary>
	/// Reference to the object's original AudioSource component which
	/// the values for the new ones will be copied from.
	/// </summary>
	private AudioSource _originalAudioSource;

    public void Awake() {
        // Retrieves the desired components
        _originalAudioSource = GetComponent<AudioSource>();

        // The walk's audio source is the original one
        onSelectButton.audioSource = _originalAudioSource;

        // Creates a copy of the audio source for the other sounds
        onClick.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
        backButton.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
    }

    public void PlayEffect(AudioMenuType type) {
        switch (type) {
            case AudioMenuType.ON_CLICK:
                onClick.PlayAudio();
                break;
            case AudioMenuType.BACK_BUTTON:
                backButton.PlayAudio();
                break;
            case AudioMenuType.ON_SELECT:
                onSelectButton.PlayAudio();
                break;
        }
    }

    public void PlayEffect(int type) {
        if (type < numberAudioMenuType) {
            PlayEffect((AudioMenuType)type);
        }
    }
}
