using UnityEngine;
using System.Collections;

public class CameraBoundaryController : MonoBehaviour {

    public float width = 5.0f;
    public float height = 3.0f;

    private CameraController _cameraController;
    private GameObject _camera;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _cameraController = _camera.GetComponent<CameraController>();
        transform.position = _cameraController.currentCharacter.transform.position;

        transform.localScale = new Vector3(width, height, 3.0f);
        //transform.position = new Vector3(transform.position.x + (width / 2), transform.position.y, transform.position.z);
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        float passedOutX = 0.0f;
        float passedOutY = 0.0f;

        passedOutX = _cameraController.currentCharacter.transform.position.x - (width / 2) - transform.position.x;
        if (passedOutX > 0)
        {
            if (passedOutX > 0.3f)
                passedOutX = 0.3f;
            _camera.transform.position += new Vector3(passedOutX, 0.0f, 0.0f);
            transform.position += new Vector3(passedOutX, 0.0f, 0.0f);
        }
        passedOutX = _cameraController.currentCharacter.transform.position.x + (width / 2) - transform.position.x;
        if (passedOutX < 0)
        {
            if (passedOutX < -0.3f)
                passedOutX = -0.3f;
            _camera.transform.position += new Vector3(passedOutX, 0.0f, 0.0f);
            transform.position += new Vector3(passedOutX, 0.0f, 0.0f);
        }

        passedOutY = _cameraController.currentCharacter.transform.position.y - (height / 2) - transform.position.y;
        if (passedOutY > 0)
        {
            if (passedOutY > 0.3f)
                passedOutY = 0.3f;
            _camera.transform.position += new Vector3(0.0f, passedOutY, 0.0f);
            transform.position += new Vector3(0.0f, passedOutY, 0.0f);
        }
        passedOutY = _cameraController.currentCharacter.transform.position.y + (height / 2) - transform.position.y;
        if (passedOutY < 0)
        {
            if (passedOutY < -0.3f)
                passedOutY = -0.3f;
            _camera.transform.position += new Vector3(0.0f, passedOutY, 0.0f);
            transform.position += new Vector3(0.0f, passedOutY, 0.0f);
        }
        
        //_cameraController.Move("Right");
    }
}
