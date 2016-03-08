using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuNavigator : MonoBehaviour {
    public enum Menu {
        NONE,
        MAIN_MENU,
        PAUSE_MENU,
        OPTION_MENU,
        MAP_LEVEL_MENU,
        CREDIT_MENU
    }

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
            panel.SetActive(false);
        }
        public void enable() {
            panel.SetActive(true);
        }
    }

    #region Public Attributes
    public Menu StartWithMenu=Menu.NONE;
    public GameObject background;

    public MenuInstance MainMenu = new MenuInstance(Menu.MAIN_MENU);
    public MenuInstance PauseMenu = new MenuInstance(Menu.PAUSE_MENU);
    public MenuInstance OptionMenu = new MenuInstance(Menu.OPTION_MENU);
    public MenuInstance MapLevelMenu = new MenuInstance(Menu.MAP_LEVEL_MENU);
    public MenuInstance CreditMenu = new MenuInstance(Menu.CREDIT_MENU);
    #endregion
    
    private Stack<MenuInstance> _menuPanel;

    #region Methods
    public void Awake() {
        _menuPanel = new Stack<MenuInstance>();
        if (StartWithMenu != Menu.NONE) {
            openMenu(StartWithMenu);
        }
    }
    
    public bool IsMenuActive() {
        return _menuPanel.Count > 0;
    }
    

    /// <summary>
    /// Open the main menu or a specified menu, adding it to the stack
    /// </summary>
    /// <param name="menu">The menu to open</param>
    public void openMenu(Menu menu = Menu.MAIN_MENU) {
        MenuInstance last = null;
        if (_menuPanel.Count > 0) {
            last = _menuPanel.Peek();
        } else {
            background.SetActive(true);
        }
        
        if (last==null || menu != last.IdMenu) {
            switch (menu) {
                case Menu.MAIN_MENU:
                    _menuPanel.Push(MainMenu);
                    break;
                case Menu.PAUSE_MENU:
                    _menuPanel.Push(PauseMenu);
                    break;
                case Menu.OPTION_MENU:
                    _menuPanel.Push(OptionMenu);
                    break;
                case Menu.MAP_LEVEL_MENU:
                    _menuPanel.Push(MapLevelMenu);
                    break;
                case Menu.CREDIT_MENU:
                    _menuPanel.Push(CreditMenu);
                    break;
            }
            if (last != null) {
                last.disable();
            }
            last = _menuPanel.Peek();
            last.enable();
        }
    }

    /// <summary>
    /// Close all opened menus
    /// </summary>
    public void closeMenu() {
        MenuInstance panel;
        while (_menuPanel.Count > 0) {
            panel = _menuPanel.Pop();
            panel.disable();
        }
        background.SetActive(false);
    }

    /// <summary>
    /// Return to the previous menu on stack
    /// </summary>
    public void ComeBack() {
        MenuInstance panel=_menuPanel.Pop();
        panel.disable();
        panel = _menuPanel.Peek();
        panel.enable();
        if (_menuPanel.Count == 0) {
            closeMenu();
        }
    }

    public void pauseGame() {
        pauseGame(!IsMenuActive());
    }

    public void pauseGame(bool paused) {
        if (paused) {
            Time.timeScale = 0;
            openMenu(MenuNavigator.Menu.PAUSE_MENU);

        } else {
            closeMenu();
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public void ExitGame() {

    }
    #endregion
}
