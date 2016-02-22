using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    //Game Object References
    private GameObject _cameraBoundary;
    private GameObject _lastCharacter;
    public GameObject currentCharacter;

    //Camera attributes
    enum CameraState { IDLE, MOVING, CHANGE_DROP };
    private CameraState _cameraState;
    private Vector3 _offset;
    public float far = -15.0f;
    public float up = 3.0f;

    //BoudaryAttributes
    public float width = 5.0f;
    public float height = 3.0f;

    //Speed atributes
    public float lookAtSpeed = 0.5f;
    public float cameraMovementSpeed = 0.2f;
    public float cameraMovementSpeedIdle = 0.05f;

    //Anim timmers
    public float idleWaitTime = 3.0f;
    public float changeDropStepTime = 0.25f;
    //Timmers counters
    private float _idleCounter = 0;
    private float _changeDropCounter = 0;

    // private references
    private Vector3 _lastPosition; //Substitude ==> _lastPosition -> _lastPlayerPosition
    private Vector3 _lastPositionMovement;
    private Vector3 _lastPlayerPosition;
    private float _lastFar;


    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        //Set the camera in front of current caracter
        _offset = new Vector3(0.0f, up, far);
        transform.position = currentCharacter.transform.position + _offset;

        //Set camera status
        _cameraState = CameraState.MOVING;

        //Saver references
        _lastPosition = currentCharacter.transform.position;
        _lastPlayerPosition = currentCharacter.transform.position;
        _lastPositionMovement = transform.position;
        _lastCharacter = currentCharacter;
        _lastFar = far;

        //Look up for boundary and set it
        _cameraBoundary = GameObject.FindGameObjectWithTag("CameraBoundary");
        _cameraBoundary.transform.position = currentCharacter.transform.position;
        _cameraBoundary.transform.localScale = new Vector3(width, height, 0.1f);
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
        // if player isn't moving
        if (_lastPlayerPosition == currentCharacter.transform.position)
        {
            _idleCounter += Time.deltaTime;
            if (_idleCounter > idleWaitTime)
                _cameraState = CameraState.IDLE;
        }
        else
        {
            //Reset anim count & state
            _idleCounter = 0.0f;
            _cameraState = CameraState.MOVING;
        }
        //Comment this line
        _lastPlayerPosition = currentCharacter.transform.position;


        //if player has changed
        if (_lastCharacter != currentCharacter)
        {
            //One step back
            far = _lastFar - 10.0f;
            _changeDropCounter += Time.deltaTime;
            _cameraState = CameraState.CHANGE_DROP;
            if (_changeDropCounter > changeDropStepTime)
            {
                //return to origin position
                far = _lastFar + 10.0f;
                _lastCharacter = currentCharacter;
            }
        }
        else
        {
            //Reset anim count
            _changeDropCounter = 0.0f;
        }
    }

    /// <summary>
    /// Makes the camera look to the player's position
    /// </summary>
    private void MoveCamera()
    {

        //Actualize ofset and boundary depends of the size
        float size = currentCharacter.GetComponent<CharacterSize>().GetSize();
        _offset = new Vector3(0.0f, up, far - (size * 5));
        _cameraBoundary.transform.localScale = new Vector3(width * size, height * size, 0.1f);

        //Set objective movement base & calculate diference from our position to obsective
        Vector3 objectiveMovement = _lastPositionMovement;
        Vector3 diffMovement = currentCharacter.transform.position + _offset - _lastPositionMovement;

        //Camera Bounds
        float widthBound = 0;
        float heightBound = 0;
        if (_cameraState == CameraState.IDLE)
        {
            Debug.Log("idle");
            //No Camera bounds
        }
        else
        {
            //Actualize Camera bounds
            float widthBound = (width * size / 2);
            float heightBound = (height * size / 2);
            
            if (_cameraState == CameraState.MOVING)
                Debug.Log("Moving");
            if (_cameraState == CameraState.CHANGE_DROP)
                Debug.Log("ChangeDrop");
        }

        //Calculate ratio of movement
        float ratioXYMovement = Mathf.Abs(diffMovement.x) / Mathf.Abs(diffMovement.y);
        if (ratioXYMovement > 1)
            ratioXYMovement = 1;

        //Actualize X position
        float movementX = currentCharacter.transform.position.x - widthBound - _cameraBoundary.transform.position.x;
        if (movementX > 0)
        {
            if (movementX > (cameraMovementSpeed * ratioXYMovement * size))
                movementX = (cameraMovementSpeed * ratioXYMovement * size);
            objectiveMovement.x = _lastPositionMovement.x + movementX;
        }
        movementX = currentCharacter.transform.position.x + widthBound - _cameraBoundary.transform.position.x;
        if (movementX < 0)
        {
            if (movementX < (-cameraMovementSpeed * ratioXYMovement * size))
                movementX = (-cameraMovementSpeed * ratioXYMovement * size);
            objectiveMovement.x = _lastPositionMovement.x + movementX;
        }

        //Calculate ratio of movement
        ratioXYMovement = Mathf.Abs(diffMovement.y) / Mathf.Abs(diffMovement.x);
        if (ratioXYMovement > 1)
            ratioXYMovement = 1;

        //Actualize Y position
        float movementY = currentCharacter.transform.position.y - heightBound - _cameraBoundary.transform.position.y;
        if (movementY > 0)
        {
            if (movementY > (cameraMovementSpeed * ratioXYMovement * size))
                movementY = (cameraMovementSpeed * ratioXYMovement * size);
            objectiveMovement.y = _lastPositionMovement.y + movementY;
        }
        movementY = currentCharacter.transform.position.y + heightBound - _cameraBoundary.transform.position.y;
        if (movementY < 0)
        {
            if (movementY < (-cameraMovementSpeed * ratioXYMovement * size))
                movementY = (-cameraMovementSpeed * ratioXYMovement * size);
            objectiveMovement.y = _lastPositionMovement.y + movementY;
        }

        //Actualize Z position
        if (diffMovement.z > cameraMovementSpeed * size)
        {
            objectiveMovement.z += cameraMovementSpeed * size;
        }
        else if (diffMovement.z < -cameraMovementSpeed * size)
        {
            objectiveMovement.z -= cameraMovementSpeed * size;
        }


        //Actualize the position of the camera && the boundary
        _cameraBoundary.transform.position = objectiveMovement - _offset;
        transform.position = objectiveMovement;

        //Save the last position
        _lastPositionMovement = objectiveMovement;//Substitude ==> _lastPosition -> _lastPlayerPosition
    }

    /// <summary>
    /// Makes the camera look to the player's position
    /// </summary>
    private void LookAt()
    {
        //Get objective and if it is moving
        Vector3 objective = currentCharacter.transform.position;
        Vector3 diff = currentCharacter.transform.position - _lastPosition;//Substitude ==> _lastPosition -> _lastPlayerPosition

        //Ratio for diagonal looking
        float ratioXY = Mathf.Abs(diff.x) / Mathf.Abs(diff.y);
        if (ratioXY > 1)
            ratioXY = 1;

        //if player moved on X
        if (diff.x > lookAtSpeed)
        {
            objective.x = _lastPosition.x + (lookAtSpeed * ratioXY);//Substitude ==> _lastPosition -> _lastPlayerPosition
        }
        else if (diff.x < -lookAtSpeed)
        {
            objective.x = _lastPosition.x - (lookAtSpeed * ratioXY);//Substitude ==> _lastPosition -> _lastPlayerPosition
        }

        //Ratio for diagonal looking
        ratioXY = Mathf.Abs(diff.y) / Mathf.Abs(diff.x);
        if (ratioXY > 1)
            ratioXY = 1;

        //if player moved on Y
        if (diff.y > lookAtSpeed)
        {
            objective.y = _lastPosition.y + (lookAtSpeed * ratioXY);//Substitude ==> _lastPosition -> _lastPlayerPosition
        }
        else if (diff.y < -lookAtSpeed)
        {
            objective.y = _lastPosition.y - (lookAtSpeed * ratioXY);//Substitude ==> _lastPosition -> _lastPlayerPosition
        }

        //Set position lookAt to camera
        transform.LookAt(objective);

        //Substitude ==> _lastPosition -> _lastPlayerPosition
        _lastPosition = objective;
    }

    /// <summary>
    /// Set the objective of the camera
    /// </summary>
    public void SetObjective(GameObject objective)
    {
        currentCharacter = objective;
    }
}
