using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenu : IngameMenu {
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
	private SubOption _actualPanel;
	/// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;
	#endregion

	#region Methods
	public new void OnEnable() {
		base.OnEnable();
        //make sure the option is visible and running
        _actualPanel = firstPanelSelected.GetComponent<SubOption>();
        _actualPanel.GetPanel().SetActive(true);
        _actualSelected = firstSelected;
        //we have to select the option in update
        _selectOption = true;
	}

	public new void Update() {
		base.Update();
		//if we have to select the option...
		if (_selectOption) {
			//only once!
			_selectOption = false;
			//select the option
			EventSystem.current.SetSelectedGameObject(firstSelected);
        }

		//B
		if (Input.GetButtonDown(Axis.Irrigate))
			menuNavigator.ComeBack();

		//return
		if (Input.GetButtonDown(Axis.Back))
			menuNavigator.ComeBack();

		//start
		if (Input.GetButtonDown(Axis.Start))
			menuNavigator.ComeBack();

	}

	/// <summary>
	/// Change the panel, the game object is the panel with the script suboption
	/// </summary>
	/// <param name="panel">new panel</param>
	public void ChangePanel(GameObject panel) {
        //get the script
        SubOption subOption = panel.GetComponent<SubOption>();

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
