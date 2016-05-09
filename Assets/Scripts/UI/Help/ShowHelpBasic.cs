using UnityEngine;

/// <summary>
/// Simple help component that shows and hides another
/// game object.
/// </summary>
public class ShowHelpBasic : ShowHelp {

	/// <summary>
	/// The game object containing the help. It will be
	/// activated and deactivated.
	/// </summary>
	public GameObject helpObject;

	protected override void OnShow() {
		helpObject.SetActive(true);
	}

	protected override void OnHide() {
		helpObject.SetActive(false);
	}
}
