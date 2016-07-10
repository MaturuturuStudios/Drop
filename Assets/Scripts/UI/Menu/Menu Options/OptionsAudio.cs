using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsAudio : MonoBehaviour, SubOptionInterface {
    #region Public attributes
    /// <summary>
    /// Title of the panel
    /// </summary>
    public Text title;
    /// <summary>
    /// A slider for the master
    /// </summary>
    public Slider master;
    /// <summary>
    /// A slider for the master
    /// </summary>
    public Slider music;
    /// <summary>
    /// A slider for the master
    /// </summary>
    public Slider ambient;
    /// <summary>
    /// A slider for the master
    /// </summary>
    public Slider effects;
    #endregion

    #region Methods
    public void Start() {
        SetStoredOptions();
    }
    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    public GameObject GetPanel() {
        return gameObject;
    }

    /// <summary>
    /// Get the focus to the panel
    /// </summary>
    public bool GetFocus() {
        //select the option
        EventSystem.current.SetSelectedGameObject(master.gameObject);
        if (title != null) {
            //mark title as panel selected
            title.color = MenuNavigator.titleColor;
        }
        return true;
    }

    public void LoseFocus() {
        if (title != null) {
            title.color = Color.white;
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void Awake() {
        if (master == null) {
            Debug.LogError("Don't have the master option!");
        }
        if (music == null) {
            Debug.LogError("Don't have the music option!");
        }
        if (ambient == null) {
            Debug.LogError("Don't have the ambient option!");
        }
        if (effects== null) {
            Debug.LogError("Don't have the effects option!");
        }
    }


    /// <summary>
    /// When hitted, save the changes and apply them
    /// </summary>
    public void SaveChanges() {
        //write changes to player prefs
        PlayerPrefs.SetFloat(OptionsKey.AudioMaster, master.value);
        PlayerPrefs.SetFloat(OptionsKey.AudioMusic, music.value);
        PlayerPrefs.SetFloat(OptionsKey.AudioAmbient, ambient.value);
        PlayerPrefs.SetFloat(OptionsKey.AudioEffects, effects.value);
        PlayerPrefs.Save();
        //come back to menu options
    }

    private void SetStoredOptions() {
        master.value = PlayerPrefs.GetFloat(OptionsKey.AudioMaster, 1);
        music.value = PlayerPrefs.GetFloat(OptionsKey.AudioMusic, 1);
        ambient.value = PlayerPrefs.GetFloat(OptionsKey.AudioAmbient, 1);
        effects.value = PlayerPrefs.GetFloat(OptionsKey.AudioEffects, 1);
    }
    #endregion
}
