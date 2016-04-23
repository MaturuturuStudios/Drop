using UnityEngine;

public class GameControllerInput : MonoBehaviour {

	#region Attributes
	// Internal references
	private GameControllerIndependentControl _switcher;
	private MainCameraController _mainCameraController;

	//menu navigator of the scene
	private MenuNavigator _ui;

	/// <summary>
	/// Control if the trigger is pressed for only call change drop one time
	/// </summary>
	private bool _triggerPressed = false;

	/// <summary>
	/// Control if the shootCounter is pressed for only call it one time
	/// </summary>
	private bool _shootCounterPressed = false;
	#endregion

	#region Methods
	void Start() {
		// Retrives the independent control component
		_switcher = GetComponent<GameControllerIndependentControl>();
		_mainCameraController = GetComponentInChildren<MainCameraController>();

		_ui = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
	}

	void Update() {
		//Start button
		if (Input.GetButtonDown(Axis.Start))
			_ui.PauseGame();

		if (_ui==null || !_ui.IsMenuActive()) {
			// Retrieves current character's components
			CharacterControllerCustomPlayer cccp = _switcher.currentCharacter.GetComponent<CharacterControllerCustomPlayer>();
			CharacterShoot shootComponent = _switcher.currentCharacter.GetComponent<CharacterShoot>();
			CharacterAction actionComponent = _switcher.currentCharacter.GetComponent<CharacterAction>();

			// Horizontal input
			float hInput = Input.GetAxis(Axis.Horizontal);
			cccp.HorizontalInput = hInput;

			// Vertical input
			float vInput = Input.GetAxis(Axis.Vertical);
			cccp.VerticalInput = vInput;

			// Jump input
			float jumpInput = Input.GetAxis(Axis.Jump);
			cccp.JumpInput = jumpInput;
			
			// Control that triggers are pressed only one time
			if (!_triggerPressed && Input.GetAxis(Axis.SelectDrop) > 0) {
				_switcher.ControlNextDrop();
				_triggerPressed = true;
			}
			else if (!_triggerPressed && Input.GetAxis(Axis.SelectDrop) < 0) {
				_switcher.ControlBackDrop();
				_triggerPressed = true;
			}
			else if (Input.GetAxis(Axis.SelectDrop) == 0)
				_triggerPressed = false;

			// Handles the direct access input
			if (Input.GetButtonDown(Axis.SelectDrop1))
				_switcher.SetControl(0);
			if (Input.GetButtonDown(Axis.SelectDrop2))
				_switcher.SetControl(1);
			if (Input.GetButtonDown(Axis.SelectDrop3))
				_switcher.SetControl(2);
			if (Input.GetButtonDown(Axis.SelectDrop4))
				_switcher.SetControl(3);
			if (Input.GetButtonDown(Axis.SelectDrop5))
				_switcher.SetControl(4);

			//Camera looking arround
			float hLookInput = Input.GetAxis(Axis.CamHorizontal);
			float vLookInput = Input.GetAxis(Axis.CamVertical);
			_mainCameraController.LookArround(hLookInput, vLookInput);

			//Handle shoot input
			if (Input.GetButtonDown(Axis.Action) || Input.GetButtonDown(Axis.Jump))
				shootComponent.Shoot();
			if (Input.GetButtonDown(Axis.ShootMode))
				shootComponent.Aim();

			// Control that shootCounter is pressed only one time
			if (!_shootCounterPressed && Input.GetAxis(Axis.ShootCounter) > 0) {
				shootComponent.IncreaseSize();
				_shootCounterPressed = true;
			} else if (!_shootCounterPressed && Input.GetAxis(Axis.ShootCounter) < 0) {
				shootComponent.DecreaseSize();
				_shootCounterPressed = true;
			} else if (Input.GetAxis(Axis.ShootCounter) == 0)
				_shootCounterPressed = false;

			//Handle action input
			if (Input.GetButtonDown(Axis.Action))
				actionComponent.DoAction();

			///NOT SETTED CONTROLS
			//Shoot mode pointer inputs
			if (Input.GetAxis(Axis.LookAtDir) != 0)
				Debug.Log(Axis.LookAtDir);

			//Irrigate action
			if (Input.GetButtonDown(Axis.Irrigate))
				Debug.Log(Axis.Irrigate);

			//Select button
			if (Input.GetButtonDown(Axis.Back))
				Debug.Log(Axis.Back);
		}
		else if(_ui!=null){
			//Select button
			if (Input.GetButtonDown(Axis.Back)) {
				_ui.ComeBack();
				//come back close the menu? unpause the game!!
				if (!_ui.IsMenuActive()) {
					_ui.PauseGame(false);
				}
			}
		}
	}

	/// <summary>
	/// Stops the player and closes the input.
	/// </summary>
	public void StopInput() {
		CharacterControllerCustomPlayer cccp = _switcher.currentCharacter.GetComponent<CharacterControllerCustomPlayer>();
		cccp.Stop();
		enabled = false;
	}
	#endregion
}