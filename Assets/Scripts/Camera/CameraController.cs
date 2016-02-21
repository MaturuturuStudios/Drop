using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject currentCharacter;

    public float far = -15.0f;
    public float up = 3.0f;
    public float lookAtSpeed = 0.5f;

    private Vector3 _offset;

    private Vector3 _lastPosition;
    private Vector3 _lastPositionMovement;


    public float width = 5.0f;
    public float height = 3.0f;

    public float cameraMovementSpeed = 0.2f;

    private GameObject _cameraBoundary;


    void OnEnable()
    {
        _offset = new Vector3(0.0f, up, far);
        transform.position = currentCharacter.transform.position + _offset;

        _lastPosition = currentCharacter.transform.position;
        _lastPositionMovement = transform.position;
    }


    void Start()
    {
        _cameraBoundary = GameObject.FindGameObjectWithTag("CameraBoundary");
        _cameraBoundary.transform.position = currentCharacter.transform.position;

        _cameraBoundary.transform.localScale = new Vector3(width, height, 0.1f);
    }

    void LateUpdate()
    {


        bool passed = false;
        Vector3 objectiveMovement = _lastPositionMovement;
        Vector3 diffMovement = currentCharacter.transform.position + _offset - _lastPositionMovement;

        float ratioXYMovement = Mathf.Abs(diffMovement.x) / Mathf.Abs(diffMovement.y);
        if (ratioXYMovement > 1)
            ratioXYMovement = 1;

        float passedOutX = currentCharacter.transform.position.x - (width / 2) - _cameraBoundary.transform.position.x;
        if (passedOutX > 0)
        {
            if (passedOutX > (cameraMovementSpeed * ratioXYMovement))
                passedOutX = (cameraMovementSpeed * ratioXYMovement);
            objectiveMovement.x = _lastPositionMovement.x + passedOutX;
            passed = true;
        }
        passedOutX = currentCharacter.transform.position.x + (width / 2) - _cameraBoundary.transform.position.x;
        if (passedOutX < 0)
        {
            if (passedOutX < (-cameraMovementSpeed * ratioXYMovement))
                passedOutX = (-cameraMovementSpeed * ratioXYMovement);
            objectiveMovement.x = _lastPositionMovement.x + passedOutX;
            passed = true;
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
            passed = true;
        }
        passedOutY = currentCharacter.transform.position.y + (height / 2) - _cameraBoundary.transform.position.y;
        if (passedOutY < 0)
        {
            if (passedOutY < (-cameraMovementSpeed * ratioXYMovement))
                passedOutY = (-cameraMovementSpeed * ratioXYMovement);
            objectiveMovement.y = _lastPositionMovement.y + passedOutY;
            passed = true;
        }

        _cameraBoundary.transform.position = objectiveMovement - _offset;
        transform.position = objectiveMovement;

        _lastPositionMovement = objectiveMovement;




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
