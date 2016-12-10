using UnityEngine;
using System.Collections;

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
    #endregion

    #region Methods
    void Start() {
		// Retrives the independent control component
		_switcher = GetComponent<GameControllerIndependentControl>();
		//_mainCameraController = GetComponentInChildren<MainCameraController>();
		_helpController = GetComponent<GameControllerHelp>();

		_ui = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
        _enabled = true;

    }

	void Update() {
		//Start button
		if (Input.GetButtonDown(Axis.Start))
			_ui.PauseGame();

        _moving = false;

        if (_enabled && (_ui == null || !_ui.IsMenuActive())) {
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

            // Controls if player wants to move
            _moving = hInput != 0 || vInput != 0;

            // Jump input
            if (Input.GetButtonDown(Axis.Jump)) {
                cccp.Jump();
            }

            // Control that triggers are pressed only one time
            if (!_triggerPressed && Input.GetAxisRaw(Axis.SelectDrop) > 0) {
                _switcher.ControlNextDrop();
                _triggerPressed = true;
            }
			else if (!_triggerPressed && Input.GetAxisRaw(Axis.SelectDrop) < 0) {
                _switcher.ControlBackDrop();
                _triggerPressed = true;
            }
			else if (Input.GetAxisRaw(Axis.SelectDrop) == 0)
                _triggerPressed = false;

            // Handles the direct access input
            if (Input.GetButtonDown(Axis.SelectDrop1))
                _switcher.SetControlFromInput(0);
            if (Input.GetButtonDown(Axis.SelectDrop2))
                _switcher.SetControlFromInput(1);
            if (Input.GetButtonDown(Axis.SelectDrop3))
                _switcher.SetControlFromInput(2);
            if (Input.GetButtonDown(Axis.SelectDrop4))
                _switcher.SetControlFromInput(3);
            if (Input.GetButtonDown(Axis.SelectDrop5))
                _switcher.SetControlFromInput(4);

            //Camera looking arround
            //float hLookInput = Input.GetAxis(Axis.CamHorizontal);
            //float vLookInput = Input.GetAxis(Axis.CamVertical);
            //_mainCameraController.LookArround(hLookInput, vLookInput);

            //Handle shoot input
            bool isShooting = shootComponent.isShooting();
            if (Input.GetButtonDown(Axis.Action) || Input.GetButtonDown(Axis.Jump))
                shootComponent.Shoot();
            if (Input.GetButtonDown(Axis.ShootMode))
                shootComponent.Aim();

            // Control that shootCounter is pressed only one time
            if (!_shootCounterPressed && Input.GetAxisRaw(Axis.ShootCounter) > 0) {
                shootComponent.IncreaseSize();
                _shootCounterPressed = true;
            } else if (!_shootCounterPressed && Input.GetAxisRaw(Axis.ShootCounter) < 0) {
                shootComponent.DecreaseSize();
                _shootCounterPressed = true;
            } else if (Input.GetAxisRaw(Axis.ShootCounter) == 0)
                _shootCounterPressed = false;

            //Handle action input
            if (Input.GetButtonDown(Axis.Action) && !isShooting)
                actionComponent.DoAction();

            //Irrigate action
            if (Input.GetButtonDown(Axis.Irrigate))
                actionComponent.Irrigate();

            //Select button
            bool help = Input.GetButtonDown(Axis.Back);
            if (help)
                _helpController.ToggleHelp();
            _helpController.UpdateAutoShow(hInput, vInput);

            //Shoot mode pointer inputs
            if (Input.GetAxis(Axis.LookAtDir) < 0)
                shootComponent.LookatRight();
            if (Input.GetAxis(Axis.LookAtDir) > 0)
                shootComponent.LookatLeft();
        } else {
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