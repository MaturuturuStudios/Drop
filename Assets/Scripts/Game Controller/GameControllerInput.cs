using UnityEngine;

public class GameControllerInput : MonoBehaviour {

    // Internal references
    private GameControllerIndependentControl _switcher;

    void Start() {
        // Retrives the independent control component
        _switcher = GetComponent<GameControllerIndependentControl>();
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
}