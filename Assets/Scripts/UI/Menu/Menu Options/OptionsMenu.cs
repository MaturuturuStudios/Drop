using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System;

public class OptionsMenu : MonoBehaviour, BackOption {
	#region Public Attributes
	/// <summary>
	/// first option to be selected (button)
	/// </summary>
	public GameObject firstSelected;
    /// <summary>
    /// first panel to be selected
    /// Must match with firstSelected
    /// </summary>
    public GameObject firstPanelSelected;

    public GameObject background;
    #endregion

    #region Private Attributes
    private GameObject _actualMenuSelected;
	/// <summary>
	/// the actual panel selected
	/// </summary>
	private SubOptionInterface _actualPanel;
	/// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;
    /// <summary>
	/// A reference to the menu's navigator.
	/// </summary>
	protected MenuNavigator _menuNavigator;
    /// <summary>
    /// Reference for the audio menu
    /// </summary>
    private AudioMenu _audioMenu;
    /// <summary>
    /// Check if we have the input controller (if not, we need to control it)
    /// </summary>
    private bool _hasInputController = false;
    #endregion

    #region Methods
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
        _audioMenu = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<AudioMenu>();

        GameObject[] gameController = GameObject.FindGameObjectsWithTag(Tags.GameController);
        if (gameController.Length == 0) return;
        GameControllerInput input = gameController[0].GetComponent<GameControllerInput>();
        if (input == null) return;
        _hasInputController = input.isActiveAndEnabled;
    }

	public void OnEnable() {
        if (_actualPanel != null) {
            _actualPanel.GetPanel().SetActive(false);
        }
        if (_actualMenuSelected!=null) {
            _actualMenuSelected.GetComponent<Animator>().SetBool("Setted", false);
        }

        //we have to select the option in update
        _selectOption = true;
	}

	public void Update() {
		//if we have to select the option...
		if (_selectOption) {
			//only once!
			_selectOption = false;

            //the first time don't play effect
            OnSelectInvokeAudio audio = firstSelected.GetComponent<OnSelectInvokeAudio>();
            if (audio != null)
                audio.passPlayAudio = true;

            //select the option
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        //B
        if (Input.GetButtonDown(Axis.Irrigate)
            || (!_hasInputController && Input.GetButtonDown(Axis.Back))
            || (Input.GetButtonDown(Axis.Start))) {
            //check if focus is inside the suboption
            if (IsUnderSubOption())
                //if yes, unselect the option
                UnfocusOption();
            else 
                //if not, the focus is already on the buttons menu, come back
                _menuNavigator.ComeBack();
            _audioMenu.PlayEffect(AudioMenuType.BACK_BUTTON);
            

        }
    }

	/// <summary>
	/// Delaies the select until the end of the frame.
	/// If we do not the current object will be selected instead
	/// </summary>
	/// <param name="select">Select.</param>
	private IEnumerator DelaySelect(Selectable select, bool setFocus=false){
		yield return new WaitForEndOfFrame();

        if (select != null || !select.gameObject.activeInHierarchy) {
            select.Select();
            if (setFocus) {
                FocusOption();
            }
        } else
            Debug.Log("Please make sure your explicit navigation is configured correctly.");
	}

    private bool IsUnderSubOption() {
        GameObject objectSelected = EventSystem.current.currentSelectedGameObject;
        if (objectSelected == null) return true;
        Button currentSelected = objectSelected.GetComponent<Button>();
        //if null, for sure is not a button on list of options
        if (currentSelected == null) return true;
        if (_actualPanel == null) return false;
        
        //get all buttons of the panel
        Component[] components = _actualPanel.GetPanel().GetComponentsInChildren(currentSelected.GetType(), false);
        //check if some of them is the selected button
        foreach(Component compponent in components) {
            //if is, the selected is inside the panel
            if (compponent == currentSelected) return true;
        }
        //if not, is a button from the menu options
        return false;
    }

    public void UnfocusOption() {
        //deselect the main option as a panel's option focused
        _actualMenuSelected.GetComponent<Animator>().SetBool("Setted", false);
        //hide it
        GameObject panel = _actualMenuSelected.GetComponent<OnSelectInvokeOptions>().panelToShow;
        if(panel!=null) panel.SetActive(false);
        _actualPanel.LoseFocus();
        //and set the focus to the button (not sound)
        OnSelectInvokeAudio audio = _actualMenuSelected.GetComponent<OnSelectInvokeAudio>();
        if (audio != null)
            audio.passPlayAudio = true;
        EventSystem.current.SetSelectedGameObject(_actualMenuSelected);

        background.SetActive(false);
    }

    public void FocusOption() {
        //get the option selected
        _actualMenuSelected = EventSystem.current.currentSelectedGameObject;

        //show the panel
        OnSelectInvokeOptions invoke = _actualMenuSelected.GetComponent<OnSelectInvokeOptions>();
        invoke.ChangePanel();

        //send the focus to the suboption panel
        //set the focus on an element of the panel and get its title as panel under focus
        if (_actualPanel.GetFocus()) {
            //if there is something to focus, mark it as selected (focusing in its panel)
            _actualMenuSelected.GetComponent<Animator>().SetBool("Setted", true);
        }
    }

	/// <summary>
	/// Change the panel, the game object is the panel with the script suboption
	/// </summary>
	/// <param name="panel">new panel</param>
	public void ChangePanel(GameObject panel) {
        if (panel == null) {
            //option nullable (come back) only set the focus
            _actualMenuSelected = EventSystem.current.currentSelectedGameObject;
            return;
        }
        //get the script
        SubOptionInterface subOption = panel.GetComponent<SubOptionInterface>();

        background.SetActive(true);

        //store new suboption and get it setted
        _actualPanel = subOption;
        _actualMenuSelected = EventSystem.current.currentSelectedGameObject;
        _actualPanel.GetPanel().SetActive(true);
        
    }

    /// <summary>
    /// Tell the action back is taked in this script, so game controller input must do nothing
    /// here
    /// </summary>
    /// <returns></returns>
    public bool BackTaked() {
        _audioMenu.PlayEffect(AudioMenuType.BACK_BUTTON);
        //check if focus is inside the suboption
        if (IsUnderSubOption()) {
            //if yes, unselect the option
            UnfocusOption();
            return true;
        } else {
            //if not, the focus is already on the buttons menu, come back (return false)
            return false;
        }
        
    }

    #endregion
}
