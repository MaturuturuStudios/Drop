using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class OptionsHelp : MonoBehaviour, SubOptionInterface {
    #region Public attributes
    /// <summary>
    /// Title of the panel
    /// </summary>
    public Text title;
    /// <summary>
    /// The first option to be selected
    /// </summary>
    public GameObject firstSelected;
    /// <summary>
    /// The game object with the buttons to change the mechanics
    /// </summary>
    public GameObject mechanicsButton;
    /// <summary>
    /// The game object with the panels of the mechanics
    /// </summary>
    public GameObject imagePanels;
    #endregion

    #region Private class
    /// <summary>
    /// On select level class to react a events.
    /// </summary>
    private class OnSelectMechanic : MonoBehaviour, ISelectHandler {
        /// <summary>
        /// the index of mechanic
        /// </summary>
        public int index;
        /// <summary>
        /// Script to call to
        /// </summary>
        public OptionsHelp delegateAction;

        public void OnSelect(BaseEventData eventData) {
            StartCoroutine(Delegate(eventData.selectedObject));
        }

        /// <summary>
        /// Make sure at the end of frame that the final selected level
        /// is this one. This is because when selecting a level, can be non interactable,
        /// so the event is skiping to the next one and this way we avoid animations and
        /// other things starting
        /// </summary>
        /// <param name="selected">the selected object (the level selected)</param>
        /// <returns></returns>
        private IEnumerator Delegate(GameObject selected) {
            yield return new WaitForEndOfFrame();

            if (EventSystem.current.currentSelectedGameObject == selected) {
                delegateAction.ChangeMechanic(index);
            }
        }
    }
    #endregion

    #region Private attributes
    /// <summary>
    /// Array of panels of mechanics
    /// </summary>
    private GameObject[] panelMechanic;
    /// <summary>
    /// Actual mechanic
    /// </summary>
    private int actualMechanic=-1;
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
        EventSystem.current.SetSelectedGameObject(firstSelected);
        return true;
    }

    public void LoseFocus() {
        if (title != null) {
            if (EventSystem.current.currentSelectedGameObject != null)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void Awake() {
        //Get all the mechanics buttons and theirs panels
        GetMechanicsButtons();
        GetMechanicsPanels();

        ChangeMechanic(0);
    }
    
    /// <summary>
    /// Change to the mechanic given
    /// </summary>
    /// <param name="index"></param>
    public void ChangeMechanic(int index) {
        if (index < 0 || index >= panelMechanic.Length) return;
        if (actualMechanic >= 0) panelMechanic[actualMechanic].SetActive(false);
        panelMechanic[index].SetActive(true);
        actualMechanic = index;
    }

    /// <summary>
    /// Change to next mechanic
    /// </summary>
    public void ChangeNextMechanic() {
        ChangeMechanic(actualMechanic+1);
    }

    /// <summary>
    /// Change to previous mechanic
    /// </summary>
    public void ChangePreviousMechanic() {
        ChangeMechanic(actualMechanic-1);
    }
    

    /// <summary>
    /// Get the panels to show the informacion of panels
    /// </summary>
    private void GetMechanicsPanels() {
        panelMechanic = new GameObject[this.imagePanels.transform.childCount];

        int i = 0;
        //for each mechanic, store it
        foreach (Transform childTransform in imagePanels.transform) {
            panelMechanic[i] = childTransform.gameObject;
            panelMechanic[i].SetActive(false);
            i++;
        }
    }

    /// <summary>
    /// Get all the buttons and set the onclick
    /// </summary>
    private void GetMechanicsButtons() {
        panelMechanic = new GameObject[this.mechanicsButton.transform.childCount];

        int i = 0;
        //for each mechanic
        foreach (Transform childTransform in mechanicsButton.transform) {
            GameObject aMechanic = childTransform.gameObject;
            Button button = aMechanic.GetComponent<Button>();
            button.onClick.AddListener(() => { ChangeMechanic(i); });

            //add a script to select the mechanic
            aMechanic.AddComponent(typeof(OnSelectMechanic));
            OnSelectMechanic script = aMechanic.GetComponent<OnSelectMechanic>();

            script.index = i;
            script.delegateAction = this; //script to delegate
            i++;
        }
    }
    #endregion
}
