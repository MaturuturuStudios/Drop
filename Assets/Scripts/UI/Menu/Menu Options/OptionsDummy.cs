using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsDummy : MonoBehaviour, SubOptionInterface {
    /// <summary>
    /// Title of the panel
    /// </summary>
    public Text title;

    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    public GameObject GetPanel() {
        return gameObject;
    }

    /// <summary>
    /// Get the focus to the panel
    /// </summary>
    public bool GetFocus() {
        return false;
    }

    public void LoseFocus() {
        if (title != null) {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
