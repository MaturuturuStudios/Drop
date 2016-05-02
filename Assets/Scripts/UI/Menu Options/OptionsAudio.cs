using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsAudio : MonoBehaviour, SubOptionInterface {
    #region Public attributes
    /// <summary>
    /// A DropDown in which resolutions will show
    /// </summary>
    public Slider master;
    /// <summary>
    /// A DropDown for the quality selection
    /// </summary>
    public Slider music;
    /// <summary>
    /// A slider for the antialiasing
    /// </summary>
    public Slider ambient;
    /// <summary>
    /// A toggle for fullscreen
    /// </summary>
    public Slider effects;
    #endregion

    #region Private attributes
    private float _previousMaster;
    #endregion

    #region Methods
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
    public void GetFocus() {
        //select the option
        EventSystem.current.SetSelectedGameObject(master.gameObject);
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
        if (effects == null) {
            Debug.LogError("Dons't have the effects option!");
        }

        //get the music configuration


        _previousMaster = master.value;
    }

    public void Start() {
        master.onValueChanged.AddListener(delegate {
            MasterChange(master);
        });

        music.onValueChanged.AddListener(delegate {
            MusicChange(music);
        });

        ambient.onValueChanged.AddListener(delegate {
            AmbientChange(ambient);
        });

        effects.onValueChanged.AddListener(delegate {
            EffectsChange(effects);
        });
    }

    private void MasterChange(Slider target) {
        Debug.Log("Changed master");
        float difference = target.value - _previousMaster;
        _previousMaster = target.value;

        music.value += difference;
        ambient.value += difference;
        effects.value += difference;
    }

    private void MusicChange(Slider target) {
        Debug.Log("Changed music");
    }

    private void AmbientChange(Slider target) {
        Debug.Log("Changed ambient");
    }

    private void EffectsChange(Slider target) {
        Debug.Log("Changed effects");
    }


    /// <summary>
    /// When hitted, save the changes and apply them
    /// </summary>
    public void SaveChanges() {
        //write changes to file
        //come back to menu options
    }
    #endregion
}
