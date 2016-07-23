using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightedOnSelectSubOption : MonoBehaviour, ISelectHandler, IDeselectHandler {
    public Animator asociatedTitle;

    public void OnDeselect(BaseEventData eventData) {
        asociatedTitle.SetTrigger("Normal");
    }

    public void OnSelect(BaseEventData eventData) {
        asociatedTitle.SetTrigger("Selected");
    }

    public void Disable() {
        asociatedTitle.SetTrigger("Disabled");
    }
}
