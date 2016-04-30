using UnityEngine;

public class CreditsMenu : IngameMenu {
	#region Methods

	public new void Update() {
		base.Update();
		//A
		if (Input.GetButtonDown (Axis.Jump))
			menuNavigator.ComeBack();

		//B
		if (Input.GetButtonDown(Axis.Irrigate))
			menuNavigator.ComeBack();

		//return
		if (Input.GetButtonDown(Axis.Back))
			menuNavigator.ComeBack();

		//start
		if (Input.GetButtonDown(Axis.Start))
			menuNavigator.ComeBack();
	}
	#endregion
}
