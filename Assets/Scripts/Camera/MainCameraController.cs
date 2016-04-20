using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controller for main camera.
/// </summary>
public class MainCameraController : MonoBehaviour {

    #region Public Attributes
    // Target of the camera, it use to be the player
    private GameObject target;

    /// <summary>
    /// Distance from player to camera X position will be allways the same
    /// </summary>
    //Offset Variable
    public Vector3 offset = new Vector3(0F, 2F, -15F);
    //Offset Reference
    private Vector3 _offset;
	
    /// <summary>
    /// Camera movement configuration options
    /// </summary>
    public Movement movement;
    [System.Serializable]
	//  Movement Class
    public class Movement {
		//Smooth on XY movement
        public float smooth = 2.0f;
		//Smooth on Z movement
        public float zSmooth = 1.0f;
    }
    // Movement position control
    private Vector3 _lastObjective;
	
    /// <summary>
    /// Camera look at configuration options
    /// </summary>
    public LookingAt lookAt;
    [System.Serializable]
	//  Movement Class
    public class LookingAt {
		// Enable/Disable Look at player liberty when bound reached
		public bool lookAtFixedOnBounds = true;
		//Look at movement smooth
        public float lookAtSmooth = 2.5f;
		//Look arround movement smooth
		public float lookArroundSmooth = 10F;
    }
    //Look Arround Offset
    private Vector3 _lookArroundOffset;

    /// <summary>
    /// Camera liberty movement bounds
    /// </summary>
    //	Bounds Attributes
    public Boundary bounds;
    //  Bounds Class
    [System.Serializable]
    public class Boundary {
        public float top = 100.0f;
        public float bottom = -100.0f;
        public float left = -100.0f;
        public float right = 100.0f;
    }
    ///private bounds references

    
    /// <summary>
    /// Array of areas where area will be allocated in a fixed place
    /// </summary>
    public List<Transform> fixedAreas;





    #endregion

    #region Private Attributes

    // Reference to the independent control component from the scene's game controller.
    private GameControllerIndependentControl _independentControl;

    //bounds exceded controll
    private float excededX = 0F;
    private float excededY = 0F;

    //Reference to current caracter size
    private float _dropSize;

    // Check for camera in locked area
    private bool _cameraInLockArea = false;

    // Check for camera in locked area
    private Vector3 _lockPosition;

    // Camera locked handler
    private bool _lookAtPlace = false;

    // The time that camera will be looking at this place
    private float _lookAtPlaceTimmer = 0;

    // The position that camera will be looking
    private Vector3 _lookAtPlacePos;

    #endregion

    #region Methods
    /// <summary>
    /// Unity's method called when this entity is created, even if it is disabled.
    /// </summary>
    void Awake() {
        // Looks for the independent controller component
        _independentControl = FindObjectOfType<GameControllerIndependentControl>();

        // Sets the camera's target to the current character
        RestoreTarget();
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
        // Update status
        UpdateState();
    }

    /// <summary>
    /// Update the camera phisics
    /// </summary>
    void FixedUpdate() {
        //Camera Movement
        MoveCamera();

        //LookAt player
        LookAt();

        // Reset camera lock at place state
        _lookAtPlace = false;
        _cameraInLockArea = false;
    }

    /// <summary>
    /// Update the camera status
    /// </summary>
    private void UpdateState() {
        //Get drop size
        _dropSize = target.GetComponent<CharacterSize>().GetSize();

        //Update ofset and boundary depends of the size
        if (!target.GetComponent<CharacterControllerCustom>().State.IsFlying)
            _offset = new Vector3(_dropSize * offset.x, _dropSize * offset.y, _dropSize * offset.z);

        // Update look at place timmer if it's needed
        if (_lookAtPlaceTimmer > 0)
            _lookAtPlaceTimmer -= Time.deltaTime;
    }


