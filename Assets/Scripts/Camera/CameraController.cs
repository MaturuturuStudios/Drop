using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject currentCharacter;
    private GameObject lastCharacter;

    public float far = -15.0f;
    public float up = 3.0f;
    public float lookAtSpeed = 0.5f;

    private Vector3 _offset;

    private Vector3 _lastPosition;
    private Vector3 _lastPositionMovement;
    private Vector3 _lastPlayerPosition;


    public float width = 5.0f;
    public float height = 3.0f;

    public float cameraMovementSpeed = 0.2f;
    public float cameraMovementSpeedIdle = 0.05f;

    private GameObject _cameraBoundary;

    private bool _idle;
    private float _idleCounter = 0;
    private float _changeDropCounter = 0;

    enum CameraState { IDLE, MOVING, CHANGE_DROP };
    private CameraState _cameraState;


    void OnEnable()
    {
        _offset = new Vector3(0.0f, up, far);
        transform.position = currentCharacter.transform.position + _offset;

        _lastPosition = currentCharacter.transform.position;
        _lastPositionMovement = transform.position;
        _cameraState = CameraState.MOVING;
        lastCharacter = currentCharacter;
    }


    void Start()
    {
        _cameraBoundary = GameObject.FindGameObjectWithTag("CameraBoundary");
        _cameraBoundary.transform.position = currentCharacter.transform.position;

        _cameraBoundary.transform.localScale = new Vector3(width, height, 0.1f);
        _lastPlayerPosition = currentCharacter.transform.position;
    }

    void LateUpdate()
    {
        _offset = new Vector3(0.0f, up, far);

        if (_lastPlayerPosition == currentCharacter.transform.position)
        {
            _idleCounter += Time.deltaTime;
            if (_idleCounter > 3)
                _cameraState = CameraState.IDLE;
        }
        else {
            _idleCounter = 0;
            _cameraState = CameraState.MOVING;
        }
        _lastPlayerPosition = currentCharacter.transform.position;

        if (lastCharacter != currentCharacter)
        {
            _changeDropCounter += Time.deltaTime;
            _cameraState = CameraState.CHANGE_DROP;
            if (_changeDropCounter > 1)
                lastCharacter = currentCharacter;
        }
        else
        {
            _changeDropCounter = 0;
        }




        Vector3 objectiveMovement;
        Vector3 diffMovement;

        if (_cameraState == CameraState.IDLE)
        {
            Debug.Log("idle");
            objectiveMovement = currentCharacter.transform.position + _offset;
            diffMovement = currentCharacter.transform.position + _offset - _lastPositionMovement;

            float ratioXYMovement = Mathf.Abs(diffMovement.x) / Mathf.Abs(diffMovement.y);
            if (ratioXYMovement > 1)
                ratioXYMovement = 1;

            float passedOutX = currentCharacter.transform.position.x - _cameraBoundary.transform.position.x;
            if (passedOutX > 0)
            {
                if (passedOutX > (cameraMovementSpeedIdle * ratioXYMovement))
                    passedOutX = (cameraMovementSpeedIdle * ratioXYMovement);
                objectiveMovement.x = _lastPositionMovement.x + passedOutX;
            }
            passedOutX = currentCharacter.transform.position.x - _cameraBoundary.transform.position.x;
            if (passedOutX < 0)
            {
                if (passedOutX < (-cameraMovementSpeedIdle * ratioXYMovement))
                    passedOutX = (-cameraMovementSpeedIdle * ratioXYMovement);
                objectiveMovement.x = _lastPositionMovement.x + passedOutX;
            }


            ratioXYMovement = Mathf.Abs(diffMovement.y) / Mathf.Abs(diffMovement.x);
            if (ratioXYMovement > 1)
                ratioXYMovement = 1;

            float passedOutY = currentCharacter.transform.position.y - _cameraBoundary.transform.position.y;
            if (passedOutY > 0)
            {
                if (passedOutY > (cameraMovementSpeedIdle * ratioXYMovement))
                    passedOutY = (cameraMovementSpeedIdle * ratioXYMovement);
                objectiveMovement.y = _lastPositionMovement.y + passedOutY;
            }
            passedOutY = currentCharacter.transform.position.y - _cameraBoundary.transform.position.y;
            if (passedOutY < 0)
            {
                if (passedOutY < (-cameraMovementSpeedIdle * ratioXYMovement))
                    passedOutY = (-cameraMovementSpeedIdle * ratioXYMovement);
                objectiveMovement.y = _lastPositionMovement.y + passedOutY;
            }

            _cameraBoundary.transform.position = objectiveMovement - _offset;
            transform.position = objectiveMovement;

            _lastPositionMovement = objectiveMovement;
        }
        else if (_cameraState == CameraState.MOVING)
        {
            Debug.Log("Moving");
            objectiveMovement = _lastPositionMovement;
            diffMovement = currentCharacter.transform.position + _offset - _lastPositionMovement;

            float ratioXYMovement = Mathf.Abs(diffMovement.x) / Mathf.Abs(diffMovement.y);
            if (ratioXYMovement > 1)
                ratioXYMovement = 1;

            float passedOutX = currentCharacter.transform.position.x - (width / 2) - _cameraBoundary.transform.position.x;
            if (passedOutX > 0)
            {
                if (passedOutX > (cameraMovementSpeed * ratioXYMovement))
                    passedOutX = (cameraMovementSpeed * ratioXYMovement);
                objectiveMovement.x = _lastPositionMovement.x + passedOutX;
            }
            passedOutX = currentCharacter.transform.position.x + (width / 2) - _cameraBoundary.transform.position.x;
            if (passedOutX < 0)
            {
                if (passedOutX < (-cameraMovementSpeed * ratioXYMovement))
                    passedOutX = (-cameraMovementSpeed * ratioXYMovement);
                objectiveMovement.x = _lastPositionMovement.x + passedOutX;
            }


            ratioXYMovement = Mathf.Abs(diffMovement.y) / Mathf.Abs(diffMovement.x);
            if (ratioXYMovement > 1)
                ratioXYMovement = 1;

            float passedOutY = currentCharacter.transform.position.y - (height / 2) - _cameraBoundary.transform.position.y;
            if (passedOutY > 0)
            {
                if (passedOutY > (cameraMovementSpeed * ratioXYMovement))
                    passedOutY = (cameraMovementSpeed * ratioXYMovement);
                objectiveMovement.y = _lastPositionMovement.y + passedOutY;
            }
            passedOutY = currentCharacter.transform.position.y + (height / 2) - _cameraBoundary.transform.position.y;
            if (passedOutY < 0)
            {
                if (passedOutY < (-cameraMovementSpeed * ratioXYMovement))
                    passedOutY = (-cameraMovementSpeed * ratioXYMovement);
                objectiveMovement.y = _lastPositionMovement.y + passedOutY;
            }

            _cameraBoundary.transform.position = objectiveMovement - _offset;
            transform.position = objectiveMovement;

            _lastPositionMovement = objectiveMovement;

        }
        else  
        {
            if (_cameraState == CameraState.CHANGE_DROP)
                Debug.Log("ChangeDrop");
            objectiveMovement = _lastPositionMovement;
            diffMovement = currentCharacter.transform.position + _offset - _lastPositionMovement;
        }


        if (diffMovement.z > cameraMovementSpeed)
        {
            objectiveMovement.z += cameraMovementSpeed;
        }
        else if (diffMovement.z < -cameraMovementSpeed)
        {
            objectiveMovement.z -= cameraMovementSpeed;
        }

        _cameraBoundary.transform.position = objectiveMovement - _offset;
        transform.position = objectiveMovement;

        _lastPositionMovement = objectiveMovement;




        //LookAt
        Vector3 objective = currentCharacter.transform.position;
        Vector3 diff = currentCharacter.transform.position - _lastPosition;

        float ratioXY = Mathf.Abs(diff.x) / Mathf.Abs(diff.y);
        if (ratioXY > 1)
            ratioXY = 1;

        if (diff.x > lookAtSpeed)
        {
            objective.x = _lastPosition.x + (lookAtSpeed * ratioXY);
        }
        else if (diff.x < -lookAtSpeed)
        {
            objective.x = _lastPosition.x - (lookAtSpeed * ratioXY);
        }

        ratioXY = Mathf.Abs(diff.y) / Mathf.Abs(diff.x);
        if (ratioXY > 1)
            ratioXY = 1;
        if (diff.y > lookAtSpeed)
        {
            objective.y = _lastPosition.y + (lookAtSpeed * ratioXY);
        }
        else if (diff.y < -lookAtSpeed)
        {
            objective.y = _lastPosition.y - (lookAtSpeed * ratioXY);
        }

        transform.LookAt(objective);

        _lastPosition = objective;


    }

    public void SetObjective(GameObject objective)
    {
        currentCharacter = objective;
    }
}
