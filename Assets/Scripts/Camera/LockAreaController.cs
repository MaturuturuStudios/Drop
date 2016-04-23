using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for end Scene, you can configure the displayed message
/// </summary>
public class LockAreaController : MonoBehaviour {

    #region Attributes

    /// <summary>
    /// Main camera reference
    /// </summary>
    private MainCameraController _cameraController;


    /// <summary>
    /// Reference to the independent control component from the scene's game controller.
    /// </summary>
    private GameControllerIndependentControl _independentControl;


    /// <summary>
    /// Reference to the collider of end region.
    /// </summary>
    private BoxCollider _collider;


    /// <summary>
    /// Camera field of view
    /// </summary>
    private Rect _area;

    #endregion

    #region Methods

    /// <summary>
    /// Unity's method called when the entity is created.
    /// Recovers the desired componentes of the entity.
    /// </summary>
    public void Awake() {

        // Retrieves the components of the entities.
        _collider = gameObject.GetComponent<BoxCollider>();

    }

    /// <summary>
    /// Unity's method called on start script only one time
    /// </summary>
    void Start() {

        // Retrieves the components of the entity.
        _cameraController = Camera.main.GetComponent<MainCameraController>();

        // Looks for the independent controller component
        _independentControl = FindObjectOfType<GameControllerIndependentControl>();
    }

    /// <summary>
    /// Control the state of the input data
    /// </summary>
	void Update() {

        // Force 16/9 dimensions to camara vision field
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x * 9 / 16, transform.localScale.x * 9 / 16);
        _collider.size = new Vector3(Mathf.Clamp(_collider.size.x, 0.01f, 1f) , Mathf.Clamp(_collider.size.y, 0.01f, 1f), _collider.size.y);

        // Force position
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        _collider.center = new Vector3(
                Mathf.Clamp(_collider.center.x, -0.5f + (_collider.size.x/2),  0.5f - (_collider.size.x / 2)), 
                Mathf.Clamp(_collider.center.y, -0.5f + (_collider.size.y / 2), 0.5f - (_collider.size.y / 2)), 
                0);

        // Force rotation
        transform.rotation = new Quaternion(0, 0, transform.rotation.z, 0);

        // Calculate parameters to send
        _area = new Rect(transform.position.x, transform.position.y, transform.localScale.x, transform.localScale.y);


    }


    /// <summary>
    /// When player enters on the trigger fix the camera in a determinate position
    /// </summary>
    /// <param name="other">Collider who enters in the trigger</param>
    void OnTriggerStay(Collider other) {

        //only active it with players
        if (other.CompareTag(Tags.Player) && _independentControl.currentCharacter == other.gameObject) {

            // Lock the camera
            _cameraController.LockCamera(_area);
        }
    }


    /// <summary>
    /// When player exits on the trigger unfix the camera
    /// </summary>
    /// <param name="other">Collider who exits out the trigger</param>
    void OnTriggerExit(Collider other) {

        //only active it with players
        if (other.CompareTag(Tags.Player)) {

            // Fix the camera
            _cameraController.UnlockCamera();
        }
    }



    /// <summary>
    /// Unity's method called by the editor in order to draw the gizmos.
    /// Draws the volume on the editor.
    /// </summary>
    public void OnDrawGizmos() {

        // Calls the configuration functions
        if (!Application.isPlaying) {
            Awake();
            Update();
        }

        // Defines the color of the gizmo
        Color color = Color.yellow;
        color.a = 0.15f;
        Gizmos.color = color;

        // Draws camera displayed zone in a plane
        Vector3 size = transform.localScale;
        size.z = 0.01f;
        Gizmos.DrawCube(transform.position, size);

        // Draw trigger action zone
        Gizmos.DrawLine(new Vector3(_collider.center.x - _collider.size.x / 2, _collider.center.y + _collider.size.y / 2, 0), new Vector3(_collider.center.x + _collider.size.x / 2, _collider.center.y + _collider.size.y / 2, 0));
        Gizmos.DrawLine(new Vector3(_collider.center.x - _collider.size.x / 2, _collider.center.y + _collider.size.y / 2, 0), new Vector3(_collider.center.x - _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, 0));
        Gizmos.DrawLine(new Vector3(_collider.center.x - _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, 0), new Vector3(_collider.center.x + _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, 0));
        Gizmos.DrawLine(new Vector3(_collider.center.x + _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, 0), new Vector3(_collider.center.x + _collider.size.x / 2, _collider.center.y + _collider.size.y / 2, 0));
    }
    #endregion
}
