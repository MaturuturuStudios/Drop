using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuNavigator : MonoBehaviour {
    public enum Menu {
        MAIN_MENU,
        PAUSE_MENU,
        OPTION_MENU,
        MAP_LEVEL_MENU,
        CREDIT_MENU
    }

    [System.Serializable]
    public struct MenuInstance {
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

    public MenuInstance MainMenu = new MenuInstance(Menu.MAIN_MENU);
    public MenuInstance PauseMenu = new MenuInstance(Menu.PAUSE_MENU);
    public MenuInstance OptionMenu = new MenuInstance(Menu.OPTION_MENU);
    public MenuInstance MapLevelMenu = new MenuInstance(Menu.MAP_LEVEL_MENU);
    public MenuInstance CreditMenu = new MenuInstance(Menu.CREDIT_MENU);

    private Stack<MenuInstance> _menuPanel;

    void Awake() {
        _menuPanel = new Stack<MenuInstance>();
    }

    public void openMenu(Menu menu = Menu.MAIN_MENU) {
        MenuInstance last = _menuPanel.Peek();
        if (menu != last.IdMenu) {
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
            last.disable();
            
        }
    }

    public void closeMenu() {

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
}
