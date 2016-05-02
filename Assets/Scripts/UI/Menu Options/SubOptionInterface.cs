using UnityEngine;

public interface SubOptionInterface {
    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    GameObject GetPanel();

    /// <summary>
    /// Get the focus to the panel
    /// </summary>
    void GetFocus();
}
