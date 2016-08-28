using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OptionsGraphics : InterfaceLanguage, SubOptionInterface {
    #region Public attributes
    /// <summary>
    /// Title of the panel
    /// </summary>
    public Text title;
    /// <summary>
    /// A DropDown in which resolutions will show
    /// </summary>
    public Dropdown resolution;
    /// <summary>
    /// A DropDown for the quality selection
    /// </summary>
    public Dropdown quality;
    /// <summary>
    /// A slider for the antialiasing
    /// </summary>
    public Slider antialiasing;
    /// <summary>
    /// A toggle for fullscreen
    /// </summary>
    public Toggle fullscreen;
    #endregion

    #region Private attributes
    /// <summary>
    /// List of resolutions
    /// </summary>
    private Resolution[] _resolutions;
    /// <summary>
    /// The selected resolution
    /// </summary>
    private int _selectedResolution=-1;
    /// <summary>
    /// What resolution is selected by the user
    /// </summary>
    private int _changedResolution = -1;
    /// <summary>
    /// The selected quality
    /// </summary>
    private int _selectedQuality = -1;
    /// <summary>
    /// Quality selected by the user
    /// </summary>
    private int _changedQuality = -1;
    /// <summary>
    /// The selected antialiasing
    /// </summary>
    private int _selectedAntiAliasing = -1;
    /// <summary>
    /// Antialiasing selected by the user
    /// </summary>
    private float _changedAntiAliasing = -1;
    #endregion

    #region Method
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
        //the first time don't play effect
        OnSelectInvokeAudio audio = resolution.gameObject.GetComponent<OnSelectInvokeAudio>();
        if (audio != null)
            audio.passPlayAudio = true;

        //select the option
        EventSystem.current.SetSelectedGameObject(resolution.gameObject);

        return true;
    }

    public void LoseFocus() {
    }

    public void Awake() {
        if (resolution == null) {
            Debug.LogError("Don't have the resolution option!");
        }
        if (quality == null) {
            Debug.LogError("Don't have the quality option!");
        }
        if (antialiasing == null) {
            Debug.LogError("Don't have the antialiasing option!");
        }
        if (fullscreen == null) {
            Debug.LogError("Dons't have the fullscreen option!");
        }

        //set the enabled resolutions
        FillResolutions();

        //get the quality
        FillQuality();

        //get the fullscreen
        int stored = PlayerPrefs.GetInt(OptionsKey.GraphicsFullscreen, 3);
        if (stored == 3) {
            fullscreen.isOn = Screen.fullScreen;
        } else {
            fullscreen.isOn = (stored == 0) ? true : false;
        }

        //get the antialiasing
        _selectedAntiAliasing = PlayerPrefs.GetInt(OptionsKey.GraphicsAntialiasing, QualitySettings.antiAliasing)/2;
        antialiasing.value = _selectedAntiAliasing;

    }

    public void Start() {
        resolution.onValueChanged.AddListener(delegate {
            ResolutionChanged(resolution);
        });

        quality.onValueChanged.AddListener(delegate {
            QualityChanged(quality);
        });

        antialiasing.onValueChanged.AddListener(delegate {
            AntiAliasingChanged(antialiasing);
        });
    }

    public void Destroy() {
        resolution.onValueChanged.RemoveAllListeners();
        quality.onValueChanged.RemoveAllListeners();
        antialiasing.onValueChanged.RemoveAllListeners();
    }

    public override void OnChangeLanguage(LanguageManager languageManager) {
        FillQuality();
    }

    #region Private Methods
    /// <summary>
    /// Get all available quality level and present it to the user
    /// </summary>
    private void FillQuality() {
        //Get the names
        string[] names;
        names = QualitySettings.names;

        //fill the dropdown
        quality.ClearOptions();
        LanguageManager languageManager = LanguageManager.Instance;
        foreach (string name in names) {
            string translated = languageManager.GetText(name);
            quality.options.Add(new Dropdown.OptionData(translated));
        }

        //store the actual quality
        _selectedQuality = PlayerPrefs.GetInt(OptionsKey.GraphicsQuality, QualitySettings.GetQualityLevel());
        quality.value = _selectedQuality;
        quality.RefreshShownValue();
    }

    /// <summary>
    /// Get all list of available an desirable resolutions and present it to the user
    /// </summary>
    private void FillResolutions() {
        //get all resolutions
        Resolution[] allResolutions = Screen.resolutions;
        //prepare a list with the proper resolutions
        List<Resolution> adecuateRatio = new List<Resolution>();

        //this is the desired ratio
        int ratio = (int)((16.0f / 9.0f) * 100);
        //for every resolution...
        foreach (Resolution res in allResolutions) {
            //get the ratio
            int actualRatio = (int)(((float)res.width / (float)res.height) * 100);
            //if desired, get it
            if (actualRatio == ratio) {
                adecuateRatio.Add(res);
            }
        }

        //create the array
        _resolutions = new Resolution[adecuateRatio.Count];
        //clear the dropdown and store the resolutions
        resolution.ClearOptions();
        int i = 0;
        foreach (Resolution res in adecuateRatio) {
            _resolutions[i] = res;
            resolution.options.Add(new Dropdown.OptionData(res.width + "x" + res.height));
            //check if its the actual resolution
            if (res.width == Screen.width && res.height == Screen.height) {
                _selectedResolution = i;
            }
            i++;
        }

        //if the actual resolution is not adecuate, get the first option
        if (_selectedResolution < 0) {
            resolution.value = 0;
            ResolutionChanged(resolution);
        } else {
            resolution.value = _selectedResolution;
        }

        //refresh text
        resolution.RefreshShownValue();
    }

    /// <summary>
    /// Detect changes to antialiasing
    /// </summary>
    /// <param name="target"></param>
    private void AntiAliasingChanged(Slider target) {
        _changedAntiAliasing = target.value;
    }

    /// <summary>
    /// Detect changes to resolution dropdown
    /// </summary>
    /// <param name="target"></param>
    private void ResolutionChanged(Dropdown target) {
        _changedResolution = target.value;
    }

    /// <summary>
    /// Detect changes to quality dropdown
    /// </summary>
    /// <param name="target"></param>
    private void QualityChanged(Dropdown target) {
        _changedQuality = target.value;
    }
    #endregion

    /// <summary>
    /// When hitted, save the changes and apply them
    /// </summary>
    public void SaveChanges() {
        //change the resolution and fullscren
        if (Screen.fullScreen != fullscreen.isOn || _selectedResolution != _changedResolution) {
            //if no changes on resolution, set the previous one
            if (_changedResolution < 0) _changedResolution = _selectedResolution;
            Screen.SetResolution(_resolutions[_changedResolution].width, _resolutions[_changedResolution].height, fullscreen.isOn);
            _selectedResolution = _changedResolution;
        }

        //change the quality
        if (_selectedAntiAliasing != _changedAntiAliasing || _changedQuality != _selectedQuality) {
            if (_changedAntiAliasing < 0) _changedAntiAliasing = _selectedAntiAliasing;
            if (_changedQuality < 0) _changedQuality = _selectedQuality;

            QualitySettings.antiAliasing = (int)(_changedAntiAliasing) * 2;
            QualitySettings.SetQualityLevel(_changedQuality);

            _selectedQuality = _changedQuality;
            _selectedAntiAliasing = (int)(_changedAntiAliasing);
        }

        //recover the dirty state
        _changedQuality = -1;
        _changedResolution = -1;
        _changedAntiAliasing = -1;

        StoreOptions();
    }

    private void StoreOptions() {
        PlayerPrefs.SetInt(OptionsKey.GraphicsQuality, QualitySettings.GetQualityLevel());

        int full = (fullscreen.isOn) ? 0 : 1;
        PlayerPrefs.SetInt(OptionsKey.GraphicsFullscreen, full);
        
        PlayerPrefs.SetInt(OptionsKey.GraphicsAntialiasing, _selectedAntiAliasing * 2);

        int width = _resolutions[_selectedResolution].width;
        int height = _resolutions[_selectedResolution].height;
        PlayerPrefs.SetInt(OptionsKey.GraphicsResolutionWidth, width);
        PlayerPrefs.SetInt(OptionsKey.GraphicsResolutionheight, height);

        PlayerPrefs.Save();
    }
    #endregion
}
