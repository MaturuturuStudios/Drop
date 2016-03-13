using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour {
    #region Public Attributes
    /// <summary>
    /// first option to be selected
    /// </summary>
    public GameObject firstSelected;

    /// <summary>
    /// graphics panel
    /// </summary>
    public GameObject graphics;
    /// <summary>
    /// audio panel
    /// </summary>
    public GameObject audioOptions;
    /// <summary>
    /// input panel
    /// </summary>
    public GameObject input;
    /// <summary>
    /// language panel
    /// </summary>
    public GameObject language;
    /// <summary>
    /// help panel
    /// </summary>
    public GameObject help;
    #endregion

    #region Private Attributes
    /// <summary>
    /// the actual panel selected
    /// </summary>
    private GameObject _actualPanel;
    /// <summary>
    /// Control if I have to select a default option
    /// </summary>
    private bool _selectOption;
    #endregion

    #region Methods
    public void Awake() {
        _actualPanel = graphics;
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
            //select the option
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }



    #region Options
    /// <summary>
    /// Change to graphics option
    /// </summary>
    public void Graphics() {
        ChangePanel(graphics);
    }

    /// <summary>
    /// change to audio options
    /// </summary>
    public void AudioOption() {
        ChangePanel(audioOptions);
    }

    /// <summary>
    /// change to input options
    /// </summary>
    public void Input() {
        ChangePanel(input);
    }

    /// <summary>
    /// change to language options
    /// </summary>
    public void Language() {
        ChangePanel(language);
    }

    /// <summary>
    /// change to help options
    /// </summary>
    public void Help() {
        ChangePanel(help);
    }

    /// <summary>
    /// Change the panel
    /// </summary>
    /// <param name="panel">new panel</param>
    private void ChangePanel(GameObject panel) {
        _actualPanel.SetActive(false);
        _actualPanel = panel;
        _actualPanel.SetActive(true);
    }
    #endregion

    #endregion
}
