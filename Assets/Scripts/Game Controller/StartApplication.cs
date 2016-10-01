using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartApplication : MonoBehaviour {
    public Texture2D cursorTexture;

    void Awake () {
        ApplyGraphics();
        ApplyMouse();
    }

    private void ApplyMouse() {
        Cursor.visible = false;
        CursorMode cursorMode = CursorMode.Auto;
        Vector2 hotSpot = Vector2.zero;
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    private void ApplyGraphics() {
        //set resolution
        SetAdecuateResolution();

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

    private void SetAdecuateResolution() {
        int width = PlayerPrefs.GetInt(OptionsKey.GraphicsResolutionWidth, -1);
        int height = PlayerPrefs.GetInt(OptionsKey.GraphicsResolutionheight, -1);

        if (width != -1 && height != -1) {
            bool fullscreen = (PlayerPrefs.GetInt(OptionsKey.GraphicsFullscreen, 0) == 0) ? true : false;
            Screen.SetResolution(width, height, fullscreen);

        } else {
            //this is the desired ratio
            int ratio = (int)((16.0f / 9.0f) * 100);

            //check if native resolution is good
            height = Screen.currentResolution.height;
            width = Screen.currentResolution.width;
            int actualRatio = (int)(((float)width / (float)height) * 100);

            //if not good, check the maximum
            if(actualRatio != ratio) {
                //get all resolutions
                Resolution[] allResolutions = Screen.resolutions;
                //for every resolution...
                bool found = false;
                for (int i = allResolutions.Length-1; i >= 0 && !found; i--) {
                    Resolution res = allResolutions[i];

                    //solve little bug of unity with higher but supported resolutions than actual native resolution
                    if(width < res.width || height < res.height) continue;
                    

                    //get the ratio
                    actualRatio = (int)(((float)res.width / (float)res.height) * 100);

                    //if desired, get it
                    if (actualRatio == ratio) {
                        found = true;

                        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
                        width = res.width;
                        height = res.height;
                        
                    }
                }
            }
            //store the first option
            PlayerPrefs.SetInt(OptionsKey.GraphicsResolutionWidth, width);
            PlayerPrefs.SetInt(OptionsKey.GraphicsResolutionheight, height);
            PlayerPrefs.Save();
        }
    }
}
