using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

/// <summary>
/// Controller for main camera.
/// </summary>
public class MainCameraController : MonoBehaviour {

    #region Public Attributes

    /// <summary>
    /// Defines the states of the camera for set the correct velocity
    /// </summary>
    public enum CameraState {
        Default,
        ChangeDrop,
        ChangeSizeFast,
        ChangeSizeSlow,
        LookArround,
        LockArea,
        GoBackFromArround,
        GoBackFromArea,
        ShootMode
    }

    /// <summary>
    /// Distance from player to camera on Z, other edges will be allways the at the same distance
    /// </summary>
    [Range (0,20)]
    public float zDistance = 7.5F;


    /// <summary>
    /// Camera raising position respect to the objective
    /// </summary>
    [Range(0, 1)]
    public float yOffset = .5f;


    /// <summary>
    /// Look arround max distance
    /// </summary>
    [Range(0, 1)]
    public float lookArroundDistance = .8F;


    /// <summary>
    /// Per cent of camera position exceded when drop size increse or decrease
    /// </summary>
    [Range(0, .5f)]
    public float extraSizeWhenGrow = .2f;


    /// <summary>
    /// Extra distance of z when drop is size 1
    /// </summary>
    public float zDistortionDistance = 5F;


    /// <summary>
    /// Max size where zDistortion will be used
    /// </summary>
    public int maxDistotionSize = 5;


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
    /// Camera movement velocity on XY when LookArround state
    /// </summary>
    public float vibrateTime = 1f;


    /// <summary>
    /// Camera movement velocity on XY when LookArround state
    /// </summary>
    public float vibrateDistance = 1f;


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
    #endregion

    #region Private Attributes


    /// <summary>
    /// Camera State
    /// </summary>
    private CameraState _cameraState = CameraState.Default;


    /// <summary>
    /// Reference to the independent control component from the scene's game controller.
    /// </summary>
    private GameControllerIndependentControl _independentControl;


    /// <summary>
    /// Reference to the input control component from the scene's game controller.
    /// </summary>
    private GameControllerInput _inputControl;


    /// <summary>
    /// Reference depth of field
    /// </summary>
    private DepthOfField _dof;


    /// <summary>
    /// Target of the camera, it use to be the player
    /// </summary>
    private GameObject target;


    /// <summary>
    /// Controlls if we are switching character
    /// </summary>
    private GameObject _lastTarget;


    /// <summary>
    /// Reference to current caracter size
    /// </summary>
    private float _dropSize;


    /// <summary>
    /// For check if drop has changed size
    /// </summary>
    private float _lastDropSize;


    /// <summary>
    /// Offset Reference
    /// </summary>
    private Vector3 _offset;


    /// <summary>
    /// Extra size increased when growing
    /// </summary>
    private float _extraSizeDistance = 0;


    /// <summary>
    /// Distance from the drop position to out of the screen
    /// </summary>
    private float _distanceToBorder = 1F;


    /// <summary>
    /// Extra distance of z sized
    /// </summary>
    private float _zDistortionSized;


    /// <summary>
    /// Look Arround Offset
    /// </summary>
    private Vector3 _lookArroundOffset;


    /// <summary>
    /// Check for camera in locked area
    /// </summary>
    private Vector3 _lockPosition;


    /// <summary>
    /// Check for camera in locked area
    /// </summary>
    private bool _cameraLocked = false;


    /// <summary>
    /// Check for camera in locked area
    /// </summary>
    private bool _resetLockState = false;


    /// <summary>
    /// Camera final movement velocity on XY
    /// </summary>
    private float _velocity;


    /// <summary>
    /// Camera final movement velocity on Z
    /// </summary>
    private float _zVelocity;


    /// <summary>
    /// Camera final movement velocity on XY when goes back from look arroun and lock area
    /// </summary>
    private float _velocityGoBack;


    /// <summary>
    /// Camera final movement velocity on X when goes back from look arroun and lock area
    /// </summary>
    private float _zVelocityGoBack;  


