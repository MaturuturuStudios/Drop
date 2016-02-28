using UnityEngine;
using System.Collections.Generic;

public class CameraSwitcher : MonoBehaviour {
    
    //Camera mode
    public enum CameraMode { TRAVEL, GAME, DEBUG };
    public CameraMode cameraMode = CameraMode.GAME;

    public List<GameObject> camerasPool;

    //Cameras
    private int _currentCamera;

    void Start () {
        //Set the main camera active
        SetActiveCamera(0);
    }

    // Change to the next camera
    public void NextCamera()
    {
        if (camerasPool.Count > 1)
        {
            //Get next index
            int nextCamera = _currentCamera + 1;

            //Loop control
            if (nextCamera >= camerasPool.Count)
                nextCamera = 0;

            //Set Camera
            SetActiveCamera(nextCamera);
        }
    }

    // Change to the back camera
    public void BackCamera()
    {
        if (camerasPool.Count > 1)
        {
            //Get back index
            int backCamera = _currentCamera - 1;

            //Loop control
            if (backCamera  <= -1)
                backCamera = camerasPool.Count - 1;

            //Set Camera
            SetActiveCamera(backCamera);
        }
    }

    //Set camera mode
    public void SetCameraMode(CameraMode newMode)
    {
        cameraMode = newMode;
        if (cameraMode == CameraMode.GAME)
        {
            GameObject mainCamera = transform.Find("MainCamera").gameObject;
            int index = camerasPool.IndexOf(mainCamera);
            SetActiveCamera(index);
        }
    }

    //Set Active Camera
    private void SetActiveCamera(int cameraToActive)
    {
        _currentCamera = cameraToActive;
        for (int i = 0; i < camerasPool.Count; ++i)
            camerasPool[i].SetActive(false);
        camerasPool[cameraToActive].SetActive(true);
    }
}
