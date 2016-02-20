using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject currentCharacter;

    public Vector3 offset;

    private GameObject _boundary;
    private float _boundaryWidth;
    private float _boundaryHeight;

    void OnEnable()
    {
        offset = new Vector3(0.0f, 3.0f, -15.0f);
        transform.position = currentCharacter.transform.position + offset;

        _boundary = GameObject.FindGameObjectWithTag("CameraBoundary");
        _boundaryWidth = _boundary.GetComponent<CameraBoundaryController>().width;
        _boundaryHeight = _boundary.GetComponent<CameraBoundaryController>().height;
    }

	void LateUpdate () {
        transform.LookAt(currentCharacter.transform);
    }

    public void SetObjective(GameObject objective)
    {
        currentCharacter = objective;
    }

    public void SetPosition(Vector3 movement, Vector3 boundaryMovement)
    {
        transform.position = currentCharacter.transform.position + offset  + boundaryMovement;
        _boundary.transform.position += movement;
    }

    public void Move(string side = "Right")
    {
        float movementWidth = _boundaryWidth / 2;
        float movementheight = _boundaryHeight / 2;
        if (currentCharacter.transform.position.x + (_boundaryWidth / 2) > _boundary.transform.position.x) {
            float triggerExitLimit = (_boundary.transform.position.x + (_boundaryWidth / 2));
            float triggerExitLimit2 = currentCharacter.transform.position.x + 0.5f;

            float triggerExitDropShouldBePosition = triggerExitLimit - (currentCharacter.transform.localScale.x / 2);
            movementWidth = currentCharacter.transform.position.x -  triggerExitDropShouldBePosition;
            movementWidth = triggerExitLimit2 - _boundary.transform.position.x - (_boundaryWidth / 2);
            movementWidth = 0.15f;
            // movementWidth = currentCharacter.transform.position.x + (_boundaryWidth / 2) - _boundary.transform.position.x + currentCharacter.transform.localScale.x;
        }
        else if(currentCharacter.transform.position.x - (_boundaryWidth / 2) < _boundary.transform.position.x)
        {
            movementWidth = currentCharacter.transform.position.x - ((_boundaryWidth / 2) + _boundary.transform.position.x);
        }
        else
        {
            movementWidth = 0;
        }

        movementheight = 0.0f;

        Vector3 movement = new Vector3(movementWidth, movementheight, 0.0f);

        //Move Boundary

        //Move camera
        SetPosition(movement, movement + new Vector3(-_boundaryWidth / 2, 0.0f, 0.0f));
    }

}