    /// <summary>
    /// Custom ratio value
    /// </summary>
    [HideInInspector]
    public float _invertRatio;


    /// <summary>
    /// Controls if drop is moving
    /// </summary>
    private bool _moving = false;


    /// <summary>
    /// Controls if drop is moving
    /// </summary>
    private float _vibrating = 0f;


    /// <summary>
    /// Controls if drop is moving
    /// </summary>
    private Vector3 _shootPosition = Vector3.zero;

    #endregion

    #region Methods

    /// <summary>
    /// Unity's method called when this entity is created, even if it is disabled.
    /// </summary>
    void Awake() {

        // Looks for the independent controller component
        _independentControl = FindObjectOfType<GameControllerIndependentControl>();

        // Looks for the independent controller component
        _inputControl = FindObjectOfType<GameControllerInput>();

        // Get component depth of field
        _dof = GetComponent<DepthOfField>();

        // Sets the camera's target to the current character
        SetObjective(_independentControl.currentCharacter);
    }


    /// <summary>
    /// Unity's method called on start script only one time
    /// </summary>
    void Start() {

        // Calculate offset
        _offset = new Vector3(0, 0, -zDistance - zDistortionDistance);

        // Get drop size
        _dropSize = target.GetComponent<CharacterSize>().GetSize();

        // Set initial values
        _lastDropSize = _dropSize;
        _lastTarget = target;

        // Set ratio
        _invertRatio = (float)Screen.height /Screen.width;

        // Set distance to border
        _distanceToBorder = Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg) * (Mathf.Abs(_offset.z));
        _distanceToBorder *= Camera.main.aspect;
    }


    /// <summary>
    /// Update the state of the camera
    /// </summary>
    void Update() {
        // Get drop size
        _dropSize = target.GetComponent<CharacterSize>().GetSize();

        // Get Shoot indicator position
        bool shootMode = target.GetComponent<CharacterShoot>().shootmode;
        if (_cameraState != CameraState.LockArea && shootMode) {            
            _shootPosition = target.GetComponent<CharacterShootTrajectory>()._ball.transform.position;
        } else
            _shootPosition = Vector3.zero;

        // Sets distortion distance and Vignette value
        if (_dropSize < maxDistotionSize) {
            _zDistortionSized = zDistortionDistance * ((maxDistotionSize - _dropSize + 1) / maxDistotionSize);
        } else {
            _zDistortionSized = 0;
        }

        // Set depth of field values
        _dof.focalTransform = target.transform;

        // Check if drop is moving
        _moving = _inputControl.isMoving();

        // Update offset and boundary depends of the size
        if (!target.GetComponent<CharacterControllerCustom>().State.IsFlying) {
            _offset = new Vector3(0, yOffset * _distanceToBorder, _dropSize * (-zDistance - _zDistortionSized));
        }
        

        // Update the current status
        int statusModifierControl = 0;

        if (_extraSizeDistance != 0 && (_cameraState == CameraState.ChangeSizeFast || _cameraState == CameraState.ChangeSizeSlow)) {

            // distance to reach when fast step
            if (_cameraState == CameraState.ChangeSizeFast && Mathf.Abs((_extraSizeDistance + _offset.z) - transform.position.z) < 1f) {
                _extraSizeDistance = 0.1f;
                _cameraState = CameraState.ChangeSizeSlow;
            }

            // distance to reach when slow step
            if (_cameraState == CameraState.ChangeSizeSlow && _extraSizeDistance == 0.1f && Mathf.Abs((_offset.z) - transform.position.z) < 0.5f) {
                _cameraState = CameraState.Default;
            }

            ++statusModifierControl;
        } else {
            // return to defaul state
            if (_cameraState != CameraState.GoBackFromArea && _cameraState != CameraState.GoBackFromArround) 
                _cameraState = CameraState.Default;
            ++statusModifierControl;
        }

        // Changeing size
        if (_lastDropSize != _dropSize) {
            // Set the extra distance to reach
            _extraSizeDistance = (1 + extraSizeWhenGrow) * (_offset.z);
            _extraSizeDistance -= _offset.z;

            // Look if drop is incresing or decreasing
            if (_lastDropSize > _dropSize)
                _extraSizeDistance *= -1;

            _cameraState = CameraState.ChangeSizeFast;
        }
        _lastDropSize = _dropSize;
        
        // Looking arround
        if (_lookArroundOffset != Vector3.zero) {
            _cameraState = CameraState.LookArround;
            ++statusModifierControl;
        }

        // Changeing target
        if (_lastTarget != target) {
            _cameraState = CameraState.ChangeDrop;

            // When the camera is close to the objective position
            if ((target.transform.position - transform.position - _offset).magnitude < 0.01f) {
                _cameraState = CameraState.Default;
                _lastTarget = target;
            }
            ++statusModifierControl;
        }
        
        // Lock area
        if (_cameraLocked) {
            _cameraState = CameraState.LockArea;
            ++statusModifierControl;
        }

        // Going Back
        if ((_cameraState == CameraState.GoBackFromArea || _cameraState == CameraState.GoBackFromArround) && _moving) {
            // When drop moves to much from the start of go back position
            _cameraState = CameraState.Default;
        }

        // Shoot mode

        if (_shootPosition != Vector3.zero) {
            _cameraState = CameraState.ShootMode;
        }

        // Default state
        if (statusModifierControl == 0 && _cameraState != CameraState.GoBackFromArea && _cameraState != CameraState.GoBackFromArround && _cameraState != CameraState.ShootMode) {
            _cameraState = CameraState.Default;
        }


        // Sets velocity depending of the current event
        switch (_cameraState) {
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
                if (!_moving) { 
                    _velocity = velocityLookArround;
                    _zVelocity = zVelocity;
                } else {
                    _velocity = velocity;
                    _zVelocity = zVelocity;
                }
                break;

            case CameraState.LockArea:
                _velocity = velocityLockArea;
                _zVelocity = zVelocityLockArea;
                break;

            case CameraState.GoBackFromArea:
            case CameraState.GoBackFromArround:
                _velocity = _velocityGoBack;
                _zVelocity = _zVelocityGoBack;
                break;

            case CameraState.ShootMode:
                _velocity = velocityLockArea;
                _zVelocity = zVelocityLockArea;
                break;

            default:
                _cameraState = CameraState.Default;
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
        if (_resetLockState) {
            _cameraLocked = false;
            _resetLockState = false;

            _velocityGoBack = velocityLockArea;
            _zVelocityGoBack = zVelocityLockArea;

            _cameraState = CameraState.GoBackFromArea;
        }
    }


    /// <summary>
    /// Move the camera to offset position of the player gradually
    /// </summary>
    private void MoveCamera() {
        // Calculate destination
        Vector3 destination = target.transform.position + _offset;

        // Set destination depending of the camera status

        // Calculate look arround position        
        if (_cameraState == CameraState.LookArround) {

            // Add Look arround offset
            destination += _lookArroundOffset;
        }

        // Character changing size
        if (_cameraState == CameraState.ChangeSizeFast)
            destination.z += _extraSizeDistance;

        // Camera locked in position
        if (_cameraState == CameraState.LockArea)
            destination = _lockPosition;
        else {
            // Calculate if it is out of bounds and stop it at bound exceded
            destination = CheckBounds(destination);

            // When it is in shootmode
            if (_cameraState == CameraState.ShootMode) {
                Debug.Log("Drop Position >" + destination + "<");
                destination += _shootPosition + _offset;
                destination /= 2;
                Debug.Log("Shoot Position >" + _shootPosition + "< Destination >"+ destination + "<");
            }
        }


        // Calculate next position
        Vector3 newPosition;
        newPosition = Vector2.Lerp(transform.position, destination, Time.deltaTime * _velocity);
        newPosition.z = Mathf.Lerp(transform.position.z, destination.z, Time.deltaTime * _zVelocity);


        _vibrating -= Time.deltaTime;
        if (_vibrating > 0f) {
            newPosition += new Vector3(Random.Range(-vibrateDistance, vibrateDistance), Random.Range(-vibrateDistance, vibrateDistance), 0f);
        }

        // Set the position to the camera
        transform.position = newPosition;

        // Check if it is too near to set as target reached
        float squaredDistance = (transform.position - destination).magnitude;
        if (squaredDistance < 0.5f) {
            // Actualize target 
            _lastTarget = target;

            // Go to desired state
            if (_cameraState == CameraState.ChangeDrop || _cameraState == CameraState.GoBackFromArea)
                _cameraState = CameraState.Default;
        }
    }


    /// <summary>
    /// Checks if the camera vision area is out of the bound and stops it at bound exceded
    /// </summary>
    private Vector3 CheckBounds(Vector3 destination) {

        // Calculate if it is out of bounds _distanceToBorder
        _distanceToBorder = Mathf.Tan(Camera.main.fieldOfView * Mathf.Rad2Deg) * (Mathf.Abs(_offset.z));

        _distanceToBorder -= _dropSize;

        // If bottom bound exceded
        if (destination.x < bounds.left + (_distanceToBorder))
            destination.x = bounds.left + (_distanceToBorder);

        // If top bound exceded       
        else if (destination.x > bounds.right - (_distanceToBorder)) { 
            destination.x = bounds.right - (_distanceToBorder);

            // If bottom bound exceded
            if (destination.x < bounds.left + (_distanceToBorder))
            destination.x = bounds.left + (_distanceToBorder);
        }

        _distanceToBorder *= _invertRatio;

        // If bottom bound exceded
        if (destination.y < bounds.bottom + (_distanceToBorder))
            destination.y = bounds.bottom + (_distanceToBorder);
        // If top bound exceded
        else if (destination.y > bounds.top - (_distanceToBorder)) { 
            destination.y = bounds.top - (_distanceToBorder);

            // If bottom bound exceded
            if (destination.y < bounds.bottom + (_distanceToBorder))
                destination.y = bounds.bottom + (_distanceToBorder);
        } 
        
        return destination;
    }


    /// <summary>
    /// Set the objective of the camera
    /// </summary>
    /// <param name="objective">GameObject who is the target of the camera</param>
    public void SetObjective(GameObject objective, bool isFusion = false) {
        target = objective;
        if (isFusion) _lastTarget = target;
    }


    /// <summary>
    /// Set the look arround offset
    /// </summary>
    public void LookArround(float OffsetX, float OffsetY) {

        if ((OffsetX != 0 || OffsetY != 0) ) {
            // Setting look arround values depending of the input
            _lookArroundOffset = new Vector3(OffsetX, OffsetY, 0F);

            // Get offset
            if (_lookArroundOffset.y > 0)
                _lookArroundOffset.y *= ((_distanceToBorder * lookArroundDistance ) - _offset.y) / (_distanceToBorder * lookArroundDistance );
            else if (_lookArroundOffset.y < 0)
                _lookArroundOffset.y *= ((_distanceToBorder * lookArroundDistance ) + _offset.y) / (_distanceToBorder * lookArroundDistance );
                
            _lookArroundOffset.x *= Camera.main.aspect;
            _lookArroundOffset *= lookArroundDistance * _distanceToBorder;
        } else if (_cameraState == CameraState.LookArround) {
            // Going back
            _lookArroundOffset = Vector3.zero;

            _velocityGoBack = velocityLockArea;
            _zVelocityGoBack = zVelocityLockArea;
            if (!_moving)
                _cameraState = CameraState.GoBackFromArround;
            else
                _cameraState = CameraState.Default;

        }
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

        // Set go back velocity the same as lock Camera velocity
        _velocityGoBack = velocityLockArea;
        _zVelocityGoBack = zVelocityLockArea;

        _cameraState = CameraState.GoBackFromArea;
    }

    /// <summary>
    /// Makes camera vibrate efect
    /// </summary>
    /// <param name="time"></param>
    public void Vibrate(float time = 0) {

        _vibrating = time;

        if (time != 0)
            _vibrating = vibrateTime;
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
