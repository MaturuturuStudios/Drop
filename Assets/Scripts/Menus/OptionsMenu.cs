using UnityEngine;
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
    /// <summary>
	/// Menu navigator
	/// </summary>
	private MenuNavigator _menuNavigator;
    #endregion

    #region Methods
    public void Awake() {
        _actualPanel = graphics;
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus)
            .GetComponent<MenuNavigator>();
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

        //B
        if (Input.GetButtonDown(Axis.Irrigate))
            _menuNavigator.ComeBack();

        //return
        if (Input.GetButtonDown(Axis.Back))
            _menuNavigator.ComeBack();

        //start
        if (Input.GetButtonDown(Axis.Start))
            _menuNavigator.ComeBack();

    }



    #region Options
    /// <summary>
    /// Change to graphics option
    /// </summary>
    public void GraphicsOption() {
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
    public void InputOption() {
        ChangePanel(input);
    }

    /// <summary>
    /// change to language options
    /// </summary>
    public void LanguageOption() {
        ChangePanel(language);
    }

    /// <summary>
    /// change to help options
    /// </summary>
    public void HelpOption() {
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
