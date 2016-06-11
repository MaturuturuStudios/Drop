using UnityEngine;
/// <summary>
/// Help item for showing irrigatable plants' information.
/// </summary>
public class IrrigateHelpItem : TextHelpItem {

	/// <summary>
	/// Reference to the parents's Irrigate component.
	/// </summary>
	private Irrigate _irrigate;

	void Start() {
		// Retrieves the desired components
		// This needs to be done on the start method since this object
		// is going to be instantiated
		_irrigate = GetComponentInParent<Irrigate>();
	}

	protected override string GetText() {
		// Returns the amount of drops needed
		return _irrigate.dropsNeeded.ToString();
	}
}
