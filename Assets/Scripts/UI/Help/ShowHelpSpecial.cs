using UnityEngine;

/// <summary>
/// Help component which shows or hides another game objects
/// containing the actual help. Depending on a condition, it
/// may show different information.
/// </summary>
public abstract class ShowHelpSpecial : ShowHelp {

	/// <summary>
	/// Object containing the normal help information.
	/// </summary>
	public GameObject helpObjectNormal;

	/// <summary>
	/// Object containing the special help information.
	/// </summary>
	public GameObject helpObjectSpecial;

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		if (_shown)
			OnShow();
	}

	protected override void OnShow() {
		if (IsSpecialTriggered()) {
			helpObjectNormal.SetActive(false);
			helpObjectSpecial.SetActive(true);
		}
		else {
			helpObjectNormal.SetActive(true);
			helpObjectSpecial.SetActive(false);
		}
	}

	protected override void OnHide() {
		helpObjectNormal.SetActive(false);
		helpObjectSpecial.SetActive(false);
	}

	/// <summary>
	/// Delegate for the children classes to specify the condition
	/// for the special help information to be shown.
	/// </summary>
	/// <returns>If the special information should be shown</returns>
	protected abstract bool IsSpecialTriggered();
}
