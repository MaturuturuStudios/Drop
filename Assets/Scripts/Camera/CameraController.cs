using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    //Camera status
    enum CameraState { IDLE, MOVING, CHANGE_DROP };
    private CameraState _cameraState;    

    //Drop References
    public GameObject currentCharacter;
    private GameObject _lastCharacter;
    //Drop size reference
    private float _size;

    /// <summary>
    /// Distance from player to camera
    /// </summary>
    [System.Serializable]
    public class Offset
    {
        public float far = -15.0f;
        public float up = 2.0f;
    }
    //Offset Attributes
    public Offset offset;
    //Offset Reference
    private Vector3 _offset;

    /// <summary>
    /// Player liberty area movement
    /// </summary>
    [System.Serializable]
    public class Boundary
    {
        //camera allways fixed in same x & y+_offset as player
        public bool disableBoundaryLiberty = false;
        //set the boundary visible
        public bool visible = true;
        //boundary attributes
        public float width = 5.0f;
        public float height = 3.0f;
    }
    //Boudary Attributes
    public Boundary boundary;
    private GameObject _cameraBoundary;
    //Bounds control
    private bool _outOfBounds;
    private bool _outOfTopBound;
    private bool _outOfBottomBound;
    private bool _outOfLeftBounds;
    private bool _outOfRightBound;

    /// <summary>
    /// Camera options when it is moving
    /// </summary>
    [System.Serializable]
    public class Movement
    {
        //Camera movement attributes
        public float lookAtPlayerSpeed = 0.5f;
        public float movementSpeed = 0.2f;
    }
    //Boudary Attributes
    public Movement movement;
    //Movement speed control
    private float _currentMovementSpeed;
    private float _currentZMovementSpeed;
    //Movement position control
    private Vector3 _lastPositionMovement;
    //Look at position control
    private Vector3 _lastObjective;


    /// <summary>
    /// Camera options when it is idle
    /// </summary>
    [System.Serializable]
    public class Idle
    {
        //On end change drop event set idle state
        public bool onSwichEndIdle = false;
        //Camera movement attributes
        public float movementSpeed = 0.01f;
        //Time waiting for set idle state
        public float waitTime = 3.0f;
    }
    //Boudary Attributes
    public Idle idle;
    //Wait time counter
    private float _idleCounter = 0;

    /// <summary>
    /// Camera options when we are changing drop
    /// </summary>
    [System.Serializable]
    public class ChangeDrop
    {
        //make the movement in z direct
        public bool directZMovement = false;
        //Seconds of duration between changing drop
        public float secondsOfTransicion = 0.5f;
        public float lookAtSpeedRatioSwitching = 1.25f;
    }
    public ChangeDrop changeDrop;
    //Speed atributes
    //drop change transition progress
    private float _changingProgress = 0.0f;
    //distance to swhitch drop
    private Vector3 _destinationPosition;
    private Vector3 _startPosition;
    private float _change_distance = 0.0f;

    //Camera set reference
    private CameraSwitcher cameraSwitcher;

    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        //Get Camera Switcher
        cameraSwitcher = transform.parent.GetComponent<CameraSwitcher>();

        //Calculate offset
        _offset = new Vector3(0.0f, offset.up, offset.far);
        //Set drop to its position
        transform.position = currentCharacter.transform.position + new Vector3(-4.0f, 25.0f, -55.0f);

        //Set camera status
        _cameraState = CameraState.MOVING;

        //Saver references
        _lastObjective = currentCharacter.transform.position;
        _lastPositionMovement = transform.position;
        _lastCharacter = currentCharacter;

        //Activate boundary
        if (boundary.visible) {
            //Create new boundary
            _cameraBoundary = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _cameraBoundary.name = "CameraBoundary";

            //Delete colliders
            Destroy(_cameraBoundary.GetComponent<Collider>());

            //paint it
            Color color = Color.blue;
            color.a = 0.1f;
            Material material = new Material(Shader.Find("Transparent/Diffuse"));
            material.color = color;
            _cameraBoundary.GetComponent<Renderer>().material = material;

            //Put it in its position
            _cameraBoundary.transform.position = currentCharacter.transform.position;
            _cameraBoundary.transform.localScale = new Vector3(boundary.width, boundary.height, 0.1f);
        }
    }

    /// <summary>
    /// Update the state of the camera
    /// </summary>
    void Update()
    {
        // Actualize status
        ActualizeState();
    }

    /// <summary>
    /// Update the camera atributes
    /// </summary>
    void LateUpdate()
    {
        //Camera Movement
        MoveCamera();

        //LookAt player
        LookAt();
    }

    /// <summary>
    /// Actualize the camera status
    /// </summary>
    private void ActualizeState()
    {
        //Get drop size
        _size = currentCharacter.GetComponent<CharacterSize>().GetSize();

        //Actualize ofset and boundary depends of the size
        _offset = new Vector3(0.0f, offset.up + _size, offset.far - (_size * 5));
        if (boundary.visible)
        {
            _cameraBoundary.SetActive(true);
            _cameraBoundary.transform.localScale = new Vector3(boundary.width * _size, boundary.height * _size, 0.1f);
        }
        else
        {
            _cameraBoundary.SetActive(false);
        }

        //Set new destination position
        _destinationPosition = currentCharacter.transform.position + _offset;
        Vector3 dist = transform.position - _destinationPosition;

        //is out of bounds range
        if (_outOfBounds)
        {
            //Check side
            if (_outOfRightBound)
                _destinationPosition.x += boundary.width * _size / 2;
            if (_outOfLeftBounds)
                _destinationPosition.x += -boundary.width * _size / 2;
            if (_outOfTopBound)
                _destinationPosition.y += boundary.height * _size / 2;
            if (_outOfBottomBound)
                _destinationPosition.y += -boundary.height * _size / 2;

            //On changing drop
            if (_cameraState == CameraState.CHANGE_DROP)
            {
                _changingProgress = (_change_distance - dist.magnitude) / _change_distance;
                _idleCounter = 0.0f;
            }
            else
            {

                //On camera idle state 
                if (_lastObjective == currentCharacter.transform.position && _cameraState == CameraState.IDLE)
                {
                    _idleCounter += Time.deltaTime;
                    if (_idleCounter > idle.waitTime)
                        _cameraState = CameraState.IDLE;
                }
                else
                {
                    //We are moving
                    _idleCounter = 0.0f;
                    _cameraState = CameraState.MOVING;
                }
            }

        }
        else
        {
            //On change drop end event
            if (_cameraState == CameraState.CHANGE_DROP)
            {
                _cameraState = CameraState.MOVING;
                if (idle.onSwichEndIdle)
                   _cameraState = CameraState.IDLE;
                _changingProgress = 0.0f;
                _idleCounter = 0.0f;
            }

            //On camera idle state 
            if (_lastObjective == currentCharacter.transform.position)
            {
                _idleCounter += Time.deltaTime;
                if (_idleCounter > idle.waitTime)
                    _cameraState = CameraState.IDLE;
            }

        }

        //On change drop start event
        if (_lastCharacter != currentCharacter)
        {
            _cameraState = CameraState.CHANGE_DROP;

            _change_distance = dist.magnitude;
            _startPosition = transform.position;

            _changingProgress = 0.0f;

        }
        
        _lastCharacter = currentCharacter;
    }

    /// <summary>
    /// Move the camera depending of the status
    /// </summary>
    private void MoveCamera()
    {


        //Set objective movement base & calculate diference from our position to obsective
        Vector3 objectiveMovement = _lastPositionMovement;
        Vector3 diffMovement = currentCharacter.transform.position + _offset - _lastPositionMovement;

        //Camera Bounds
        float widthBound = 0.0f;
        float heightBound = 0.0f;
        _currentMovementSpeed = idle.movementSpeed * _size;
        _currentZMovementSpeed = _currentMovementSpeed;
        if (_cameraState == CameraState.IDLE)
        {
            Debug.Log("idle");
            //No Camera bounds
        }
        else
        {
            //Actualize Camera bounds
            if (!boundary.disableBoundaryLiberty)
            {
                widthBound = (boundary.width * _size / 2);
                heightBound = (boundary.height * _size / 2);
            }
            _currentZMovementSpeed = _currentMovementSpeed = movement.movementSpeed * _size;
            
            if (_cameraState == CameraState.MOVING)
                Debug.Log("Moving");
            if (_cameraState == CameraState.CHANGE_DROP)
            {
                Debug.Log("ChangeDrop");

                Vector3 dist = _startPosition - _destinationPosition;

                _currentMovementSpeed = dist.magnitude / 60 / changeDrop.secondsOfTransicion;
               
                if (_currentMovementSpeed < movement.movementSpeed)
                    _currentMovementSpeed = movement.movementSpeed;

                _currentZMovementSpeed = Mathf.Abs(_startPosition.z - _destinationPosition.z) / 60 / changeDrop.secondsOfTransicion;
                if (!changeDrop.directZMovement) _currentZMovementSpeed = Mathf.Sqrt(_currentZMovementSpeed * _changingProgress);
            }
        }

        //Calculate ratio of movement
        float ratioXYMovement = Mathf.Abs(diffMovement.x) / Mathf.Abs(diffMovement.y);
        if (ratioXYMovement > 1)
            ratioXYMovement = 1;
        //Calculate ratio of movement
        float ratioYXMovement = Mathf.Abs(diffMovement.y) / Mathf.Abs(diffMovement.x);
        if (ratioYXMovement > 1)
            ratioYXMovement = 1;

        _outOfBounds = false;
        _outOfTopBound = false;
        _outOfBottomBound = false;
        _outOfLeftBounds = false;
        _outOfRightBound = false;

        //Actualize X position
        float movementX = currentCharacter.transform.position.x - widthBound - _lastPositionMovement.x + _offset.x;
        if (movementX > 0)
        {
            if (movementX > (_currentMovementSpeed * ratioXYMovement) )
                movementX = (_currentMovementSpeed * ratioXYMovement);
            if (movementX < 0.001f)
                movementX = 0.001f;
            objectiveMovement.x = _lastPositionMovement.x + movementX;
            _outOfRightBound = _outOfBounds = true;
        }
        movementX = currentCharacter.transform.position.x + widthBound - _lastPositionMovement.x + _offset.x;
        if (movementX < 0)
        {
            if (movementX < (-_currentMovementSpeed * ratioXYMovement))
                movementX = (-_currentMovementSpeed * ratioXYMovement);
            if (movementX > -0.001f)
                movementX = -0.001f;
            objectiveMovement.x = _lastPositionMovement.x + movementX;
            _outOfLeftBounds = _outOfBounds = true;
        }


        //Actualize Y position
        float movementY = currentCharacter.transform.position.y - heightBound - _lastPositionMovement.y + _offset.y;
        if (movementY > 0)
        {
            if (movementY > (_currentMovementSpeed * ratioYXMovement))
                movementY = (_currentMovementSpeed * ratioYXMovement);
            if (movementY < 0.001f)
                movementY = 0.001f;
            objectiveMovement.y = _lastPositionMovement.y + movementY;
            _outOfTopBound = _outOfBounds = true;
        }
        movementY = currentCharacter.transform.position.y + heightBound - _lastPositionMovement.y + _offset.y;
        if (movementY < 0)
        {
            if (movementY < (-_currentMovementSpeed * ratioYXMovement))
                movementY = (-_currentMovementSpeed * ratioYXMovement);
            if (movementY > -0.001f)
                movementY = -0.001f;
            objectiveMovement.y = _lastPositionMovement.y + movementY;
            _outOfBottomBound = _outOfBounds = true;
        }

        //Actualize Z position
        if (diffMovement.z > _currentZMovementSpeed)
        {
            objectiveMovement.z += _currentZMovementSpeed;
        }
        else if (diffMovement.z < -_currentZMovementSpeed)
        {
            objectiveMovement.z -= _currentZMovementSpeed;
        }

        //Actualize the position of the camera && the boundary
        if(boundary.visible)
            _cameraBoundary.transform.position = objectiveMovement - _offset;
        transform.position = objectiveMovement;

        //Save the last position
        _lastPositionMovement = objectiveMovement;
    }

    /// <summary>
    /// Makes the camera look to the player's position
    /// </summary>
    private void LookAt()
    {
        //Speed
        float speed = movement.lookAtPlayerSpeed;

        if (_cameraState == CameraState.CHANGE_DROP) {
            if (boundary.disableBoundaryLiberty)
                speed = _currentMovementSpeed;
            else
                speed = _currentMovementSpeed * changeDrop.lookAtSpeedRatioSwitching;
        }
        //Get objective and if it is moving
        Vector3 objective = currentCharacter.transform.position;
        Vector3 diff = currentCharacter.transform.position - _lastObjective;

        //Ratio for diagonal looking
        float ratioXY = Mathf.Abs(diff.x) / Mathf.Abs(diff.y);
        if (ratioXY > 1)
            ratioXY = 1;

        //if player moved on X
        if (diff.x > speed)
        {
            objective.x = _lastObjective.x + (speed * ratioXY);
        }
        else if (diff.x < -speed)
        {
            objective.x = _lastObjective.x - (speed * ratioXY);
        }

        //Ratio for diagonal looking
        ratioXY = Mathf.Abs(diff.y) / Mathf.Abs(diff.x);
        if (ratioXY > 1)
            ratioXY = 1;

        //if player moved on Y
        if (diff.y > speed)
        {
            objective.y = _lastObjective.y + (speed * ratioXY);
        }
        else if (diff.y < -speed)
        {
            objective.y = _lastObjective.y - (speed * ratioXY);
        }

        //Set position lookAt to camera
        transform.LookAt(objective);
        
        _lastObjective = objective;
    }

    /// <summary>
    /// Set the objective of the camera
    /// </summary>
    public void SetObjective(GameObject objective)
    {
        currentCharacter = objective;
    }
}
