using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsGraphics : SubOption {
    #region Public attributes
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
    public Toggle fullsceen;
    #endregion

    #region Private attributes
    /// <summary>
    /// List of resolutions
    /// </summary>
    private Resolution[] resolutions;
    /// <summary>
    /// The selected resolution
    /// </summary>
    private int selectedResolution=-1;
    /// <summary>
    /// What resolution is selected by the user
    /// </summary>
    private int changedResolution = -1;
    /// <summary>
    /// The selected quality
    /// </summary>
    private int selectedQuality = -1;
    /// <summary>
    /// Quality selected by the user
    /// </summary>
    private int changedQuality = -1;
    /// <summary>
    /// The selected antialiasing
    /// </summary>
    private int selectedAntiAliasing = -1;
    /// <summary>
    /// Antialiasing selected by the user
    /// </summary>
    private int changedAntiAliasing = -1;
    #endregion

    #region Method
    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    public override GameObject GetPanel() {
        return gameObject;
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
        if (fullsceen == null) {
            Debug.LogError("Dons't have the fullscreen option!");
        }

        //set the enabled resolutions
        FillResolutions();

        //get the quality
        FillQuality();

        //get the fullscreen
        fullsceen.isOn = Screen.fullScreen;

        //get the antialiasing
        selectedAntiAliasing = QualitySettings.antiAliasing / 2;
        antialiasing.value = selectedAntiAliasing;
        
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
        foreach (string name in names) {
            quality.options.Add(new Dropdown.OptionData(name));
        }

        //store the actual quality
        selectedQuality = QualitySettings.GetQualityLevel();
        quality.value = selectedQuality;
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
        resolutions = new Resolution[adecuateRatio.Count];
        //clear the dropdown and store the resolutions
        resolution.ClearOptions();
        int i = 0;
        foreach (Resolution res in adecuateRatio) {
            resolutions[i] = res;
            resolution.options.Add(new Dropdown.OptionData(res.width + "x" + res.height));
            //check if its the actual resolution
            if (res.width == Screen.width && res.height == Screen.height) {
                selectedResolution = i;
            }
            i++;
        }

        //if the actual resolution is not adecuate, get the first option
        if (selectedResolution < 0) {
            resolution.value = 0;
            ResolutionChanged(resolution);
        }

        //refresh text
        resolution.RefreshShownValue();
    }

    /// <summary>
    /// Detect changes to antialiasing
    /// </summary>
    /// <param name="target"></param>
    private void AntiAliasingChanged(Slider target) {

    }

    /// <summary>
    /// Detect changes to resolution dropdown
    /// </summary>
    /// <param name="target"></param>
    private void ResolutionChanged(Dropdown target) {
        changedResolution = target.value;
    }

    /// <summary>
    /// Detect changes to quality dropdown
    /// </summary>
    /// <param name="target"></param>
    private void QualityChanged(Dropdown target) {
        changedQuality = target.value;
    }
    #endregion

    /// <summary>
    /// When hitted, save the changes and apply them
    /// </summary>
    public void SaveChanges() {
        //change the resolution and fullscren
        if (Screen.fullScreen != fullsceen.isOn || selectedResolution != changedResolution) {
            //if no changes on resolution, set the previous one
            if (changedResolution < 0) changedResolution = selectedResolution;
            Screen.SetResolution(resolutions[changedResolution].width, resolutions[changedResolution].height, fullsceen.isOn);
            selectedResolution = changedResolution;
        }

        //change the quality
        if (selectedAntiAliasing != changedAntiAliasing || changedQuality != selectedQuality) {
            if (changedAntiAliasing < 0) changedAntiAliasing = selectedAntiAliasing;
            if (changedQuality < 0) changedQuality = selectedQuality;

            QualitySettings.antiAliasing = changedAntiAliasing * 2;
            QualitySettings.SetQualityLevel(changedQuality);

            selectedQuality = changedQuality;
            selectedAntiAliasing = changedAntiAliasing;
        }

        //recover the dirty state
        changedQuality = -1;
        changedResolution = -1;
        changedAntiAliasing = -1;
    }
    #endregion
}
