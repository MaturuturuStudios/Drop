using UnityEngine;
using UnityEngine.EventSystems;

public class OnSelectInvokeAudio : MonoBehaviour, ISelectHandler {
    /// <summary>
    /// Kind of audio to play
    /// </summary>
    public AudioMenuType typeAudio;
    /// <summary>
    /// If true, the next time will not play the sound
    /// (and the next before yes)
    /// </summary>
    public bool passPlayAudio = false;
    /// <summary>
    /// Script who controls the menu's effects
    /// </summary>
    private AudioMenu _audioMenu;


    public void Awake() {
        _audioMenu = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<AudioMenu>();
    }

    public void OnSelect(BaseEventData eventData) {
        if (passPlayAudio) {
            passPlayAudio = !passPlayAudio;
            return;
        }
        _audioMenu.PlayEffect(typeAudio);
    }
}
