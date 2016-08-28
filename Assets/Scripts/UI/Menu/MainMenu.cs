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
    /// <summary>
    /// Keep a reference to the text of the new game to disable it if needed
    /// </summary>
    public GameObject buttonNewGame;
    /// <summary>
    /// Keep a reference to the text of the continue game to disable it if needed
    /// </summary>
    public GameObject buttonContinueGame;
    #endregion

    #region Private Attributes
    /// <summary>
    /// Control if I have to select a default option
    /// </summary>
    private bool _selectOption;

    private GameObject selection;
	#endregion

	#region Methods

    public void Awake() {
        selection = firstSelected;

        //check if is the first time playing the game...
        int played = PlayerPrefs.GetInt(PlayerDataStoreKeys.PlayerFirstTime, 0);
        if (played != 0) {
            //played before, change new game to continue
            buttonNewGame.SetActive(false);
            if (firstSelected == buttonNewGame) {
                selection = buttonContinueGame;
            }
        } else {
            buttonContinueGame.SetActive(false);
            if (firstSelected == buttonContinueGame) {
                selection = buttonNewGame;
            }
        }
    }

	public void OnEnable() {
        _selectOption = true;
    }

	public void Update() {
		if (_selectOption) {
			_selectOption = false;

            //the first time don't play effect
            OnSelectInvokeAudio audio= selection.GetComponent<OnSelectInvokeAudio>();
            if (audio != null)
                audio.passPlayAudio = true;

			//select the option
			EventSystem.current.SetSelectedGameObject(selection);
        }
    }
    
	
	#endregion

}
