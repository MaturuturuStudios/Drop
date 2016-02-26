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
    public float up = 2.0f;

    //BoudaryAttributes
    public float width = 5.0f;
    public float height = 3.0f;

    //Speed atributes
    public float lookAtSpeed = 0.5f;
    public float cameraMovementSpeed = 0.2f;
    public float cameraMovementSpeedIdle = 0.01f;
    public float cameraLookAtSpeedRatioSwitching = 1.25f;

    //Anim timmers
    public float idleWaitTime = 3.0f;
    //Timmers counters
    private float _idleCounter = 0;
    private float _changeDropCounter = 0;

    // private references
    private Vector3 _lastPosition; //Substitude ==> _lastPosition -> _lastPlayerPosition
    private Vector3 _lastPositionMovement;
    private Vector3 _lastPositionMovement2;
    private Vector3 _lastPlayerPosition;
    private float _lastFar;
    private float size;
    private bool switchingDrop;
    private float switchingProgress;
    public float secondsOfTransicion = 1.0f;


    /// <summary>
    /// Called on start script
    /// </summary>
    void Start()
    {
        switchingDrop = false;
        switchingProgress = 0.0f;

        //Set the camera in front of current caracter
        _offset = new Vector3(0.0f, up, far);
        //transform.position = currentCharacter.transform.position + _offset;
        transform.position = currentCharacter.transform.position + new Vector3(-4.0f, 25.0f, -55.0f);


        //Set camera status
        _cameraState = CameraState.MOVING;

        //Saver references
        _lastPosition = currentCharacter.transform.position;
        _lastPlayerPosition = currentCharacter.transform.position;
        _lastPositionMovement2 = _lastPositionMovement = transform.position;
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



        //if player has changed
        //if (switchingDrop && (_cameraState == CameraState.CHANGE_DROP || _lastCharacter != currentCharacter))
        if (_lastCharacter != currentCharacter || switchingProgress > 0.0f)
        {
            //Calculate distance              
            Vector3 dist = _lastCharacter.transform.position - currentCharacter.transform.position;
            float distance = dist.magnitude / 3;
            ++switchingProgress;   

            //One step back
            far = _lastFar - distance;
            size = 1;

            if (!switchingDrop && _cameraState == CameraState.CHANGE_DROP)
            {
                switchingProgress = 0.0f;
                _lastCharacter = currentCharacter;
            }

            switchingDrop = true;
            _cameraState = CameraState.CHANGE_DROP;
            _lastPositionMovement2 = Vector3.zero;
        }
        else
        {
            if (_cameraState == CameraState.CHANGE_DROP)
            {
                //return to origin position
                far = _lastFar;



                _changeDropCounter = 0.0f;
                switchingDrop = false;
            }

            size = currentCharacter.GetComponent<CharacterSize>().GetSize();
            // if player and camera aren't moving
            if (_lastPlayerPosition == currentCharacter.transform.position && (_lastPositionMovement2 == transform.position || _cameraState == CameraState.IDLE))
            {
                _idleCounter += Time.deltaTime;
                if (_idleCounter > idleWaitTime)
                    _cameraState = CameraState.IDLE;
            }
            else
            {
                //Reset anim count & state
                _idleCounter = 0.0f;
                //if (_lastPlayerPosition == currentCharacter.transform.position && _cameraState == CameraState.CHANGE_DROP)
                   // _cameraState = CameraState.IDLE;
                //else
                    _cameraState = CameraState.MOVING;
            }
            //Comment this line
        }
        _lastPlayerPosition = currentCharacter.transform.position;
    }

    /// <summary>
    /// Makes the camera look to the player's position
    /// </summary>
    private void MoveCamera()
    {

        //Actualize ofset and boundary depends of the size
        _offset = new Vector3(0.0f, up + size, far - (size * 5));
        _cameraBoundary.transform.localScale = new Vector3(width * size, height * size, 0.1f);

        //Set objective movement base & calculate diference from our position to obsective
        Vector3 objectiveMovement = _lastPositionMovement;
        Vector3 diffMovement = currentCharacter.transform.position + _offset - _lastPositionMovement;

        //Camera Bounds
        float widthBound = 0.0f;
        float heightBound = 0.0f;
        float speed = cameraMovementSpeedIdle;
        if (_cameraState == CameraState.IDLE)
        {
            Debug.Log("idle");
            //No Camera bounds
        }
        else
        {
            //Actualize Camera bounds
            widthBound = (width * size / 2);
            heightBound = (height * size / 2);
            speed = cameraMovementSpeed;
            
            if (_cameraState == CameraState.MOVING)
                Debug.Log("Moving");
            if (_cameraState == CameraState.CHANGE_DROP)
            {
                Debug.Log("ChangeDrop");

                Vector3 dist = _lastCharacter.transform.position - currentCharacter.transform.position;

                speed = dist.magnitude / 60 / secondsOfTransicion;
                if (speed < cameraMovementSpeed)
                    speed = cameraMovementSpeed;
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

        bool dropOut = false;
        //Actualize X position
        float movementX = currentCharacter.transform.position.x - widthBound - _cameraBoundary.transform.position.x;
        //float movementX = currentCharacter.transform.position.x - _lastPositionMovement.x;
        //float movementX = currentCharacter.transform.position.x - widthBound + _offset - _lastPositionMovement.x;
        if (movementX > 0)
        {
            if (movementX > (speed * ratioXYMovement * size) )
                movementX = (speed * ratioXYMovement * size);
            objectiveMovement.x = _lastPositionMovement.x + movementX;
            dropOut = true;
        }
        movementX = currentCharacter.transform.position.x + widthBound - _cameraBoundary.transform.position.x;
        if (movementX < 0)
        {
            if (movementX < (-speed * ratioXYMovement * size))
                movementX = (-speed * ratioXYMovement * size);
            objectiveMovement.x = _lastPositionMovement.x + movementX;
            dropOut = true;
        }


        float movementY = currentCharacter.transform.position.y - heightBound - _cameraBoundary.transform.position.y;
        //Actualize Y position
        //float movementY = currentCharacter.transform.position.y - _lastPositionMovement.y;
        if (movementY > 0)
        {
            if (movementY > (speed * ratioYXMovement * size))
                movementY = (speed * ratioYXMovement * size);
            objectiveMovement.y = _lastPositionMovement.y + movementY;
            dropOut = true;
        }
        movementY = currentCharacter.transform.position.y + heightBound - _cameraBoundary.transform.position.y;
        if (movementY < 0)
        {
            if (movementY < (-speed * ratioYXMovement * size))
                movementY = (-speed * ratioYXMovement * size);
            objectiveMovement.y = _lastPositionMovement.y + movementY;
            dropOut = true;
        }

        //Actualize Z position
        if (diffMovement.z > speed * size)
        {
            objectiveMovement.z += speed * size;
        }
        else if (diffMovement.z < -speed * size)
        {
            objectiveMovement.z -= speed * size;
        }

        switchingDrop = switchingDrop && dropOut;

        //need to performance this
        _lastPositionMovement2 = transform.position;

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
        //Speed
        float speed = lookAtSpeed;
        if (_cameraState == CameraState.CHANGE_DROP)
        {
            Vector3 dist = _lastCharacter.transform.position - currentCharacter.transform.position;

            speed = dist.magnitude / 60 / secondsOfTransicion;
            if (speed < cameraMovementSpeed)
                speed = cameraMovementSpeed;
        }


        if (_cameraState == CameraState.CHANGE_DROP)
            speed *= cameraLookAtSpeedRatioSwitching;

        //Get objective and if it is moving
        Vector3 objective = currentCharacter.transform.position;
        Vector3 diff = currentCharacter.transform.position - _lastPosition;//Substitude ==> _lastPosition -> _lastPlayerPosition

        //Ratio for diagonal looking
        float ratioXY = Mathf.Abs(diff.x) / Mathf.Abs(diff.y);
        if (ratioXY > 1)
            ratioXY = 1;

        //if player moved on X
        if (diff.x > speed)
        {
            objective.x = _lastPosition.x + (speed * ratioXY);//Substitude ==> _lastPosition -> _lastPlayerPosition
        }
        else if (diff.x < -speed)
        {
            objective.x = _lastPosition.x - (speed * ratioXY);//Substitude ==> _lastPosition -> _lastPlayerPosition
        }

        //Ratio for diagonal looking
        ratioXY = Mathf.Abs(diff.y) / Mathf.Abs(diff.x);
        if (ratioXY > 1)
            ratioXY = 1;

        //if player moved on Y
        if (diff.y > speed)
        {
            objective.y = _lastPosition.y + (speed * ratioXY);//Substitude ==> _lastPosition -> _lastPlayerPosition
        }
        else if (diff.y < -speed)
        {
            objective.y = _lastPosition.y - (speed * ratioXY);//Substitude ==> _lastPosition -> _lastPlayerPosition
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
