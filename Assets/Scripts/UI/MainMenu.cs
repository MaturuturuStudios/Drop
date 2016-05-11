using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Control all about the main menu
/// </summary>
public class MainMenu : MonoBehaviour {
	#region Public Atributes
	/// <summary>
	/// The option to be selected
	/// </summary>
	public GameObject firstSelected;
	#endregion

	#region Private Attributes
	/// <summary>
	/// Control if I have to select a default option
	/// </summary>
	private bool _selectOption;
	#endregion

	#region Methods

	public void OnEnable() {
        _selectOption = true;
	}

	public void Update() {
		if (_selectOption) {
			_selectOption = false;
			//select the option
			EventSystem.current.SetSelectedGameObject(firstSelected);
		}
	}
	
	#endregion

}
