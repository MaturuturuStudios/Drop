using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;

/// <summary>
/// Control the flow between menus and specials action taken between thems
/// </summary>
public class MenuNavigator : MonoBehaviour {
    #region Enumerations
    public enum Menu {
        NONE,
        MAIN_MENU,
        PAUSE_MENU,
        OPTION_MENU,
        MAP_LEVEL_MENU,
        CREDITS_MENU
    }
    #endregion

    #region Private class
    [System.Serializable]
    public class MenuInstance {
        [System.NonSerialized]
        public Menu IdMenu;
        public GameObject panel;

        public MenuInstance(Menu menu) {
            IdMenu = menu;
            panel = null;
        }

        public void disable() {
            if (panel == null) return;
            panel.SetActive(false);
        }
        public void enable() {
            if (panel == null) return;
            panel.SetActive(true);
        }
    }
    #endregion

    #region Public Attributes
    /// <summary>
    /// Which scene is the starting point of the application/main menu
    /// </summary>
    /// TODO: make an editor to drag and drop a Scene 
    public string startSceneMainMenu;
    /// <summary>
    /// When scene start, should open which menu?
    /// </summary>
    public Menu startWithMenu=Menu.NONE;
    /// <summary>
    /// A gameobject acting as a background for all menus
    /// </summary>
    public GameObject background;

    /// <summary>
    /// Main menu
    /// </summary>
    public MenuInstance mainMenu = new MenuInstance(Menu.MAIN_MENU);
    /// <summary>
    /// Pause Menu
    /// </summary>
    public MenuInstance pauseMenu = new MenuInstance(Menu.PAUSE_MENU);
    /// <summary>
    /// Option Menu
    /// </summary>
    public MenuInstance optionMenu = new MenuInstance(Menu.OPTION_MENU);
    /// <summary>
    /// Map and levels Menu
    /// </summary>
    public MenuInstance mapLevelMenu = new MenuInstance(Menu.MAP_LEVEL_MENU);
    /// <summary>
    /// Credits Menu
    /// </summary>
    public MenuInstance creditsMenu = new MenuInstance(Menu.CREDITS_MENU);
    #endregion

    #region Private Attributes
    /// <summary>
    /// The stack of menus
    /// </summary>
    private Stack<MenuInstance> _menuPanel;
    #endregion

    #region Methods
    public void Awake() {
        _menuPanel = new Stack<MenuInstance>();
        if (startWithMenu != Menu.NONE) {
            OpenMenu(startWithMenu);
        }
    }
    
    /// <summary>
    /// Open the main menu or a specified menu, adding it to the stack.
    /// Advice: this only open the menu and does not perform any special action like pausing the game,
    /// if so, you must use the concrete methods for it.
    /// </summary>
    /// <param name="menu">The menu to open</param>
    public void OpenMenu(Menu menu) {
        MenuInstance last = null;

        //retrieve the last menu
        if (_menuPanel.Count > 0) {
            last = _menuPanel.Peek();
        } else {
            //if is firts, active the background
            background.SetActive(true);
        }
        
        //Check if first menu or is not the same as last menu opened
        if (last==null || menu != last.IdMenu) {
            switch (menu) {
                case Menu.MAIN_MENU:
                    _menuPanel.Push(mainMenu);
                    break;
                case Menu.PAUSE_MENU:
                    _menuPanel.Push(pauseMenu);
                    break;
                case Menu.OPTION_MENU:
                    _menuPanel.Push(optionMenu);
                    break;
                case Menu.MAP_LEVEL_MENU:
                    _menuPanel.Push(mapLevelMenu);
                    break;
                case Menu.CREDITS_MENU:
                    _menuPanel.Push(creditsMenu);
                    break;
            }

            //if had a previous menu, disable it
            if (last != null) {
                last.disable();
            }
            //retrieve and enable the new menu
            last = _menuPanel.Peek();
            last.enable();
        }
    }

    /// <summary>
    /// Close all opened menus
    /// </summary>
    public void CloseMenu() {
        MenuInstance panel;
        //disable all of them
        while (_menuPanel.Count > 0) {
            panel = _menuPanel.Pop();
            panel.disable();
        }
        //disable background
        background.SetActive(false);
    }

    /// <summary>
    /// Return to the previous menu on stack without special actions
    /// Close the menu if there is no more menus on stack
    /// </summary>
    public void ComeBack() {
        MenuInstance panel=_menuPanel.Pop();
        panel.disable();

        //if no more menus, close it
        if (_menuPanel.Count == 0) {
            CloseMenu();
        } else {
            panel = _menuPanel.Peek();
            panel.enable();
        }
    }

    //Region of methods with special/specific actions when pushed or poped from stack
    #region Specifics menu actions
    public void MainMenu() {
        SceneManager.LoadScene(startSceneMainMenu, LoadSceneMode.Single);
    }

    /// <summary>
    /// Pause or unpause the game, depending if exists a menu
    /// If is any menu, close all of them, if not, pause it and show the menu
    /// </summary>
    public void PauseGame() {
        PauseGame(!IsMenuActive());
    }

    /// <summary>
    /// Pause or unpause the game
    /// Unpausing it close all menus
    /// </summary>
    /// <param name="paused">true to pause the game and show the menu</param>
    public void PauseGame(bool paused) {
        if (paused) {
            Time.timeScale = 0;
            OpenMenu(MenuNavigator.Menu.PAUSE_MENU);

        } else {
            CloseMenu();
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Close the game
    /// TODO: needs a confirmation and probably more actions to close it correctly
    /// </summary>
    public void ExitGame() {
        Application.Quit();
    }
    #endregion

    #region Other Methods
    /// <summary>
    /// Check if there exists any menu
    /// </summary>
    /// <returns>True if any menu exists</returns>
    public bool IsMenuActive() {
        return _menuPanel.Count > 0;
    }
    #endregion

    #endregion
}
