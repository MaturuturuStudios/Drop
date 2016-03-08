using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    private MenuNavigator _menuNavigator;

    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag("Menus")
                                .GetComponent<MenuNavigator>();
    }

    public void ContinueGame() {
        _menuNavigator.pauseGame(false);
    }

    public void ReturnToMainMenu() {
        _menuNavigator.closeMenu();
        _menuNavigator.openMenu(MenuNavigator.Menu.MAIN_MENU);
    }

    public void LoadLevel() {
        _menuNavigator.openMenu(MenuNavigator.Menu.MAP_LEVEL_MENU);
    }

    public void Options() {
        _menuNavigator.openMenu(MenuNavigator.Menu.OPTION_MENU);
    }

    public void Credits() {
        _menuNavigator.openMenu(MenuNavigator.Menu.CREDIT_MENU);
    }

    public void ExitGame() {
        _menuNavigator.ExitGame();
    }
}
