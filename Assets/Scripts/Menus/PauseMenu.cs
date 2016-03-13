using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Control all about pause menu
/// </summary>
public class PauseMenu : MonoBehaviour {
    #region Public Attributes
    /// <summary>
    /// The first option to be selected
    /// </summary>
    public GameObject firstSelected;
    #endregion

    #region Private Attributes
    /// <summary>
    /// The menu navigator
    /// </summary>
    private MenuNavigator _menuNavigator;
    /// <summary>
    /// Control if I have to select a default option
    /// </summary>
    private bool _selectOption;
    #endregion

    #region Methods
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag("Menus")
                                .GetComponent<MenuNavigator>();
    }

    public void OnEnable() {
        //we have to select the option in update
        _selectOption = true;
    }

    public void Update() {
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
        //this script or menuNavigator?
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
