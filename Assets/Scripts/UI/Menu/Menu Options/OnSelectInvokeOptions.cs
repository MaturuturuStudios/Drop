using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

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
