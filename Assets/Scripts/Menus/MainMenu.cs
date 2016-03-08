using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public Scene NewGameScene;

    private MenuNavigator _menuNavigator;

    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag("Menus")
                                .GetComponent<MenuNavigator>();
    }

    public void NewGame() {
        SceneManager.LoadScene(NewGameScene.name, LoadSceneMode.Single);
    }

    //TODO
    public void LoadGame() {
        
    }

    public void Credits() {
        _menuNavigator.openMenu(MenuNavigator.Menu.CREDIT_MENU);
    }

    //TODO
    public void ExitGame() {
        _menuNavigator.ExitGame();
    }

    public void Options() {
        _menuNavigator.openMenu(MenuNavigator.Menu.CREDIT_MENU);
    }


}
