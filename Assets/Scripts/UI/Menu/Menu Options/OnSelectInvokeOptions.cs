using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class OnSelectInvokeOptions : MonoBehaviour, ISelectHandler{
    public GameObject panelToShow;
    public OptionsMenu menuOptions;
    
    public void OnSelect(BaseEventData eventData) {
        menuOptions.ChangePanel(panelToShow);
    }
}
