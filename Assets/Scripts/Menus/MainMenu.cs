using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Control all about the main menu
/// </summary>
public class MainMenu : IngameMenu {
    #region Public Atributes
    /// <summary>
    /// Scene to be opened when starting a new game
    /// </summary>
    public Scene NewGameScene;
    /// <summary>
    /// The option to be selected
    /// </summary>
    public GameObject firstSelected;
    #endregion

    #region Private Attributes
    /// <summary>
    /// Control if I have to select a default option
    /// </summary>
    private bool _selectOption;
    #endregion

    #region Methods
    public new void Awake() {
        base.Awake();
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus)
                                .GetComponent<MenuNavigator>();
    }

    public new void OnEnable() {
        base.OnEnable();
        _selectOption = true;
    }

    public new void Update() {
        base.Update();
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
        _menuNavigator.ChangeScene(NewGameScene.name);
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
