using UnityEngine;

public class GameControllerInput : MonoBehaviour {

    // Internal references
    private GameControllerIndependentControl _switcher;
    private CameraSwitcher _cameraSwitcher;
    private CameraDebugController _cameraDebugControler;
    private Camera _camera;

    void Start() {
        // Retrives the independent control component
        _switcher = GetComponent<GameControllerIndependentControl>();
        _cameraSwitcher = GameObject.FindGameObjectWithTag("CameraSet")
                                .GetComponent<CameraSwitcher>();
        _camera = _cameraSwitcher.transform.FindChild("MainCamera")
                                .GetComponent<Camera>();
        _cameraDebugControler = _cameraSwitcher.transform.FindChild("DebugCamera")
                                .GetComponent<CameraDebugController>();
    }

	void Update() {
        // Retrieves current character's components
		CharacterControllerCustomPlayer cccp = _switcher.currentCharacter.GetComponent<CharacterControllerCustomPlayer>();
		CharacterShoot shootComponent = _switcher.currentCharacter.GetComponent<CharacterShoot>();
        bool shootmode = _switcher.currentCharacter.GetComponent<CharacterShoot>().isShooting();

        // Horizontal input
        float hInput = Input.GetAxis("Horizontal");
		cccp.HorizontalInput = hInput;

		// Vertical input
		float vInput = Input.GetAxis("Vertical");
		cccp.VerticalInput = vInput;

		// Jump input
		float jumpInput = Input.GetAxis("Jump");
		cccp.JumpInput = jumpInput;

        // Change controlled character
        // Handles the next and back input
        if (Input.GetButtonDown("SelectDrop") && Input.GetAxis("SelectDrop") > 0)
            _switcher.ControlNextDrop();
        if (Input.GetButtonDown("SelectDrop") && Input.GetAxis("SelectDrop") < 0)
            _switcher.ControlBackDrop();

            // Handles the direct access input
            if (Input.GetButtonDown("SelectDrop1"))
            _switcher.SetControl(0);
        if (Input.GetButtonDown("SelectDrop2"))
            _switcher.SetControl(1);
        if (Input.GetButtonDown("SelectDrop3"))
            _switcher.SetControl(2);
        if (Input.GetButtonDown("SelectDrop4"))
            _switcher.SetControl(3);
        if (Input.GetButtonDown("SelectDrop5"))
            _switcher.SetControl(4);

		// Handle shoot input
        if (Input.GetButtonDown("Action"))
            if(shootmode)
                shootComponent.Shoot();
        if (Input.GetButtonDown("ShootMode"))
            shootComponent.Aim();

        // Debug Camera input
        DebugCamera();

        ///NOT SETTED CONTROLS
        //Shoot mode pointer inputs
        if (Input.GetAxis("LookAtDir") != 0)
            Debug.Log("LookAtDir");
        if (Input.GetAxis("ShootCounter") != 0)
            Debug.Log("ShootCounter");

        //Camera whatching arround
        if (Input.GetAxis("CamHorizontal") != 0)
            Debug.Log("CamHorizontal");
        if (Input.GetAxis("CamHorizontal") != 0)
            Debug.Log("CamHorizontal");

        //Sluice action
        if (Input.GetButtonDown("Sluice"))
            Debug.Log("Sluice");

        //Start button
        if (Input.GetButtonDown("Start"))
            Debug.Log("Start");

        //Select button
        if (Input.GetButtonDown("Back"))
            Debug.Log("Back");

    }


    private void DebugCamera() {
        //Switch debug mode
        if (Input.GetKeyDown(KeyCode.F1))
            _cameraSwitcher.SetCameraMode(CameraSwitcher.CameraMode.DEBUG);

        //Back camera
        if (Input.GetKeyDown(KeyCode.F2) && _cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG)
            _cameraSwitcher.BackCamera();

        //Next Camera
        if (Input.GetKeyDown(KeyCode.F3) && _cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG)
            _cameraSwitcher.NextCamera();

        //Debug Camera Input
        if (_cameraSwitcher.cameraMode == CameraSwitcher.CameraMode.DEBUG) {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveClose = Input.GetAxis("Vertical");
            float moveVertical = 0.0f;
            //At the moment I put it here I have to move it to L & R
            if (Input.GetAxis("SelectDrop") != 0)
                moveVertical += Input.GetAxis("SelectDrop");

            Vector3 movement = new Vector3(moveHorizontal, moveVertical, moveClose);

            _cameraDebugControler.SetMovement(movement);

            float mouseAxisX = Input.GetAxis("Mouse X");
            float mouseAxisY = Input.GetAxis("Mouse Y");
            _cameraDebugControler.SetLookAt(mouseAxisX, mouseAxisY);
        }
    }
}