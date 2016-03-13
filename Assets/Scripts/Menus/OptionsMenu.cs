using UnityEngine;
using System.Collections;

public class OptionsMenu : MonoBehaviour {
    #region Public Attributes
    public GameObject graphics;
    public GameObject audioOptions;
    public GameObject input;
    public GameObject language;
    public GameObject help;
    #endregion

    #region Private Attributes

    private GameObject actualPanel;
    #endregion

    #region Methods
    public void Awake() {
        actualPanel = graphics;
    }

    #region Options
    public void Graphics() {
        actualPanel.SetActive(false);
        actualPanel = graphics;
        actualPanel.SetActive(true);
    }

    public void AudioOption() {
        actualPanel.SetActive(false);
        actualPanel = audioOptions;
        actualPanel.SetActive(true);
    }

    public void Input() {
        actualPanel.SetActive(false);
        actualPanel = input;
        actualPanel.SetActive(true);
    }

    public void Language() {
        actualPanel.SetActive(false);
        actualPanel = language;
        actualPanel.SetActive(true);
    }

    public void Help() {
        actualPanel.SetActive(false);
        actualPanel = help;
        actualPanel.SetActive(true);
    }
    #endregion

    #endregion
}
