using UnityEngine;

/// <summary>
/// Control all about pause menu
/// </summary>
public class PauseMenu : MonoBehaviour {
    #region Private Attributes
    /// <summary>
    /// The menu navigator
    /// </summary>
    private MenuNavigator _menuNavigator;
    #endregion

    #region Methods
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag("Menus")
                                .GetComponent<MenuNavigator>();
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
    /// Open the map and levels menu
    /// </summary>
    public void RestartLevel() {
        //TODO
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
