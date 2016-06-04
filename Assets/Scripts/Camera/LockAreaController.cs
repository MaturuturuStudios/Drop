using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for end Scene, you can configure the displayed message
/// </summary>
public class LockAreaController : MonoBehaviour {

    #region Attributes

    /// <summary>
    /// Size of the shown area
    /// </summary>
    public float area = 1;

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


    /// Move this method to AWAKE
    /// <summary>
    /// Control the state of the input data
    /// </summary>
	void Update() {

        // Force 16/9 dimensions to camara vision field
        _collider.size = new Vector3(Mathf.Clamp(_collider.size.x, 0.01f, area) , Mathf.Clamp(_collider.size.y, 0.01f, area * 9 / 16), _collider.size.y);
        
        // Force position
        _collider.center = new Vector3(
                Mathf.Clamp(_collider.center.x, -(area / 2 ) + (_collider.size.x/2), (area / 2 ) - (_collider.size.x / 2)), 
                Mathf.Clamp(_collider.center.y, -(area / 2 * 9 / 16) + (_collider.size.y / 2), (area / 2 * 9 / 16) - (_collider.size.y / 2)), 
                0);
        
        // Force rotation
        transform.rotation = new Quaternion(0, 0, transform.rotation.z, 0);

        // Calculate parameters to send
        _area = new Rect(transform.position.x, transform.position.y, area, area * 9 / 16);


    }


    /// <summary>
    /// When player enters on the trigger fix the camera in a determinate position
    /// </summary>
    /// <param name="other">Collider who enters in the trigger</param>
    void OnTriggerStay(Collider other) {

        //only active it with players
        if (other.CompareTag(Tags.Player) && _independentControl.currentCharacter == other.gameObject) {

            // Lock the camera
            _cameraController.LockCameraAndReset(_area);
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
        
        // Draws camera displayed zone in a plane
        Color color = Color.yellow;
        color.a = 0.15f;
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position,new Vector3(area, area * 9 / 16, .1f));

        // Draw trigger action zone
        color = Color.green;
        color.a = 0.15f;
        Gizmos.color = color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(_collider.center, new Vector3(_collider.size.x, _collider.size.y, .1f) );
    }
    #endregion
}
