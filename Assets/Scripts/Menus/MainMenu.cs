using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

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
    /// <summary>
    /// The option to be selected
    /// </summary>
    public GameObject firstSelected;
    #endregion

    #region Private Attributes
    /// <summary>
    /// Menu navigator
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
        _selectOption = true;
    }
    
    public void Update() {
        if (_selectOption) {
            _selectOption = false;
            //select the option
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    public void NewGame() {
        _menuNavigator.ChangeScene(NewGameScene);
    }

    /// <summary>
    /// Load a previous game
    /// </summary>
    public void LoadGame() {
        //TODO: open the menu level
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
