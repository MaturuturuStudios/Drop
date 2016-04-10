using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
        CREDITS_MENU,
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
    /// Seconds waiting before the reaction of a button/text action
    /// </summary>
    public float secondsReaction=0.2f;
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
    /// Canvas with the menus
    /// </summary>
	public CanvasGroup uiCanvasGroup;
    /// <summary>
    /// Canvas with the confirmation quit
    /// </summary>
	public CanvasGroup confirmQuit;
    /// <summary>
    /// The option from quit confirmation that should be the default option
    /// </summary>
    public GameObject confirmQuitDefault;

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
    /// <summary>
    /// Reference to the fading scenes
    /// </summary>
    private SceneFadeInOut _fading;
    /// <summary>
    /// Variable to know if I have to open a menu
    /// </summary>
    private Menu openMenu;
    /// <summary>
    /// The previous selected game object
    /// </summary>
    private GameObject selected;
    #endregion

    #region Methods
    public void Awake() {
        _menuPanel = new Stack<MenuInstance>();
        //open a menu if indicated
        if (startWithMenu != Menu.NONE) {
            OpenMenu(startWithMenu);
        }
        DoConfirmQuitNo();

        //get the fading
        _fading = GetComponent<SceneFadeInOut>();
    }

    public void Update() {
        //if is some menu to open, open it
        if (openMenu != Menu.NONE) {
            OpenMenu(openMenu);
            openMenu = Menu.NONE;
        }
        
        if (IsMenuActive()) {
            GameObject actualSelected = EventSystem.current.currentSelectedGameObject;

            if (actualSelected == null) {
                EventSystem.current.SetSelectedGameObject(selected);
            } else if (actualSelected != selected) {
                selected = actualSelected;
            }
        }
    }

    /// <summary>
    /// Coroutine
    /// Wait for seconds when the time scale is set to zero and need to wait that seconds anyway
    /// TODO: move to a global script functions
    /// </summary>
    /// <param name="delay">seconds to wait</param>
    /// <returns>Ienumerator</returns>
    public static IEnumerator WaitForRealSeconds(float delay) {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + delay) {
            yield return null;
        }
    }

    /// <summary>
    /// Open the main menu or a specified menu, adding it to the stack.
    /// Advice: this only open the menu and does not perform any special action like pausing the game,
    /// if so, you must use the concrete methods for it.
    /// </summary>
    /// <param name="menu">The menu to open</param>
    /// <param name="delayReaction">if want the delay for the change panel (not used the first time the menu is opened)</param>
    public void OpenMenu(Menu menu, bool delayReaction = true) {
        MenuInstance last = null;
        
        if (openMenu == Menu.NONE && delayReaction) {
            //retrieve the last menu
            if (_menuPanel.Count > 0) {
                StartCoroutine(waitReaction(menu));
                return;
            }
        }

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

        //make sure to quit the confirmation
        DoConfirmQuitNo();
    }

    /// <summary>
    /// Return to the previous menu on stack without special actions
    /// Close the menu if there is no more menus on stack
    /// </summary>
    public void ComeBack() {
        StartCoroutine(ComeBackWait());
        //make sure to quit the confirmation
        DoConfirmQuitNo();
    }
    
    /// <summary>
    /// Change the scene with fading
    /// </summary>
    /// <param name="nameScene">The next scene</param>
    /// <param name="delayStart">Delay should wait before starting. By default -1 that means the default will be used</param>
    /// <param name="delayEnd">Delay should wait after ending. By default -1 that means the default will be used</param>
    public void ChangeScene(string nameScene, float delayStart = -1, float delayEnd = -1) {
        _fading.ChangeScene(nameScene, delayStart, delayEnd);
    }

    /// <summary>
    /// Change to the given menu waiting few seconds before that
    /// </summary>
    /// <param name="menu">menu to open</param>
    /// <returns></returns>
    private IEnumerator waitReaction(Menu menu) {
        yield return WaitForRealSeconds(secondsReaction);
        openMenu = menu;
    }

    /// <summary>
    /// Come back as a routine
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator ComeBackWait() {
        yield return WaitForRealSeconds(secondsReaction);
        MenuInstance panel = _menuPanel.Pop();
        panel.disable();

        //if no more menus, close it
        if (_menuPanel.Count == 0) {
            CloseMenu();
        } else {
            panel = _menuPanel.Peek();
            panel.enable();
        }

    }

    /// <summary>
	/// Called if clicked on No (confirmation)
	/// </summary>
	public void DoConfirmQuitNo() {
        //enable the normal ui
        uiCanvasGroup.alpha = 1;
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;

        //disable the confirmation quit ui
        confirmQuit.alpha = 0;
        confirmQuit.interactable = false;
        confirmQuit.blocksRaycasts = false;

        if (_menuPanel.Count > 0) {
            _menuPanel.Peek().disable();
            _menuPanel.Peek().enable();
        }
    }

    /// <summary>
    /// Called if clicked on Yes (confirmation)
    /// </summary>
    public void DoConfirmQuitYes() {
        Application.Quit();
    }

    //Region of methods with special/specific actions when pushed or poped from stack
    #region Specifics menu actions
    public void MainMenu() {
        ChangeScene(startSceneMainMenu);
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
        //reduce the visibility of normal UI, and disable all interraction
        uiCanvasGroup.alpha = 0.3f;
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;

        //enable interraction with confirmation gui and make visible
        confirmQuit.alpha = 1;
        confirmQuit.interactable = true;
        confirmQuit.blocksRaycasts = true;


        //set selected option by default NO
        EventSystem.current.SetSelectedGameObject(confirmQuitDefault);
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
