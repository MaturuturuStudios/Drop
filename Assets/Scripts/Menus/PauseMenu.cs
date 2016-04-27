using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Control all about pause menu
/// </summary>
public class PauseMenu : IngameMenu {
	#region Public Attributes
	/// <summary>
	/// The first option to be selected
	/// </summary>
	public GameObject firstSelected;
	#endregion

	#region Private Attributes
	/// <summary>
	/// Reference to the scene's game controller input
	/// </summary>
	private GameControllerInput _gameControllerInput;

	/// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;
	#endregion

	#region Methods
	public new void Awake() {
		base.Awake();
		_gameControllerInput = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerInput>();
	}

	public new void OnEnable() {
		base.OnEnable();

		//we have to select the option in update
		_selectOption = true;
	}

	public new void Update() {
		base.Update();

		//if we have to select the option...
		if (_selectOption) {
			//only once!
			_selectOption = false;
			//select the option
			EventSystem.current.SetSelectedGameObject(firstSelected);
		}
	}

	/// <summary>
	/// Quit the pause
	/// </summary>
	public void ContinueGame() {
		_menuNavigator.PauseGame(false);
	}

	/// <summary>
	/// Quit the actual game and return to the main menu
	/// </summary>
	public void ReturnToMainMenu() {
		//TODO: need to control other actions as save game, quit the actual scene...
		//stop the player!
		_gameControllerInput.StopInput();

		//unpause just in case
		_menuNavigator.PauseGame(false);
		_menuNavigator.MainMenu();
	}

	/// <summary>
	/// Open the map and levels menu
	/// </summary>
	public void LoadLevel() {
		_menuNavigator.OpenMenu(MenuNavigator.Menu.MAP_LEVEL_MENU);
	}

	/// <summary>
	/// Reset the level
	/// </summary>
	public void RestartLevel() {
		//TODO: avoid input game and another triggers like win game, attack...
		_gameControllerInput.StopInput();

		_menuNavigator.PauseGame(false);
		_menuNavigator.ChangeScene(SceneManager.GetActiveScene().name);
	}

	/// <summary>
	/// Show the option menu
	/// </summary>
	public void Options() {
		_menuNavigator.OpenMenu(MenuNavigator.Menu.OPTION_MENU);
	}

	/// <summary>
	/// Show the credits
	/// </summary>
	public void Credits() {
		_menuNavigator.OpenMenu(MenuNavigator.Menu.CREDITS_MENU);
	}

	/// <summary>
	/// Quit the game
	/// </summary>
	public void ExitGame() {
		_menuNavigator.ExitGame();
	}
	#endregion
}
