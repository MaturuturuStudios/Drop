using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for end Scene, you can configure the displayed message
/// </summary>
public class LockAreaController : MonoBehaviour {

    #region Attributes

    /// <summary>
    /// Frame size of the trigger. It's the zone that will be seen outside of the trigger
    /// </summary>
    public float frameSize = 0F;


    /// <summary>
    /// Reference to the collider of end region.
    /// </summary>
    private BoxCollider _collider;


    /// <summary>
    /// Main camera reference
    /// </summary>
    private MainCameraController _cameraController;

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
    }

    /// <summary>
    /// Control the state of the input data
    /// </summary>
	void Update() {

        // Force dimensions
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.x);
        _collider.size = new Vector3(_collider.size.x, _collider.size.x * 9 / 16, _collider.size.x * 9 / 16);

        // Force position
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        _collider.center = new Vector3(_collider.center.x, _collider.center.y, 0);

        // Force rotation
        transform.rotation = new Quaternion(0, 0, transform.rotation.z, 0);

        // Force frame size
        if (frameSize < 1)
            frameSize = 1;
    }


    /// <summary>
    /// When player enters on the trigger fix the camera in a determinate position
    /// </summary>
    /// <param name="other">Collider who enters in the trigger</param>
    void OnTriggerEnter(Collider other) {

        //only active it with players
        if (other.CompareTag(Tags.Player)) {

            // Calculate parameters to send
            float xPos = transform.position.x + (_collider.center.x * transform.localScale.x);
            float yPos = transform.position.y + (_collider.center.y * transform.localScale.y);
            Vector2 centerPosition = new Vector2(xPos, yPos);
            Vector2 size = new Vector2(_collider.size.x + (frameSize * 2), _collider.size.y + (frameSize * (2 * 9 / 16)));

            // Fix the camera
            _cameraController.FixCamera(centerPosition, size * transform.localScale.x);
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
            _cameraController.UnfixCamera();
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

        // Convert dimensions to local
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draws camera displayed zone
        Vector3 pos = _collider.center;
        Vector3 size = _collider.size;
        // Add frame
        size.x += (frameSize * 2);
        size.y += (frameSize * (2 * 9 / 16) );
        size.z = 0.01f;
        Gizmos.DrawCube(pos, size);

        // Draw trigger action zone
        Gizmos.DrawLine(new Vector3(_collider.center.x - _collider.size.x / 2, _collider.center.y + _collider.size.y / 2, 0), new Vector3(_collider.center.x + _collider.size.x / 2, _collider.center.y + _collider.size.y / 2, 0));
        Gizmos.DrawLine(new Vector3(_collider.center.x - _collider.size.x / 2, _collider.center.y + _collider.size.y / 2, 0), new Vector3(_collider.center.x - _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, 0));
        Gizmos.DrawLine(new Vector3(_collider.center.x - _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, 0), new Vector3(_collider.center.x + _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, 0));
        Gizmos.DrawLine(new Vector3(_collider.center.x + _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, 0), new Vector3(_collider.center.x + _collider.size.x / 2, _collider.center.y + _collider.size.y / 2, 0));

    }
    #endregion
}
