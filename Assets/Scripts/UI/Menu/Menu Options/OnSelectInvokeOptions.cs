using UnityEngine;
using UnityEngine.EventSystems;

public class OnSelectInvokeOptions : MonoBehaviour, ISelectHandler{
    public GameObject panelToShow;
    public OptionsMenu menuOptions;
    
    public void ChangePanel() {
        menuOptions.ChangePanel(panelToShow);
    }

    public void OnSelect(BaseEventData eventData) {
        //ChangePanel();
    }
}
