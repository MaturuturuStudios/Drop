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
        SetActiveCamera(1);
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

    //Set Active Camera
    public void SetActiveCamera(int cameraToActive)
    {
        _currentCamera = cameraToActive;
        for (int i = 0; i < camerasPool.Count; ++i)
            camerasPool[i].SetActive(false);
        camerasPool[cameraToActive].SetActive(true);
    }

    //Set Active Camera
    public void SetCameraMode(CameraMode newMode)
    {
        if (newMode == CameraMode.DEBUG)
            if (cameraMode == CameraSwitcher.CameraMode.DEBUG)
                newMode = CameraSwitcher.CameraMode.GAME;

        if (newMode == CameraMode.TRAVEL)
            if (cameraMode == CameraSwitcher.CameraMode.TRAVEL)
                newMode = CameraSwitcher.CameraMode.GAME;

        if (newMode == CameraMode.GAME)
        {
            GameObject mainCamera = transform.Find("MainCamera").gameObject;
            int index = camerasPool.IndexOf(mainCamera);
            SetActiveCamera(index);
        }

        cameraMode = newMode;
    }


}
