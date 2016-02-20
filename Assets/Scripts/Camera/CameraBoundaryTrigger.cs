using UnityEngine;
using System.Collections;

public class CameraBoundaryTrigger : MonoBehaviour {


    private CameraController _cameraController;
    private GameObject _camera;

    // Use this for initialization
    void Start ()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _cameraController = _camera.GetComponent<CameraController>();
        //transform.position = _cameraController.currentCharacter.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

    }

    void OnTriggerStay(Collider other)
    {
        _cameraController.Move("Right");
    }
}
