/// <summary>
/// Simple help component that shows and hides another
/// game object.
/// </summary>
public class ShowHelpBasic : ShowHelp {

	/// <summary>
	/// The game object containing the help. It will be
	/// activated and deactivated.
	/// </summary>
	public HelpItem helpObject;

	protected override void OnShow() {
		helpObject.Show();
	}

	protected override void OnHide() {
		helpObject.Hide();
	}
}
