/// <summary>
/// Help item for showing irrigatable plants' information.
/// </summary>
public class IrrigateHelpItem : TextHelpItem {

	/// <summary>
	/// Reference to the parents's Irrigate component.
	/// </summary>
	private Irrigate _irrigate;

	protected new void OnAwake() {
		// Retrieves the desired components
		base.OnAwake();
		_irrigate = GetComponentInParent<Irrigate>();
    }

	protected override string GetText() {
		// Returns the amount of drops needed
		return _irrigate.dropsNeeded.ToString();
	}
}
