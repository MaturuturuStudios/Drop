using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsAudio : MonoBehaviour, SubOptionInterface {
    #region Public attributes
    /// <summary>
    /// Title of the panel
    /// </summary>
    public Text title;
    /// <summary>
    /// A DropDown in which resolutions will show
    /// </summary>
    public Slider master;
    #endregion

    #region Methods
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
        //select the option
        EventSystem.current.SetSelectedGameObject(master.gameObject);
        if (title != null) {
            //mark title as panel selected
            title.color = Color.cyan;
        }
        return true;
    }

    public void LoseFocus() {
        if (title != null) {
            title.color = Color.white;
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void Awake() {
        if (master == null) {
            Debug.LogError("Don't have the master option!");
        }
    }


    /// <summary>
    /// When hitted, save the changes and apply them
    /// </summary>
    public void SaveChanges() {
        //write changes to file
        //come back to menu options
    }
    #endregion
}
