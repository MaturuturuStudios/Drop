using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controller for main camera.
/// </summary>
public class MainCameraController : MonoBehaviour {

    #region Public Attributes


    /// <summary>
    /// Distance from player to camera X position will be allways the same
    /// </summary>
    public Vector3 offset = new Vector3(0F, 2F, -15F);


    /// <summary>
    /// Camera movement smooth on XY
    /// </summary>
    public float smooth = 2.0f;


    /// <summary>
    /// Camera movement smooth on XY
    /// </summary>
    public float zSmooth = 1.0f;


    /// <summary>
    /// Enable/Disable Look at player liberty when bound reached
    /// </summary>
    public bool lookAtFixedOnBounds = true;


    /// <summary>
    /// Look at movement smooth
    /// </summary>
    public float lookAtSmooth = 2.5f;


    /// <summary>
    /// Look arround movement smooth
    /// </summary>
    public float lookArroundSmooth = 10F;


    /// <summary>
    /// Bound of camera liberty movement area
    /// </summary>
    public Boundary bounds;
    //  Bounds Class
    [System.Serializable]
    public class Boundary {
        public float top = 100.0f;
        public float bottom = -100.0f;
        public float left = -100.0f;
        public float right = 100.0f;
    }


    #endregion

    #region Private Attributes


    /// <summary>
    /// Reference to the independent control component from the scene's game controller.
    /// </summary>
    private GameControllerIndependentControl _independentControl;


    /// <summary>
    /// Target of the camera, it use to be the player
    /// </summary>
    private GameObject target;


    /// <summary>
    /// Bounds exceded controll on X
    /// </summary>
    private float excededX = 0F;


    /// <summary>
    /// Bounds exceded controll on Y
    /// </summary>
    private float excededY = 0F;

    /// <summary>
    /// Offset Reference
    /// </summary>
    private Vector3 _offset;


    /// <summary>
    /// Movement position control
    /// </summary>
    private Vector3 _lastObjective;


    /// <summary>
    /// Look Arround Offset
    /// </summary>
    private Vector3 _lookArroundOffset;


    /// <summary>
    /// Reference to current caracter size
    /// </summary>
    private float _dropSize;


    /// <summary>
    /// Check for camera in locked area
    /// </summary>
    private bool _cameraLocked = false;


    /// <summary>
    /// Check for camera in locked area
    /// </summary>
    private Vector3 _lockPosition;


    #endregion

    #region Methods

    /// <summary>
    /// Unity's method called when this entity is created, even if it is disabled.
    /// </summary>
    void Awake() {

        // Looks for the independent controller component
        _independentControl = FindObjectOfType<GameControllerIndependentControl>();

        // Sets the camera's target to the current character
        SetObjective(_independentControl.currentCharacter);
    }
	

    /// <summary>
    /// Unity's method called on start script only one time
    /// </summary>
    void Start() {

        //Calculate offset
        _offset = new Vector3(offset.x, offset.y, offset.z);

        //Set references
        _lastObjective = target.transform.position;
    }


    /// <summary>
    /// Update the state of the camera
    /// </summary>
    void Update() {

        // Get drop size
        _dropSize = target.GetComponent<CharacterSize>().GetSize();

        // Update ofset and boundary depends of the size
        if (!target.GetComponent<CharacterControllerCustom>().State.IsFlying)
            _offset = new Vector3(_dropSize * offset.x, _dropSize * offset.y, _dropSize * offset.z);

    }


    /// <summary>
    /// Update the camera phisics
    /// </summary>
    void FixedUpdate() {

        //Camera Movement
        MoveCamera();

        //LookAt player
        LookAt();

    }


    /// <summary>
    /// Move the camera to offset position of the player gradually
    /// </summary>
    private void MoveCamera() {
		// Calculate destination
        Vector3 destination = target.transform.position + _offset;

        if (_cameraLocked)
            // Lock camera
            destination = _lockPosition;
        else
            // Add Loook around offset
            destination += _lookArroundOffset;

        // Reset bounds exceded to recalculate
        excededX = excededY = 0;
        
        // Calculate if it is out of bounds
        float cameraRealBound = Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg) * (Mathf.Abs(_offset.z));

