using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsMenu : MonoBehaviour {
    /// <summary>
    /// the option to be selected
    /// </summary>
    public GameObject firstSelected;
    /// <summary>
    /// Control if I have to select a default option
    /// </summary>
    private bool _selectOption;

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
}
