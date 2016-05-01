﻿using UnityEngine;
using System.Collections;

public class OptionsAudio : SubOption {

    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    public override GameObject GetPanel() {
        return gameObject;
    }

    /// <summary>
    /// Get the focus to the panel
    /// </summary>
    public override void GetFocus() {

    }
}
