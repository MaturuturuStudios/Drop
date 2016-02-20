using UnityEngine;
using System.Collections;

public class CameraBoundaryController : MonoBehaviour {

    public float width = 5.0f;
    public float height = 3.0f;

    public float cameraMovementSpeed = 0.2f;

    private CameraController _cameraController;
    private GameObject _camera;

    private Vector3 _lastPosition;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _cameraController = _camera.GetComponent<CameraController>();
        transform.position = _cameraController.currentCharacter.transform.position;

        transform.localScale = new Vector3(width, height, 0.1f);

        _lastPosition = transform.position;
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        float passedOutX = 0.0f;
        float passedOutY = 0.0f;

        Vector3 objective = transform.position;
        Vector3 diff = transform.position - _lastPosition;
        float ratioXY = Mathf.Abs(diff.x) / Mathf.Abs(diff.y);
        if (ratioXY > 1)
            ratioXY = 1;
            ratioXY = 1;

        passedOutX = _cameraController.currentCharacter.transform.position.x - (width / 2) - transform.position.x;
        if (passedOutX > 0)
        {
            if (passedOutX > cameraMovementSpeed)
                passedOutX = cameraMovementSpeed;
            passedOutX *= ratioXY;
            _camera.transform.position += new Vector3(passedOutX, 0.0f, 0.0f);
            transform.position += new Vector3(passedOutX, 0.0f, 0.0f);
        }
        passedOutX = _cameraController.currentCharacter.transform.position.x + (width / 2) - transform.position.x;
        if (passedOutX < 0)
        {
            if (passedOutX < -cameraMovementSpeed)
                passedOutX = -cameraMovementSpeed;
            passedOutX *= ratioXY;
            _camera.transform.position += new Vector3(passedOutX, 0.0f, 0.0f);
            transform.position += new Vector3(passedOutX, 0.0f, 0.0f);
        }

        ratioXY = Mathf.Abs(diff.y) / Mathf.Abs(diff.x);
        if (ratioXY > 1)
            ratioXY = 1;
            ratioXY = 1;

        passedOutY = _cameraController.currentCharacter.transform.position.y - (height / 2) - transform.position.y;
        if (passedOutY > 0)
        {
            if (passedOutY > cameraMovementSpeed)
                passedOutY = cameraMovementSpeed;
            passedOutX *= ratioXY;
            _camera.transform.position += new Vector3(0.0f, passedOutY, 0.0f);
            transform.position += new Vector3(0.0f, passedOutY, 0.0f);
        }
        passedOutY = _cameraController.currentCharacter.transform.position.y + (height / 2) - transform.position.y;
        if (passedOutY < 0)
        {
            if (passedOutY < -cameraMovementSpeed)
                passedOutY = -cameraMovementSpeed;
            passedOutX *= ratioXY;
            _camera.transform.position += new Vector3(0.0f, passedOutY, 0.0f);
            transform.position += new Vector3(0.0f, passedOutY, 0.0f);
        }

        _lastPosition = transform.position;
    }
}
