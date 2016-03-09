using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Control all about the main menu
/// </summary>
public class MainMenu : MonoBehaviour {
    #region Public Atributes
    /// <summary>
    /// Scene to be opened when starting a new game
    /// </summary>
    /// TODO: editor for drag scenes
    public string NewGameScene;
    #endregion

    #region Private Attributes
    /// <summary>
    /// Menu navigator
    /// </summary>
    private MenuNavigator _menuNavigator;
    #endregion

    #region Methods
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag("Menus")
                                .GetComponent<MenuNavigator>();
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    public void NewGame() {
        SceneManager.LoadScene(NewGameScene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Load a previous game
    /// </summary>
    public void LoadGame() {
        //TODO
    }

    /// <summary>
    /// Show the credits
    /// </summary>
    public void Credits() {
        _menuNavigator.OpenMenu(MenuNavigator.Menu.CREDITS_MENU);
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void ExitGame() {
        _menuNavigator.ExitGame();
    }

    /// <summary>
    /// Show the options
    /// </summary>
    public void Options() {
        _menuNavigator.OpenMenu(MenuNavigator.Menu.OPTION_MENU);
    }
    #endregion

}
