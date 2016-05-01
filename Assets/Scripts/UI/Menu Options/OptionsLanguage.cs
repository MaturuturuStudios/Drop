using UnityEngine;
using System.Collections;

public class OptionsLanguage : SubOption {
    /// <summary>
    /// Get the focus to the panel
    /// </summary>
    public new void GetFocus() {
        Debug.Log(Application.systemLanguage);
    }
}
