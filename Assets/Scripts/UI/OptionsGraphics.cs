using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsGraphics : SubOption {
    public Dropdown resolution;
    public Dropdown quality;
    public Toggle antialiasing;
    public Toggle fullsceen;

    private Resolution[] resolutions;
    private int selectedResolution=-1;

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
            if(res.width==Screen.width && res.height == Screen.height) {
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

        //get the fullscreen
        fullsceen.isOn = Screen.fullScreen;
    }

    public void Start() {
        resolution.onValueChanged.AddListener(delegate {
            ResolutionChanged(resolution);
        });
    }

    public void Destroy() {
        resolution.onValueChanged.RemoveAllListeners();
    }

    private void ResolutionChanged(Dropdown target) {
        if (selectedResolution != target.value) {
            Screen.SetResolution(resolutions[target.value].width, resolutions[target.value].height, fullsceen.isOn);
        }
    }
}
