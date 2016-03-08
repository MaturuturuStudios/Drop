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
    }
}