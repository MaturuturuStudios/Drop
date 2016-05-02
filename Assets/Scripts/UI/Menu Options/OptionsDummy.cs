using UnityEngine;
using System.Collections;

public class OptionsDummy : MonoBehaviour, SubOptionInterface {
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
    public void GetFocus() {

    }
}
