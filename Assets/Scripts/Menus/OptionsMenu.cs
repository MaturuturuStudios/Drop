using UnityEngine;
using System.Collections;

public class OptionsMenu : MonoBehaviour {
    #region Private Attributes
    /// <summary>
    /// The menu navigator
    /// </summary>
    private MenuNavigator _menuNavigator;
    #endregion

    #region Methods
    public void Awake() {
        _menuNavigator = GameObject.FindGameObjectWithTag("Menus")
                                .GetComponent<MenuNavigator>();
    }


    #region Options
    public void Graphics() {

    }

    public void AudioOption() {

    }

    public void Input() {

    }

    public void Language() {

    }

    public void Help() {

    }
    #endregion

    #endregion
}
