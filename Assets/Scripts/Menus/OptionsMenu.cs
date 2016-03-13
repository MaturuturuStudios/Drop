using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour {
    #region Public Attributes
    public GameObject firstSelected;

    public GameObject graphics;
    public GameObject audioOptions;
    public GameObject input;
    public GameObject language;
    public GameObject help;
    #endregion

    #region Private Attributes
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
        _selectOption = true;
    }

    public void Update() {
        if (_selectOption) {
            _selectOption = false;
            //select the option
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public static IEnumerator WaitForRealSeconds(float delay) {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + delay) {
            yield return null;
        }
    }

    private void ChangePanel(GameObject panel) {
        _actualPanel.SetActive(false);
        _actualPanel = panel;
        _actualPanel.SetActive(true);
    }

    #region Options
    public void Graphics() {
        ChangePanel(graphics);
    }

    public void AudioOption() {
        ChangePanel(audioOptions);
    }

    public void Input() {
        ChangePanel(input);
    }

    public void Language() {
        ChangePanel(language);
    }

    public void Help() {
        ChangePanel(help);
    }
    #endregion

    #endregion
}
