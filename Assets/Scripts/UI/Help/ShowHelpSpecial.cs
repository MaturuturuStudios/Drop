/// <summary>
/// Help component which shows or hides another game objects
/// containing the actual help. Depending on a condition, it
/// may show different information.
/// </summary>
public abstract class ShowHelpSpecial : ShowHelp {

	/// <summary>
	/// Object containing the normal help information.
	/// </summary>
	public HelpItem helpObjectNormal;

	/// <summary>
	/// Object containing the special help information.
	/// </summary>
	public HelpItem helpObjectSpecial;

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		if (_shown) {
			if (IsSpecialTriggered()) {
				helpObjectNormal.gameObject.SetActive(false);
				helpObjectSpecial.gameObject.SetActive(true);
			}
			else {
				helpObjectNormal.gameObject.SetActive(true);
				helpObjectSpecial.gameObject.SetActive(false);
			}
		}
	}

	protected override void OnShow() {
		if (IsSpecialTriggered()) {
			helpObjectNormal.Hide();
			helpObjectSpecial.Show();
		}
		else {
			helpObjectNormal.Show();
			helpObjectSpecial.Hide();
		}
	}

	protected override void OnHide() {
		helpObjectNormal.Hide();
		helpObjectSpecial.Hide();
	}

	/// <summary>
	/// Delegate for the children classes to specify the condition
	/// for the special help information to be shown.
	/// </summary>
	/// <returns>If the special information should be shown</returns>
	protected abstract bool IsSpecialTriggered();
}
