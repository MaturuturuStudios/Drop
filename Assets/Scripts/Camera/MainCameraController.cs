using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controller for main camera.
/// </summary>
public class MainCameraController : MonoBehaviour {

    #region Public Attributes

    /// <summary>
    /// Defines the states of the camera for set the correct velocity
    /// </summary>
    public enum CameraState {
        Moving,
        ChangeDrop,
        ChangeSizeFast,
        ChangeSizeSlow,
        LookArround,
        LockArea
    }

    /// <summary>
    /// Distance from player to camera X position will be allways the same
    /// </summary>
    public Vector3 offset = new Vector3(0F, 0F, -7.5F);


    /// <summary>
    /// Camera movement velocity on XY
    /// </summary>
    public float velocity = 20f;


    /// <summary>
    /// Camera movement velocity on XY
    /// </summary>
    public float zVelocity = 5f;

    /// <summary>
    /// Camera movement velocity on XY changes current player
    /// </summary>
    public float velocityChangeDrop = 12f;


    /// <summary>
    /// Camera movement velocity on Z  when changes current player
    /// </summary>
    public float zVelocityChangeDrop = 8f;

    /// <summary>
    /// Camera movement velocity on XY when changes size
    /// </summary>
    public float zVelocityChangeSizeFast = 15f;

    /// <summary>
    /// Camera movement velocity on Z when changes size
    /// </summary>
    public float zVelocityChangeSizeSlow = 3f;


    /// <summary>
    /// Camera movement velocity on XY when LockArea state
    /// </summary>
    public float velocityLockArea = 2f;

    /// <summary>
    /// Camera movement velocity on z when LockArea state
    /// </summary>
    public float zVelocityLockArea = 8f;


    /// <summary>
    /// Camera movement velocity on XY when LookArround state
    /// </summary>
    public float velocityLookArround = 2f;


    /// <summary>
    /// Look arround max distance
    /// </summary>
    public float lookArroundDistance = 6F;



    /// <summary>
    /// Per cent of camera position exceded when drop size increse or decrease
    /// </summary>
    [Range (0,.5f)]
    public float extraSizeToReach = .2f;


    /// <summary>
    /// Bound of camera liberty movement area
    /// </summary>
    public Boundary bounds;
    //  Bounds Class
    [System.Serializable]
    public class Boundary {
        public float top = 10000.0f;
        public float bottom = -10000.0f;
        public float left = -10000.0f;
        public float right = 10000.0f;
    }


    /// <summary>
    /// camera raising position respect to the objective
    /// </summary>
    public float raisingPosition = 2F;
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
    /// Controlls if we are switching character
    /// </summary>
    private GameObject _lastTarget;


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
    /// Look Arround Offset
    /// </summary>
    private Vector3 _lookArroundOffset;


    /// <summary>
    /// Reference to current caracter size
    /// </summary>
    private float _dropSize;


    /// <summary>
    /// For check if drop has changed size
    /// </summary>
    private float _lastDropSize;


    /// <summary>
    /// Check for camera in locked area
    /// </summary>
    private bool _cameraLocked = false;


    /// <summary>
    /// Check for camera in locked area
    /// </summary>
    private bool _resetLockState = false;


    /// <summary>
    /// Check for camera in locked area
    /// </summary>
    private Vector3 _lockPosition;

    /// <summary>
    /// Camera final movement velocity on XY
    /// </summary>
    public float _velocity;


    /// <summary>
    /// Camera final movement velocity on XY
    /// </summary>
    public float _zVelocity;


    /// <summary>
    /// Raising position deèndign on the size
    /// </summary>
    private Vector3 _raisingPositionSized;


    /// <summary>
    /// Extra size increased when growing
    /// </summary>
    private float _extraSizeDistance = 0;

    /// <summary>
    /// Camera State
    /// </summary>
    public CameraState cameraState = CameraState.Moving;
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

