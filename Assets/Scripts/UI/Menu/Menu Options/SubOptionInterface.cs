using UnityEngine;

public interface SubOptionInterface {
    /// <summary>
    /// Get the panel of this option (itself)
    /// </summary>
    /// <returns></returns>
    GameObject GetPanel();

    /// <summary>
    /// Get the focus to the panel (mark the title as panel under focus)
    /// </summary>
    /// <returns>true if have a focus, false if there is nothing to focus</returns>
    bool GetFocus();

    /// <summary>
    /// Set the panel as not focused and unselect focus
    /// </summary>
    void LoseFocus();
}
