using UnityEngine;
using UnityEngine.EventSystems;

public class OnSelectMainMenu : MonoBehaviour, ISelectHandler {
    /// <summary>
    /// The main menu script to call
    /// </summary>
    public MainMenu mainMenu;
    

    public void OnSelect(BaseEventData evData) {
        if (mainMenu == null) return;
        mainMenu.MoveBrushSelection(this.gameObject);
    }
}