        // Calculate offset
        _offset = new Vector3(offset.x, offset.y, offset.z);

        // Get drop size
        _dropSize = target.GetComponent<CharacterSize>().GetSize();

        // Set initial values
        _lastDropSize = _dropSize;
        _lastTarget = target;
    }


    /// <summary>
    /// Update the state of the camera
    /// </summary>
    void Update() {
        // Get drop size
        _dropSize = target.GetComponent<CharacterSize>().GetSize();

        _raisingPositionSized = new Vector3(0, raisingPosition * _dropSize, 0);

        // Update ofset and boundary depends of the size
        if (!target.GetComponent<CharacterControllerCustom>().State.IsFlying) {
            _offset = new Vector3(_dropSize * offset.x, _dropSize * offset.y, _dropSize * offset.z);
        }


        // Update the current status
        int statusModifierControl = 0;

        // Changeing size
        if (_lastDropSize != _dropSize) {
            // Set the extra distance to reach
            _extraSizeDistance = _dropSize * (1 + extraSizeToReach) * offset.z;
            _extraSizeDistance -= _offset.z;

            // Look if drop is incresing or decreasing
            if (_lastDropSize > _dropSize)
                _extraSizeDistance *= -1;

            cameraState = CameraState.ChangeSizeFast;
        }
        _lastDropSize = _dropSize;


        if (_extraSizeDistance != 0 && (cameraState == CameraState.ChangeSizeFast || cameraState == CameraState.ChangeSizeSlow)) {

            // distance to reach when fast step
            if (cameraState == CameraState.ChangeSizeFast && Mathf.Abs((_extraSizeDistance + _offset.z) - transform.position.z) < 1f) {
                _extraSizeDistance = 0.1f;
                cameraState = CameraState.ChangeSizeSlow;
            }

            // distance to reach when slow step
            if (cameraState == CameraState.ChangeSizeSlow && _extraSizeDistance == 0.1f && Mathf.Abs((_offset.z) - transform.position.z) < 0.5f) {
                cameraState = CameraState.Moving;
            }

            ++statusModifierControl;
        } else {
            // return to defaul state
            cameraState = CameraState.Moving;
            ++statusModifierControl;
        }

        // Changeing target
        if (_lastTarget != target) {
            cameraState = CameraState.ChangeDrop;

            // When the camera is close to the objective position
            if ((target.transform.position - transform.position - _offset).magnitude < 0.5f) {
                cameraState = CameraState.Moving;
                _lastTarget = target;
            }
            ++statusModifierControl;
        }

        // Looking arround
        if (_lookArroundOffset != Vector3.zero) {
            cameraState = CameraState.LookArround;
            ++statusModifierControl;
        }

        // Lock area
        if (_cameraLocked) {
            cameraState = CameraState.LockArea;
            ++statusModifierControl;
        }

        // default state
        if (statusModifierControl == 0) {
            cameraState = CameraState.Moving;
        }


        // Sets velocity depending of the current event
        switch (cameraState) {
            case CameraState.ChangeDrop:
                _velocity = velocityChangeDrop;
                _zVelocity = zVelocityChangeDrop;
                break;

            case CameraState.ChangeSizeFast:
                _velocity = velocity;
                _zVelocity = zVelocityChangeSizeFast;
                break;

            case CameraState.ChangeSizeSlow:
                _velocity = velocity;
                _zVelocity = zVelocityChangeSizeSlow;
                break;

            case CameraState.LookArround:
                _velocity = velocityLookArround;
                _zVelocity = zVelocity;
                break;

            case CameraState.LockArea:
                _velocity = velocityLockArea;
                _zVelocity = zVelocityLockArea;
                break;

            default:
                cameraState = CameraState.Moving;
                _velocity = velocity;
                _zVelocity = zVelocity;
                break;
        }

    }


    /// <summary>
    /// Update the camera phisics
    /// </summary>
    void FixedUpdate() {

        // Camera Movement
        MoveCamera();

        // Reset state
        if (_resetLockState)
            _cameraLocked = false;
    }


    /// <summary>
    /// Move the camera to offset position of the player gradually
    /// </summary>
    private void MoveCamera() {
        // Calculate destination
        Vector3 destination = target.transform.position + _offset;

        // Set destination depending of the camera status
        if (cameraState == CameraState.LookArround)
            destination += _lookArroundOffset;
        else
            destination += _raisingPositionSized;

        if (cameraState == CameraState.LockArea)
            destination = _lockPosition;

        if (cameraState == CameraState.ChangeSizeFast)
            destination.z += _extraSizeDistance;


        // Calculate if it is out of bounds and stop it at bound exceded
        destination = CheckBounds(destination);

        // Calculate next position
        Vector3 newPosition;
        newPosition = Vector2.Lerp(transform.position, destination, Time.deltaTime * _velocity);
        newPosition.z = Mathf.Lerp(transform.position.z, destination.z, Time.deltaTime * _zVelocity);

        // Set the position to the camera
        transform.position = newPosition;

        // Check if it is too near to set as target reached
        float squaredDistance = (transform.position - destination).magnitude;
        if (squaredDistance < 0.01f) {
            _lastTarget = target;
        }
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
        _lookArroundOffset = new Vector3(OffsetX, OffsetY * (9f / 16f), 0F) * lookArroundDistance * _dropSize;
    }


    /// <summary>
    /// Locks the camera in a position
    /// </summary>
    /// <param name="position"> Area that camera will see</param>
    public void LockCamera(Rect area) {

        // Setting lock state
        _cameraLocked = true;

        float cameraZPos = -(area.height) / Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg);

        _lockPosition = new Vector3(area.x, area.y, cameraZPos);

    }


    /// <summary>
    /// Locks the camera in a position and reset it at the end of the frame
    /// </summary>
    /// <param name="position"> Area that camera will see</param>
    public void LockCameraAndReset(Rect area) {

        // Setting lock reset state
        _resetLockState = true;

        // Lock camera
        LockCamera(area);
    }


    /// <summary>
    /// Unfix camera when a player leaves a CameraLockArea
    /// </summary>
    public void UnlockCamera() {

        // Setting look arround values depending of the input
        _cameraLocked = false;

        _resetLockState = false;
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


    /// <summary>
    /// Checks if the camera vision area is out of the bound and stops it at bound exceded
    /// </summary>
    private Vector3 CheckBounds(Vector3 destination) {

        // Reset bounds exceded to recalculate
        excededX = excededY = 0;

        // Calculate if it is out of bounds
        float cameraShownArea = Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg) * (Mathf.Abs(_offset.z));

        // If right bound exeded
        if (destination.x < bounds.left + cameraShownArea)
            excededX = destination.x = bounds.left + cameraShownArea;

        // If top bound exceded
        else if (destination.x > bounds.right - cameraShownArea) {
            excededX = destination.x = bounds.right - cameraShownArea;

            // If left bound exeded
            if (excededX < bounds.left + cameraShownArea)
                excededX = destination.x = bounds.left + cameraShownArea;
        }

        // If bottom bound exceded
        if (destination.y < bounds.bottom + _offset.y + (cameraShownArea * 9 / 16))
            excededY = destination.y = bounds.bottom + _offset.y + (cameraShownArea * 9 / 16);

        // If top bound exceded
        else if (destination.y > bounds.top - (cameraShownArea * 9 / 16)) {
            excededY = destination.y = bounds.top - (cameraShownArea * 9 / 16);

            // If bottom bound exceded
            if (excededY < bounds.bottom + _offset.y + (cameraShownArea * 9 / 16))
                excededY = destination.y = bounds.bottom + _offset.y + (cameraShownArea * 9 / 16);
        }

        return destination;
    }
    #endregion
}