    /// <summary>
    //// Move the camera to offset position of the player gradually
    /// </summary>
    private void MoveCamera() {
		//Calculate destination
        Vector3 destination = target.transform.position + _offset;

        if (_cameraInLockArea)
            // Lock camera
            destination = _lockPosition;
        else
            //Add Loook around offset
            destination += _lookArroundOffset;


        // Look at place time control
        if (_lookAtPlaceTimmer > 0)
            //Add Loook around offset
            destination = _lookAtPlacePos;

        //Reset bounds exceded to recalculate
        excededX = excededY = 0;
        
        //Calculate if it is out of bounds
        float cameraRealBound = Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg) * (Mathf.Abs(_offset.z));
        if (destination.x < bounds.left + cameraRealBound)
            excededX = destination.x = bounds.left + cameraRealBound;
        else if (destination.x > bounds.right - cameraRealBound) {
            excededX = destination.x = bounds.right - cameraRealBound;
            if (excededX < bounds.left + cameraRealBound)
                excededX = destination.x = bounds.left + cameraRealBound;
        }
        if (destination.y < bounds.bottom + offset.y + (cameraRealBound * 9 / 16))
            excededY = destination.y = bounds.bottom + offset.y  + (cameraRealBound * 9 / 16);
        else if (destination.y > bounds.top - (cameraRealBound * 9 / 16)) {
            excededY = destination.y = bounds.top - (cameraRealBound * 9 / 16);
            if (excededY < bounds.bottom + offset.y + (cameraRealBound * 9 / 16))
                excededY = destination.y = bounds.bottom + offset.y + (cameraRealBound * 9 / 16);
        }

		//Calculate next position
		Vector3 newPosition;
        newPosition = Vector2.Lerp(transform.position, destination, Time.deltaTime * movement.smooth);
        newPosition.z = Mathf.Lerp(transform.position.z, destination.z, Time.deltaTime * movement.zSmooth);

        //Set the position to the camera
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
        if (_cameraInLockArea) {
            Vector3 destinationOnZ = _lockPosition;
            destinationOnZ.z = 0;
            destination = destinationOnZ;
            //transform.rotation = Quaternion.identity;
        }

        // Look at place time control
        if (_lookAtPlaceTimmer > 0) {
            Vector3 destinationOnZ = _lookAtPlacePos;
            destinationOnZ.z = 0;
            destination = destinationOnZ;
            //transform.rotation = Quaternion.identity;
        }


        //If there isn't liberty looking at, block it
        if (lookAt.lookAtFixedOnBounds && excededX != 0) 
            destination.x = excededX;
        if (lookAt.lookAtFixedOnBounds && excededY != 0) 
            destination.y = excededY - (offset.y * (_dropSize) );


        //Calculate the look at position of the camera
        destination = Vector3.Lerp(_lastObjective, destination, Time.deltaTime * lookAt.lookAtSmooth);

		//Set the look at attribute
        transform.LookAt(destination);

		//save the last position for future calculations
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
    /// Restores the target of the camera to the currently controlled character.
    /// </summary>
    public void RestoreTarget() {
        SetObjective(_independentControl.currentCharacter);
    }

    /// <summary>
    /// Set the look arround offset
    /// </summary>
    public void FixCamera(Vector3 position, Vector3 size, string objectName) {

        // If controlled caracter is inside the trigger
        if (_independentControl.currentCharacter.name == objectName || _cameraInLockArea == true) {
            //Setting look arround values depending of the input
            _cameraInLockArea = true;

            float cameraZPos = (size.y) / Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg);

            _lockPosition = new Vector3(position.x, position.y, -cameraZPos);
        } else {

            // Unlock camera 
            _cameraInLockArea = false;

        }



    }

    /// <summary>
    /// Set the look arround offset
    /// </summary>
    public void UnfixCamera() {
        //Setting look arround values depending of the input
        _cameraInLockArea = false;
    }

    /// <summary>
    /// Set the look arround offset
    /// </summary>
    public void LookArround(float OffsetX, float OffsetY) {
        //Setting look arround values depending of the input
        _lookArroundOffset = new Vector3(OffsetX, OffsetY, 0F) * lookAt.lookArroundSmooth * _dropSize;
    }

    /// <summary>
    /// Draws the movement bounds on the editor.
    /// </summary>
    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.top, 0), new Vector3(bounds.right, bounds.top, 0));
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.top, 0), new Vector3(bounds.left, bounds.bottom, 0));
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.bottom, 0), new Vector3(bounds.right, bounds.bottom, 0));
        Gizmos.DrawLine(new Vector3(bounds.right, bounds.bottom, 0), new Vector3(bounds.right, bounds.top, 0));

        Camera camera = GetComponent<Camera>();
        Gizmos.color = Color.yellow;
        Gizmos.matrix = camera.transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
    }


    /// <summary>
    /// Looks at position for determinate time
    /// </summary>
    /// <param name="pos"> X & Y used for position that will be used, Z used for the time that will be watching</param>
    public void LookAtPlace(Vector3 pos) {

        // Set state
        _lookAtPlace = true;

        // Set timmer
        _lookAtPlaceTimmer = pos.z;

        // Set place
        _lookAtPlacePos = new Vector3(pos.x, pos.y, transform.position.z);

    }



    #endregion
}
