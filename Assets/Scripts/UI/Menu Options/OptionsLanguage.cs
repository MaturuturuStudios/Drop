using UnityEngine;
using System.Collections;

public class OptionsLanguage : SubOption {

    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    public override GameObject GetPanel() {
        return gameObject;
    }
}
