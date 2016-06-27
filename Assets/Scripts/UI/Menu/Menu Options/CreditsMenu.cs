using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class CreditsMenu : MonoBehaviour {
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
    #endregion

    #region Private Attributes
    private GameObject _actualMenuSelected;
	/// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;
    /// <summary>
	/// A reference to the menu's navigator.
	/// </summary>
	protected MenuNavigator _menuNavigator;

    private bool _triggerPressed;
    #endregion

    #region Methods
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
    }

	public void OnEnable() {
        if (_actualMenuSelected!=null) {
            Animator anim = _actualMenuSelected.GetComponent<Animator>();
            if(anim!=null)
                anim.SetBool("Setted", false);
        }

        //make sure the option is visible and running
        _actualMenuSelected = firstSelected;
        //we have to select the option in update
        _selectOption = true;
	}

	public void Update() {
		//if we have to select the option...
		if (_selectOption) {
			//only once!
			_selectOption = false;
			//select the option
			EventSystem.current.SetSelectedGameObject(firstSelected);
        }

		
		// Control that triggers are pressed only one time
		if (!_triggerPressed && Input.GetAxis(Axis.SelectDrop) != 0) {
			_triggerPressed = true;

            //change options with triggers
            Selectable select = null;
            Selectable actualSelected;
            bool setFocusInOption = IsUnderSubOption();
            actualSelected = _actualMenuSelected.GetComponent<Selectable>();

            //check if left or rigth
            if (Input.GetAxis(Axis.SelectDrop) > 0) {
                select = actualSelected.FindSelectableOnDown();
            } else {
                select = actualSelected.FindSelectableOnUp();
            }

            //if have a selection, select it
            if (select != null) {
                //always disable permament focus
                UnfocusOption();
                //if suboption, set the focus
                StartCoroutine(DelaySelect(select, setFocusInOption));
            }
		
        }else if (Input.GetAxis(Axis.SelectDrop) == 0)
			_triggerPressed = false;
		

        //B, back or start
        if (Input.GetButtonDown(Axis.Irrigate) || Input.GetButtonDown(Axis.Back) || Input.GetButtonDown(Axis.Start))
            //check if focus is inside the suboption
            if (IsUnderSubOption())
                //if yes, unselect the option
                UnfocusOption();
            else
                //if not, the focus is already on the buttons menu, come back
                _menuNavigator.ComeBack();

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
        Button currentSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        //if null, for sure is not a button on list of options
        if (currentSelected == null) return true;

        //if not, is a button from the menu options
        return false;
    }

    public void UnfocusOption() {
        //deselect the main option as a panel's option focused
        _actualMenuSelected.GetComponent<Animator>().SetBool("Setted", false);
        //and set the focus to the button
        EventSystem.current.SetSelectedGameObject(_actualMenuSelected);
    }

    public void FocusOption() {
        //get the option selected
        _actualMenuSelected = EventSystem.current.currentSelectedGameObject;
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
        //SubOptionInterface subOption = panel.GetComponent<SubOptionInterface>();

        //store new suboption and get it setted
        _actualMenuSelected = EventSystem.current.currentSelectedGameObject;
        //_actualMenuSelected.GetComponent<Animator>().SetBool("Setted", true);
        
    }

	#endregion
}
