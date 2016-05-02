using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {
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
    private GameObject _actualSelected;
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


    #endregion

    #region Methods
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
    }

	public void OnEnable() {
        //make sure the option is visible and running
        _actualPanel = firstPanelSelected.GetComponent<SubOptionInterface>();
        _actualPanel.GetPanel().SetActive(true);
        _actualSelected = firstSelected;
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

        //B, back or start
        if (Input.GetButtonDown(Axis.Irrigate) || Input.GetButtonDown(Axis.Back) || Input.GetButtonDown(Axis.Start))
            //check if focus is inside the suboption
            if (IsUnderSubOption())
                //if yes, put focus on the button (the actual option selected
                EventSystem.current.SetSelectedGameObject(_actualSelected);
            else
                //if not, the focus is already on the buttons menu, come back
                _menuNavigator.ComeBack();

	}

    private bool IsUnderSubOption() {
        Button currentSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        //if null, for sure is not a button on list of options
        if (currentSelected == null) return true;
        
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

	/// <summary>
	/// Change the panel, the game object is the panel with the script suboption
	/// </summary>
	/// <param name="panel">new panel</param>
	public void ChangePanel(GameObject panel) {
        //get the script
        SubOptionInterface subOption = panel.GetComponent<SubOptionInterface>();

        //unload the previous suboption and deselect the button associated
        _actualPanel.GetPanel().SetActive(false);
        _actualSelected.GetComponent<Animator>().SetBool("Setted", false);

        //store new suboption and get it setted
        _actualPanel = subOption;
        _actualSelected = EventSystem.current.currentSelectedGameObject;
        _actualPanel.GetPanel().SetActive(true);
        _actualSelected.GetComponent<Animator>().SetBool("Setted", true);
        //send the focus to the suboption panel
        _actualPanel.GetFocus();
    }

	#endregion
}
