using UnityEngine;

public class StartApplication : MonoBehaviour {
    
	void Awake () {
        ApplyGraphics();
        ApplyVolume();
    }

    private void ApplyVolume() {

    }

    private void ApplyGraphics() {
        //set resolution
        int width = PlayerPrefs.GetInt(OptionsKey.GraphicsResolutionWidth, -1);
        int height = PlayerPrefs.GetInt(OptionsKey.GraphicsResolutionheight, -1);
        if (width != -1 && height != -1) {
            bool fullscreen = (PlayerPrefs.GetInt(OptionsKey.GraphicsFullscreen, 0) == 0) ? true : false;
            Screen.SetResolution(width, height, fullscreen);
        }

        //change the quality
        int quality=PlayerPrefs.GetInt(OptionsKey.GraphicsQuality, -1);
        if (quality >= 0) {
            QualitySettings.SetQualityLevel(quality);
        }
        
        //change antialiasing
        int antialiasing=PlayerPrefs.GetInt(OptionsKey.GraphicsAntialiasing, -1);
        if (antialiasing >= 0) {
            QualitySettings.antiAliasing = antialiasing;
        }

    }
}
