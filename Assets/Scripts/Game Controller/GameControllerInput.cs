using UnityEngine;

public class GameControllerInput : MonoBehaviour {

    // Internal references
    private GameControllerIndependentControl _switcher;

    private MenuNavigator _ui;
    private bool pause;

    void Start() {
        // Retrives the independent control component
        _switcher = GetComponent<GameControllerIndependentControl>();
<<<<<<< HEAD
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

		//Handle shoot input To delete
		if (Input.GetButtonDown("Action"))
            shootComponent.Shoot();
		if (Input.GetButtonDown("ShootMode"))
            shootComponent.Aim();

        ///NOT SETTED CONTROLS
        //Shoot mode pointer inputs
        if (Input.GetAxis("LookAtDir") != 0)
            Debug.Log("LookAtDir");
        if (Input.GetAxis("ShootCounter") != 0)
            Debug.Log("ShootCounter");

        //Camera whatching arround
        if (Input.GetAxis("CamHorizontal") != 0)
            Debug.Log("CamHorizontal");
        if (Input.GetAxis("CamVertical") != 0)
            Debug.Log("CamVertical");

        //Sluice action
        if (Input.GetButtonDown("Sluice"))
            Debug.Log("Sluice");

        //Start button
        if (Input.GetButtonDown("Start"))
            Debug.Log("Start");

        //Select button
        if (Input.GetButtonDown("Back"))
            Debug.Log("Back");
=======
        _cameraSwitcher = GameObject.FindGameObjectWithTag("CameraSet")
                                .GetComponent<CameraSwitcher>();
        _camera = _cameraSwitcher.transform.FindChild("MainCamera")
                                .GetComponent<Camera>();
        _cameraDebugControler = _cameraSwitcher.transform.FindChild("DebugCamera")
                                .GetComponent<CameraDebugController>();


        _ui = GameObject.FindGameObjectWithTag("Menus")
                                .GetComponent<MenuNavigator>();
    }

	void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            _ui.PauseGame();
        }

        if (!_ui.IsMenuActive()) {
            // Retrieves current character's components
            CharacterControllerCustomPlayer cccp = _switcher.currentCharacter.GetComponent<CharacterControllerCustomPlayer>();
            CharacterShoot shootComponent = _switcher.currentCharacter.GetComponent<CharacterShoot>();

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
            if (Input.GetButtonDown("BackDrop"))
                _switcher.ControlBackDrop();
            if (Input.GetButtonDown("NextDrop"))
                _switcher.ControlNextDrop();

            // Handles the direct access input
            if (Input.GetKeyDown(KeyCode.Keypad1))
                _switcher.SetControl(0);
            if (Input.GetKeyDown(KeyCode.Keypad2))
                _switcher.SetControl(1);
            if (Input.GetKeyDown(KeyCode.Keypad3))
                _switcher.SetControl(2);
            if (Input.GetKeyDown(KeyCode.Keypad4))
                _switcher.SetControl(3);

            // Handle shoot input
            if (Input.GetButtonDown("Aim"))
                shootComponent.Aim();
            if (Input.GetButtonDown("Fire"))
                shootComponent.Shoot();

            // Debug Camera input
            DebugCamera();

        }
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
            float moveClose = Input.GetAxis("Vertical");// Y rotation?
            float moveVertical = 0.0f;
            //At the moment I put it here I have to move it to L & R
            if (Input.GetKey(KeyCode.Q))
                moveVertical += 1.0f;
            if (Input.GetKey(KeyCode.E))
                moveVertical -= 1.0f;

            Vector3 movement = new Vector3(moveHorizontal, moveVertical, moveClose);

            _cameraDebugControler.SetMovement(movement);
>>>>>>> refs/remotes/origin/menus

    }
}