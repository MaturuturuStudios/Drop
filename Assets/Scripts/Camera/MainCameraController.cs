using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {

    // Target of the camera
    private GameObject target;

    /// <summary>
    /// Distance from player to camera
    /// </summary>
    [System.Serializable]
    public class Offset {
        public float far = -15.0f;
        public float up = 2.0f;
    }
    //Offset Attributes
    public Offset offset;
    //Offset Reference
    private Vector3 _offset;

    /// <summary>
    /// Camera options
    /// </summary>
    [System.Serializable]
    public class Movement {
        public float smooth = 2.0f;
        public float zSmooth = 1.0f;
        public float lookAtSmooth = 3.0f;
    }
    public Movement movement;
    //Look at position control
    private Vector3 _lastObjective;

    //Look at position control
    public Vector3 startPosition = new Vector3(-4.0f, 25.0f, -55.0f);

    /// <summary>
    /// Camera liberty rang
    /// </summary>
    [System.Serializable]
    public class Bounds {
        public float top = 100.0f;
        public float down = -100.0f;
        public float left = -100.0f;
        public float right = 100.0f;
    }
    //Bounds Attributes
    public Bounds bounds;
    //private bounds references
    private Vector3 objMov;
    private float excededX = 0F;
    private float excededY = 0F;

    //Camera liberty on bounds exceded
    public bool lookAtLiberty = true;

    //Look Arround Offset
    private Vector3 _lookArroundOffset;
    public float lookArroundSmooth = 10F;

    /// <summary>
    /// Reference to the independent control component from the scene's
    /// game controller.
    /// </summary>
    private GameControllerIndependentControl _independentControl;

    //Reference to current caracter size
    private float _dropSize;

    void OnEnable() {
        //Set camera to its position
        transform.position = startPosition;
    }

    /// <summary>
    /// Unity's method called when this entity is created, even
    /// if it is disabled.
    /// </summary>
    void Awake() {
        // Looks for the independent controller component
        _independentControl = FindObjectOfType<GameControllerIndependentControl>();

        // Sets the camera's target to the current character
        RestoreTarget();
    }

    /// <summary>
    /// Called on start script
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
    /// Update the camera atributes
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
        _offset = new Vector3(0.0f, offset.up * _dropSize, (_dropSize * offset.far));
    }

    /// <summary>
    /// Move the camera to offset position of the player gradually
    /// </summary>
    private void MoveCamera() {
        Vector3 destination = target.transform.position + _offset;

        excededX = excededY = 0;
        //Calcule if it is out of bounds
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

        //Need to use something better than size
        objMov = Vector2.Lerp(transform.position, destination, Time.deltaTime * movement.smooth);
        objMov.z = Mathf.Lerp(transform.position.z, destination.z, Time.deltaTime * movement.zSmooth);

        transform.position = objMov;
    }

    /// <summary>
    /// Makes the camera look to the player's position gradually
    /// </summary>
    private void LookAt() {
        Vector3 destination = target.transform.position + _lookArroundOffset;

        if (lookAtLiberty) {
            destination.x -= excededX;
            destination.y -= excededY;
        }

        destination = Vector3.Lerp(_lastObjective, destination, Time.deltaTime * movement.lookAtSmooth);

        transform.LookAt(destination);

        _lastObjective = destination;
    }

    /// <summary>
    /// Set the objective of the camera
    /// </summary>
    public void SetObjective(GameObject objective) {
        target = objective;
    }

    /// <summary>
    /// Restores the target of the camera to the currently controlled
    /// character.
    /// </summary>
    public void RestoreTarget() {
        SetObjective(_independentControl.currentCharacter);
    }

    /// <summary>
    /// Restores the target of the camera to the currently controlled
    /// character.
    /// </summary>
    public void LookArround(float OffsetX, float OffsetY) {
        _lookArroundOffset = new Vector3(OffsetX, OffsetY, 0F) * lookArroundSmooth * _dropSize;
    }
}
