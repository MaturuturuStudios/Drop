using UnityEngine;
using System.Collections;
using System;

public class GameControllerInput : MonoBehaviour {

	#region Attributes
	/// Internal references
	private GameControllerIndependentControl _switcher;
	//private MainCameraController _mainCameraController;
	private GameControllerHelp _helpController;
    
	/// <summary>
	/// Menu navigator of the scene
	/// </summary>
	private MenuNavigator _ui;

	/// <summary>
	/// Control if the trigger is pressed for only call change drop one time
	/// </summary>
	private bool _triggerPressed = false;

	/// <summary>
	/// Control if the shootCounter is pressed for only call it one time
	/// </summary>
	private bool _shootCounterPressed = false;

	/// <summary>
	/// Control if the input is listening
	/// </summary>
	private bool _enabled;

	/// <summary>
	/// Control if there is input moving
	/// </summary>
	private bool _moving;

	/// <summary>
	/// Controls if any key is pressed for skip intros
	/// </summary>
	private bool _allowToSkip;

	/// <summary>
	/// The source providing the input to the character.
	/// </summary>
	private InputProvider currentInput;

	/// <summary>
	/// The current sequence being played. If null, the
	/// player is on control.
	/// </summary>
	private InputSequence currentInputSequence;

	/// <summary>
	/// The time the current sequence has been being played.
	/// </summary>
	private float sequenceTime;

	[HideInInspector]
	public bool updateOnFixedStep = false;

	#endregion

	#region Methods
	void Start() {
		// Retrives the independent control component
		_switcher = GetComponent<GameControllerIndependentControl>();
		//_mainCameraController = GetComponentInChildren<MainCameraController>();
		_helpController = GetComponent<GameControllerHelp>();

		_ui = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
		_enabled = true;

		// Starts the input provider
		currentInput = new InputProviderFromPlayer();
	}

	void FixedUpdate() {
		if (IsSequencePlaying() && IsInputEnabled()) {

			if (!currentInputSequence.IsOver(sequenceTime)) {
				currentInput = currentInputSequence.GetInputAtTime(sequenceTime);
				sequenceTime += Time.deltaTime;
			}
			else {
				StopSequence();
			}
		}

		if (updateOnFixedStep && IsInputEnabled()) {
			PerformCharacterInput();
		}
	}

	void Update() {
		//Start button
		if (Input.GetButtonDown(Axis.Start))
			_ui.PauseGame();

		_moving = false;

		if (!updateOnFixedStep && !IsSequencePlaying() && IsInputEnabled()) {
			// Character input
			PerformCharacterInput();
		}
		else {
			if (_ui != null) {
				//Select button
				if (Input.GetButtonDown(Axis.Back)) {
					_ui.ComeBack();
					//come back close the menu? unpause the game!!
					if (!_ui.IsMenuActive()) {
						_ui.PauseGame(false);
					}
				}
			}

			if (_allowToSkip &&
				(Input.GetButtonDown(Axis.Action) ||
				Input.GetButtonDown(Axis.Jump) ||
				Input.GetButtonDown(Axis.Irrigate) ||
				Input.GetButtonDown(Axis.ShootMode) ||
				Input.GetButtonDown(Axis.Start) ||
				(Input.anyKeyDown && Input.inputString.Length > 0))) {
				_allowToSkip = false;
				LevelTransitionController ltc = GameObject.FindObjectOfType<LevelTransitionController>();
				if (ltc != null) {
					ltc.SkipEnd();
				}
				GetComponentInChildren<MainCameraAnimationController>().SkipIntro();
			}
		}
	}

	public bool IsInputEnabled() {
		return _enabled && (_ui == null || !_ui.IsMenuActive());
	}

