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
    #endregion

    #region Methods
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
        _audioMenu = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<AudioMenu>();
    }

	public void OnEnable() {
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


        //B, back or start
        if (Input.GetButtonDown(Axis.Irrigate) || Input.GetButtonDown(Axis.Back) || Input.GetButtonDown(Axis.Start)) {
            _menuNavigator.ComeBack();
            _audioMenu.PlayEffect(AudioMenuType.BACK_BUTTON);
        }

	}
    
	#endregion
}
