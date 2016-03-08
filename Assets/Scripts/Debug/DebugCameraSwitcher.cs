using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class that stores the debug cameras and switches between them.
/// </summary>
public class DebugCameraSwitcher : MonoBehaviour {

	#region Private Attributes

	/// <summary>
	/// List with all the cameras. The main camera is at postion 0.
	/// </summary>
	private List<Camera> _cameras;

	/// <summary>
	/// Index of the current camera.
	/// </summary>
	private int _currentCameraIndex;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the beginning of the first frame
	/// this object is active on.
	/// Stores all the cameras.
	/// </summary>
	void Start () {
		// Adds the main camera to the list of cameras
		_cameras = new List<Camera>();
		Camera mainCamera = FindObjectOfType<MainCameraController>().GetComponent<Camera>();
        _cameras.Add(mainCamera);

		// Adds all the children cameras to the list
		Camera[] childrenCameras = GetComponentsInChildren<Camera>();
		foreach (Camera camera in childrenCameras)
			_cameras.Add(camera);

		// Sets the camera to the default one
		SetActiveCamera(0);
    }
	
	/// <summary>
	/// Swtiches to the next camera in the list.
	/// </summary>
    public void NextCamera()  {
        if (_cameras.Count > 1) {
            // Get next index
            int nextCamera = _currentCameraIndex + 1;
            if (nextCamera >= _cameras.Count)
                nextCamera = 0;

            // Switches the camera
            SetActiveCamera(nextCamera);
        }
    }

    /// <summary>
	/// Switches to the previous camera in the list.
	/// </summary>
    public void PreviousCamera() {
		if (_cameras.Count > 1) {
			// Get previous index
			int previousCamera = _currentCameraIndex - 1;
			if (previousCamera < 0)
				previousCamera = _cameras.Count - 1;

			// Switches the camera
			SetActiveCamera(previousCamera);
		}
    }

	/// <summary>
	/// Switches to the camera on the selected index.
	/// </summary>
	/// <param name="cameraIndex">Index of the camera to switch to</param>
	public void SetActiveCamera(int cameraIndex) {
		// Disables all the cameras
        for (int i = 0; i < _cameras.Count; ++i)
			if (i == cameraIndex) {
				_cameras[i].enabled = true;
				_cameras[i].gameObject.SetActive(true);
			}
			else {
				_cameras[i].enabled = false;
				_cameras[i].gameObject.SetActive(false);
			}

		// Stores the index
		_currentCameraIndex = cameraIndex;
	}

	/// <summary>
	/// Swtiches to the selected camera. If the camera is not already
	/// stored on the list, adds it.
	/// </summary>
	/// <param name="camera">The camera to switch to</param>
	public void SetActiveCamera(Camera camera) {
		// Retrieves the camera index
		int cameraIndex = _cameras.IndexOf(camera);

		// If the camera was not found, adds it to the list
		if (cameraIndex < 0)
			_cameras.Add(camera);

		// Switches to the camera. It's index it's the last one
		SetActiveCamera(_cameras.Count - 1);
	}

	#endregion
}
