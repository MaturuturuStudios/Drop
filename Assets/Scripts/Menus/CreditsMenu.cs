using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsMenu : IngameMenu {
    #region Methods
    public new void Awake() {
        base.Awake();
	}

    public new void Update() {
        base.Update();
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
