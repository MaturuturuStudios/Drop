using UnityEngine;
using System.Collections;

public class OptionsInput : SubOption {

    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    public override GameObject GetPanel() {
        return gameObject;
    }
}