	private void PerformCharacterInput() {
		// Retrieves current character's components
		CharacterControllerCustomPlayer cccp = _switcher.currentCharacter.GetComponent<CharacterControllerCustomPlayer>();
		CharacterShoot shootComponent = _switcher.currentCharacter.GetComponent<CharacterShoot>();
		CharacterAction actionComponent = _switcher.currentCharacter.GetComponent<CharacterAction>();

		// Horizontal input
		float hInput = currentInput.GetAxis(Axis.Horizontal);
		cccp.HorizontalInput = hInput;

		// Vertical input
		float vInput = currentInput.GetAxis(Axis.Vertical);
		cccp.VerticalInput = vInput;

		// Controls if player wants to move
		_moving = hInput != 0 || vInput != 0;

		// Jump input
		if (currentInput.GetButtonDown(Axis.Jump)) {
			cccp.Jump();
		}

		// Control that triggers are pressed only one time
		if (!_triggerPressed && currentInput.GetAxisRaw(Axis.SelectDrop) > 0) {
			_switcher.ControlNextDrop();
			_triggerPressed = true;
		}
		else if (!_triggerPressed && currentInput.GetAxisRaw(Axis.SelectDrop) < 0) {
			_switcher.ControlBackDrop();
			_triggerPressed = true;
		}
		else if (currentInput.GetAxisRaw(Axis.SelectDrop) == 0)
			_triggerPressed = false;

		// Handles the direct access input
		if (currentInput.GetButtonDown(Axis.SelectDrop1))
			_switcher.SetControlFromInput(0);
		if (currentInput.GetButtonDown(Axis.SelectDrop2))
			_switcher.SetControlFromInput(1);
		if (currentInput.GetButtonDown(Axis.SelectDrop3))
			_switcher.SetControlFromInput(2);
		if (currentInput.GetButtonDown(Axis.SelectDrop4))
			_switcher.SetControlFromInput(3);
		if (currentInput.GetButtonDown(Axis.SelectDrop5))
			_switcher.SetControlFromInput(4);

		//Camera looking arround
		//float hLookInput = currentInput.GetAxis(Axis.CamHorizontal);
		//float vLookInput = currentInput.GetAxis(Axis.CamVertical);
		//_mainCameraController.LookArround(hLookInput, vLookInput);

		//Handle shoot input
		bool isShooting = shootComponent.isShooting();
		if (currentInput.GetButtonDown(Axis.Action) || currentInput.GetButtonDown(Axis.Jump))
			shootComponent.Shoot();
		if (currentInput.GetButtonDown(Axis.ShootMode))
			shootComponent.Aim();

		// Control that shootCounter is pressed only one time
		if (!_shootCounterPressed && currentInput.GetAxisRaw(Axis.ShootCounter) > 0) {
			shootComponent.IncreaseSize();
			_shootCounterPressed = true;
		}
		else if (!_shootCounterPressed && currentInput.GetAxisRaw(Axis.ShootCounter) < 0) {
			shootComponent.DecreaseSize();
			_shootCounterPressed = true;
		}
		else if (currentInput.GetAxisRaw(Axis.ShootCounter) == 0)
			_shootCounterPressed = false;

		//Handle action input
		if (currentInput.GetButtonDown(Axis.Action) && !isShooting)
			actionComponent.DoAction();

		//Irrigate action
		if (currentInput.GetButtonDown(Axis.Irrigate))
			actionComponent.Irrigate();

		//Select button
		bool help = currentInput.GetButtonDown(Axis.Back);
		if (help)
			_helpController.ToggleHelp();
		_helpController.UpdateAutoShow(hInput, vInput);

		//Shoot mode pointer inputs
		if (currentInput.GetAxis(Axis.LookAtDir) < 0)
			shootComponent.LookatRight();
		if (currentInput.GetAxis(Axis.LookAtDir) > 0)
			shootComponent.LookatLeft();
	}

	public bool IsSequencePlaying() {
		return currentInputSequence != null;
	}

	public void PlaySequence(InputSequence sequence) {
		if (sequence == null)
			StopSequence();
		else {
			currentInputSequence = sequence;
			sequenceTime = 0;
			updateOnFixedStep = true;
		}
	}

	public void StopSequence() {
		currentInput = new InputProviderFromPlayer();
		currentInputSequence = null;
		updateOnFixedStep = false;
	}

	public InputProvider GetCurrentInputProvider() {
		return currentInput;
	}

	/// <summary>
	/// Stops the player and closes the input.
	/// </summary>
	/// <param name="allowToSkip"></param>
	/// <param name="timeToReanude"></param>
	public void StopInput(bool allowToSkip = false, float timeToReanude = 0)
	{

		// Disable input
		CharacterControllerCustomPlayer cccp = _switcher.currentCharacter.GetComponent<CharacterControllerCustomPlayer>();
		cccp.Stop();
		_enabled = false;

		// Wait for display the intro
		StartCoroutine(WaitForSkip(allowToSkip, timeToReanude));
	}


	/// <summary>
	/// Stops the player immediatly and closes the input.
	/// </summary>
	public void StopInputAndPlayer()
	{

		// Disable input
		CharacterControllerCustomPlayer cccp = _switcher.currentCharacter.GetComponent<CharacterControllerCustomPlayer>();
		cccp.Stop(true);

		_enabled = false;
	}


	/// <summary>
	/// Waits wait time and then allows to skip
	/// </summary>
	/// <param name="waitTime">Desired time to wait untill intro is skipped</param>
	public IEnumerator WaitForSkip(bool allowToSkip, float waitTime) {

		// Wait for display the intro
		yield return new WaitForSeconds(waitTime);

		// allow to skip
		_allowToSkip = allowToSkip;

		yield return true;
	}

	/// <summary>
	/// Resumes the input.
	/// </summary>
	public void ResumeInput() {
		_enabled = true;
	}

	/// <summary>
	/// Indicates if there is moving input
	/// </summary>
	public bool isMoving() {
		return _moving;
	}
	#endregion
}
