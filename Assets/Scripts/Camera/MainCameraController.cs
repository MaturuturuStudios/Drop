using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for main camera.
/// </summary>
public class MainCameraController : MonoBehaviour {

    #region Attributes
    // Target of the camera, it use to be the player
    private GameObject target;

	/// <summary>
    /// Distance from player to camera X position will be allways the same
    /// </summary>
    //Offset Variable
    public Offset offset;
	//  Offset Class
    [System.Serializable]
    public class Offset {
        public float far = -15.0f;
        public float up = 2.0f;
    }
    //Offset Reference
    private Vector3 _offset;

    /// <summary>
    /// The position where camera will be at the beggining of the game
    /// </summary>
    public Vector3 startPosition = new Vector3(-4.0f, 25.0f, -55.0f);
	
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
		// Enable/Disable Look at player liberty
		public bool lookAtLiberty = true;
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
    public Bounds bounds;
	//  Bounds Class
    [System.Serializable]
    public class Bounds {
        public float top = 100.0f;
        public float down = -100.0f;
        public float left = -100.0f;
        public float right = 100.0f;
    }
    ///private bounds references
	//bounds exceded controll
    private float excededX = 0F;
    private float excededY = 0F;

    // Reference to the independent control component from the scene's game controller.
    private GameControllerIndependentControl _independentControl;

    //Reference to current caracter size
    private float _dropSize;
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
    /// Unity's method called when this entity is enabled.
    /// </summary>
    void OnEnable() {
        //Set camera to its position
        transform.position = startPosition;
    }
	
    /// <summary>
    /// Unity's method called on start script only one time
    /// </summary>
    void Start() {
        //Calculate offset
        _offset = new Vector3(0.0f, offset.up, offset.far);

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
    void LateUpdate() {
        //Camera Movement
        MoveCamera();

        //LookAt player
        LookAt();
    }

    /// <summary>
    /// Update the camera status
    /// </summary>
    private void UpdateState() {
        //Get drop size
        _dropSize = target.GetComponent<CharacterSize>().GetSize();

        //Update ofset and boundary depends of the size
        if (!target.GetComponent<CharacterControllerCustom>().State.IsFlying)
        _offset = new Vector3(0.0f, offset.up * _dropSize, (_dropSize * offset.far));
    }

    /// <summary>
    /// Move the camera to offset position of the player gradually
    /// </summary>
    private void MoveCamera() {
		//Calculate destination
        Vector3 destination = target.transform.position + _offset;

		//Reset bounds exceded to recalculate
        excededX = excededY = 0;
        //Calculate if it is out of bounds
        if (destination.x > bounds.right) {
            excededX = destination.x - bounds.right;
            destination.x = bounds.right;
        } else if (destination.x < bounds.left) {
            excededX = destination.x - bounds.left;
            destination.x = bounds.left;
        }

        if (destination.y > bounds.top) {
            excededY = destination.y - bounds.top;
            destination.y = bounds.top;
        } else if (destination.y < bounds.down) {
            excededY = destination.y - bounds.down;
            destination.y = bounds.down;
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
		//Calculate objective of the camera
        Vector3 destination = target.transform.position + _lookArroundOffset;

		//If there isn't liberty looking at, block it
        if (lookAt.lookAtLiberty) {
            destination.x -= excededX;
            destination.y -= excededY;
        }

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
    public void LookArround(float OffsetX, float OffsetY) {
		//Setting look arround values depending of the input
        _lookArroundOffset = new Vector3(OffsetX, OffsetY, 0F) * lookAt.lookArroundSmooth * _dropSize;
    }

    /// <summary>
    /// Draws the movement bounds on the editor.
    /// </summary>
    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.top, 0), new Vector3(bounds.right, bounds.top, 0));
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.top, 0), new Vector3(bounds.left, bounds.down, 0));
        Gizmos.DrawLine(new Vector3(bounds.left, bounds.down, 0), new Vector3(bounds.right, bounds.down, 0));
        Gizmos.DrawLine(new Vector3(bounds.right, bounds.down, 0), new Vector3(bounds.right, bounds.top, 0));
    }
    #endregion
}
