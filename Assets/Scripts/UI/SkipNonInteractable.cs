using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Skip non interactable.
/// Original author: user TechCor forum.unity3d.com/members/techcor.811569
/// </summary>
public class SkipNonInteractable : MonoBehaviour, ISelectHandler {

	private Selectable _selectable;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake () {
		_selectable = GetComponent<Selectable>();
	}

	/// <summary>
	/// Raises the select event.
	/// </summary>
	/// <param name="evData">Ev data.</param>
	public void OnSelect(BaseEventData evData){
		if (_selectable.interactable) return;

		if (Input.GetAxis("Horizontal") < 0) {
			Selectable select = _selectable.FindSelectableOnLeft();
			if (select == null || !select.gameObject.activeInHierarchy)
				select = _selectable.FindSelectableOnRight();
			
			StartCoroutine(DelaySelect(select));
		
		}else if (Input.GetAxis("Horizontal") > 0) {
			Selectable select = _selectable.FindSelectableOnRight();
			if (select == null || !select.gameObject.activeInHierarchy)
				select = _selectable.FindSelectableOnLeft();

			StartCoroutine(DelaySelect (select));
		
		} else if (Input.GetAxis("Vertical") < 0) {
			Selectable select = _selectable.FindSelectableOnDown();
			if (select == null || !select.gameObject.activeInHierarchy)
				select = _selectable.FindSelectableOnUp();
			
			StartCoroutine(DelaySelect(select));
		
		} else if (Input.GetAxis("Vertical") > 0) {
			Selectable select = _selectable.FindSelectableOnUp();
			if (select == null || !select.gameObject.activeInHierarchy)
				select = _selectable.FindSelectableOnDown();

			StartCoroutine(DelaySelect(select));
		}	
	
	}

	/// <summary>
	/// Delaies the select until the end of the frame.
	/// If we do not the current object will be selected instead
	/// </summary>
	/// <param name="select">Select.</param>
	private IEnumerator DelaySelect(Selectable select){
		yield return new WaitForEndOfFrame();

		if (select != null || !select.gameObject.activeInHierarchy)
			select.Select();
		else
			Debug.Log ("Please make sure your explicit navigation is configured correctly.");
	}
}
