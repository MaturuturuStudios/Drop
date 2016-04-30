using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct OptionsAudio {
    public GameObject panel;
}

[System.Serializable]
public struct OptionsInput {
    public GameObject panel;
}

[System.Serializable]
public struct OptionsLanguage {
    public GameObject panel;
}

[System.Serializable]
public struct OptionsHelp {
    public GameObject panel;
}

public class OptionsMenu : IngameMenu {
	#region Public Attributes
	/// <summary>
	/// first option to be selected
	/// </summary>
	public GameObject firstSelected;
    public GameObject firstPanelSelected;
	#endregion

	#region Private Attributes
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
        //get the script and get it running
        SubOption subOption = panel.GetComponent<SubOption>();

        //unload the previous suboption
        _actualPanel.GetPanel().SetActive(false);

        //store new suboption
        _actualPanel = subOption;
        _actualPanel.GetPanel().SetActive(true);

        

        
	}

	#endregion
}
