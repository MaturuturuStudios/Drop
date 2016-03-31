using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsMenu : MonoBehaviour {
	#region Private Attributes
	/// <summary>
	/// Menu navigator
	/// </summary>
	private MenuNavigator _menuNavigator;
	#endregion

	#region Methods
	public void Awake() {
		_menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus)
			.GetComponent<MenuNavigator>();
	}

    public void Update() {
		//A
		if (Input.GetButtonDown (Axis.Jump))
			_menuNavigator.ComeBack();

		//B
		if (Input.GetButtonDown(Axis.Irrigate))
			_menuNavigator.ComeBack();

		//return
		if (Input.GetButtonDown(Axis.Back))
			_menuNavigator.ComeBack();

		//start
		if (Input.GetButtonDown(Axis.Start))
			_menuNavigator.ComeBack();
    }
	#endregion
}
