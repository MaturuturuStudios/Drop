using UnityEngine;
using System.Collections.Generic;

public class CameraTravelController : MonoBehaviour {

    //Camera status
    enum CameraTravelState { START, PLAY, PAUSE, STOP };
    private CameraTravelState _travelState = CameraTravelState.START;

    /// <summary>
    /// Distance from player to camera
    /// </summary>
    [System.Serializable]
    public class Offset
    {
        public float far = -10.0f;
        public float up = 0.0f;
    }
    //Offset Attributes
    public Offset offset;
    //Offset Reference
    private Vector3 _offset;

    /// <summary>
    /// Camera options when we are changing drop
    /// </summary>
    [System.Serializable]
    public class Settings
    {
        //make the movement in z direct
        public bool directZMovement = false;
    }
    public Settings settings;

    //Position
    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);
    /// <summary>
    /// Camera options when we are changing drop
    /// </summary>
    [System.Serializable]
    public class Objective
    {
        //Position
        public Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
        //Size
        public float size = 1;
        //Seconds of duration between changing drop
        public float secondsOfTransicion = 0.5f;
        public float lookAtSpeedRatioSwitching = 1.25f;
    }
    public List<Objective> objectives;
    //Speed atributes
    //drop change transition progress
    private float _changingProgress = 0.0f;
    //distance to swhitch drop
    private Vector3 _destinationPosition;
    private Vector3 _startPosition;
    private float _change_distance = 0.0f;

    private Objective currentObjective;
    private float currentSize;

    //Movement speed control
    private float _currentMovementSpeed;
    private float _currentZMovementSpeed;
    //Movement position control
    private Vector3 _lastPositionMovement;
    //Look at position control
    private Objective _lastObjective;

    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        //Set drop to its position
        transform.position = startPosition;

        //Set current objective, the first
        currentObjective = objectives[0];
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
        currentSize = currentObjective.size;

        //Actualize ofset and boundary depends of the size
        _offset = new Vector3(0.0f, offset.up + currentSize, offset.far - (currentSize * 5));

        //Set new destination position
        _destinationPosition = currentObjective.position + _offset;
        Vector3 dist = transform.position - _destinationPosition;
        
        //On changing drop
        if (_travelState == CameraTravelState.PLAY)
        {
            _changingProgress = (_change_distance - dist.magnitude) / _change_distance;
        }

        //On change drop end event
        if (_changingProgress > 50.0f)
        {
            _travelState = CameraTravelState.STOP;

            //Next Objective
        }

        //On change drop start event
        if (_lastObjective != currentObjective)
        {
            _travelState = CameraTravelState.PLAY;

            _change_distance = dist.magnitude;
            _startPosition = transform.position;

            _changingProgress = 0.0f;

        }
        
        _lastObjective = currentObjective;
    }

    /// <summary>
    /// Move the camera depending of the status
    /// </summary>
    private void MoveCamera()
    {


        //Set objective movement base & calculate diference from our position to obsective
        Vector3 objectiveMovement = _lastPositionMovement;
        Vector3 diffMovement = currentObjective.position + _offset - _lastPositionMovement;

        //Camera Bounds
        float widthBound = 0.0f;
        float heightBound = 0.0f;
        //_currentMovementSpeed = idle.movementSpeed * currentSize;
        _currentZMovementSpeed = _currentMovementSpeed;


        Vector3 dist = _startPosition - _destinationPosition;

        _currentMovementSpeed = dist.magnitude / 60 / currentObjective.secondsOfTransicion;

        _currentZMovementSpeed = Mathf.Abs(_startPosition.z - _destinationPosition.z) / 60 / currentObjective.secondsOfTransicion;
        if (!settings.directZMovement) _currentZMovementSpeed = Mathf.Sqrt(_currentZMovementSpeed * _changingProgress);


        //Calculate ratio of movement
        float ratioXYMovement = Mathf.Abs(diffMovement.x) / Mathf.Abs(diffMovement.y);
        if (ratioXYMovement > 1)
            ratioXYMovement = 1;
        //Calculate ratio of movement
        float ratioYXMovement = Mathf.Abs(diffMovement.y) / Mathf.Abs(diffMovement.x);
        if (ratioYXMovement > 1)
            ratioYXMovement = 1;
        
        //Actualize X position
        float movementX = currentObjective.position.x - widthBound - _lastPositionMovement.x + _offset.x;
        if (movementX > 0)
        {
            if (movementX > (_currentMovementSpeed * ratioXYMovement) )
                movementX = (_currentMovementSpeed * ratioXYMovement);
            if (movementX < 0.001f)
                movementX = 0.001f;
            objectiveMovement.x = _lastPositionMovement.x + movementX;
        }
        movementX = currentObjective.position.x + widthBound - _lastPositionMovement.x + _offset.x;
        if (movementX < 0)
        {
            if (movementX < (-_currentMovementSpeed * ratioXYMovement))
                movementX = (-_currentMovementSpeed * ratioXYMovement);
            if (movementX > -0.001f)
                movementX = -0.001f;
            objectiveMovement.x = _lastPositionMovement.x + movementX;
        }


        //Actualize Y position
        float movementY = currentObjective.position.y - heightBound - _lastPositionMovement.y + _offset.y;
        if (movementY > 0)
        {
            if (movementY > (_currentMovementSpeed * ratioYXMovement))
                movementY = (_currentMovementSpeed * ratioYXMovement);
            if (movementY < 0.001f)
                movementY = 0.001f;
            objectiveMovement.y = _lastPositionMovement.y + movementY;
        }
        movementY = currentObjective.position.y + heightBound - _lastPositionMovement.y + _offset.y;
        if (movementY < 0)
        {
            if (movementY < (-_currentMovementSpeed * ratioYXMovement))
                movementY = (-_currentMovementSpeed * ratioYXMovement);
            if (movementY > -0.001f)
                movementY = -0.001f;
            objectiveMovement.y = _lastPositionMovement.y + movementY;
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
        float speed = _currentMovementSpeed * currentObjective.lookAtSpeedRatioSwitching;

        //Get objective and if it is moving
        Vector3 objective = currentObjective.position;
        Vector3 diff = currentObjective.position - _lastObjective.position;

        //Ratio for diagonal looking
        float ratioXY = Mathf.Abs(diff.x) / Mathf.Abs(diff.y);
        if (ratioXY > 1)
            ratioXY = 1;

        //if player moved on X
        if (diff.x > speed)
        {
            objective.x = _lastObjective.position.x + (speed * ratioXY);
        }
        else if (diff.x < -speed)
        {
            objective.x = _lastObjective.position.x - (speed * ratioXY);
        }

        //Ratio for diagonal looking
        ratioXY = Mathf.Abs(diff.y) / Mathf.Abs(diff.x);
        if (ratioXY > 1)
            ratioXY = 1;

        //if player moved on Y
        if (diff.y > speed)
        {
            objective.y = _lastObjective.position.y + (speed * ratioXY);
        }
        else if (diff.y < -speed)
        {
            objective.y = _lastObjective.position.y - (speed * ratioXY);
        }

        //Set position lookAt to camera
        transform.LookAt(objective);
        
        _lastObjective.position = objective;
    }

    /// <summary>
    /// Set the objective of the camera
    /// </summary>
    public void SetObjective(Objective objective)
    {
        currentObjective = objective;
    }
}