        // If bottom bound exceded
        if (destination.x < bounds.left + cameraRealBound)
            excededX = destination.x = bounds.left + cameraRealBound;

        // If top bound exceded
        else if (destination.x > bounds.right - cameraRealBound) {
            excededX = destination.x = bounds.right - cameraRealBound;

            // If bottom bound exceded
            if (excededX < bounds.left + cameraRealBound)
                excededX = destination.x = bounds.left + cameraRealBound;
        }

        // If left bound exeded
        if (destination.y < bounds.bottom + offset.y + (cameraRealBound * 9 / 16))
            excededY = destination.y = bounds.bottom + offset.y  + (cameraRealBound * 9 / 16);

        // If right bound exeded
        else if (destination.y > bounds.top - (cameraRealBound * 9 / 16)) {
            excededY = destination.y = bounds.top - (cameraRealBound * 9 / 16);

            // If left bound exeded
            if (excededY < bounds.bottom + offset.y + (cameraRealBound * 9 / 16))
                excededY = destination.y = bounds.bottom + offset.y + (cameraRealBound * 9 / 16);
        }

		// Calculate next position
		Vector3 newPosition;
        newPosition = Vector2.Lerp(transform.position, destination, Time.deltaTime * smooth);
        newPosition.z = Mathf.Lerp(transform.position.z, destination.z, Time.deltaTime * zSmooth);

        // Set the position to the camera
        transform.position = newPosition;
    }

    /// <summary>
    /// Makes the camera look to the player's position gradually
    /// </summary>
    private void LookAt() {
		// Calculate objective of the camera
        Vector3 destination = target.transform.position;

        // Add Loook around offset
        destination += _lookArroundOffset;

        // Lock area control
        if (_cameraLocked) {
            Vector3 destinationOnZ = _lockPosition;
            destinationOnZ.z = 0;
            destination = destinationOnZ;
        }


        // If there isn't liberty looking at, block it
        if (lookAtFixedOnBounds && excededX != 0) 
            destination.x = excededX;
        if (lookAtFixedOnBounds && excededY != 0) 
            destination.y = excededY - (offset.y * (_dropSize) );


        // Calculate the look at position of the camera
        destination = Vector3.Lerp(_lastObjective, destination, Time.deltaTime * lookAtSmooth);

		// Set the look at attribute
        transform.LookAt(destination);

		// Save the last position for future calculations
        _lastObjective = destination;
    }


    /// <summary>
    /// Set the objective of the camera
    /// </summary>
    /// <param name="objective">GameObject who is the target of the camera</param>
    public void SetObjective(GameObject objective) {
        target = objective;
    }


    /// <summary>
    /// Set the look arround offset
    /// </summary>
    public void LookArround(float OffsetX, float OffsetY) {

        // Setting look arround values depending of the input
        _lookArroundOffset = new Vector3(OffsetX, OffsetY, 0F) * lookArroundSmooth * _dropSize;
    }


    /// <summary>
    /// Locks the camera in a position
    /// </summary>
    /// <param name="position"> Area that camera will see</param>
    public void LockCamera(Rect area) {

        // Setting lock state
        _cameraLocked = true;

        float cameraZPos = (area.height) / Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg);

        _lockPosition = new Vector3(area.x, area.y, -cameraZPos);

    }


    /// <summary>
    /// Unfix camera when a player leaves a CameraLockArea
    /// </summary>
    public void UnlockCamera() {

        // Setting look arround values depending of the input
        _cameraLocked = false;
    }


    /// <summary>
    /// Draws the movement bounds and the camera frustum on the editor
    /// </summary>
    public void OnDrawGizmosSelected() {

        // Draw camera bounds
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.top, 0), new Vector3(bounds.right, bounds.top, 0));
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.top, 0), new Vector3(bounds.left, bounds.bottom, 0));
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.bottom, 0), new Vector3(bounds.right, bounds.bottom, 0));
        Gizmos.DrawLine(new Vector3(bounds.right, bounds.bottom, 0), new Vector3(bounds.right, bounds.top, 0));

        // Draw camera frustum
        Camera camera = GetComponent<Camera>();
        Gizmos.color = Color.yellow;
        Gizmos.matrix = camera.transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
    }

    #endregion
}
