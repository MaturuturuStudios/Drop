using UnityEngine;

/// <summary>
/// Help item for showing irrigatable plants' information.
/// </summary>
public class IrrigateHelpItem : TextHelpItem, IrrigateListener {

	/// <summary>
	/// Reference to the parents's Irrigate component.
	/// </summary>
	private Irrigate _irrigate;

	/// <summary>
	/// Reference to the GameControllerIndependentControl component.
	/// </summary>
	private GameControllerIndependentControl _independentControl;

	void Start() {
		// Retrieves the desired components
		// This needs to be done on the start method since this object
		// is going to be instantiated
		_irrigate = GetComponentInParent<Irrigate>();
		_independentControl = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();

		// Registers itself to the Irrigate events
		_irrigate.AddListener(this);
    }

	protected override string GetText() {
		// Returns the amount of drops needed
		return _irrigate.dropsNeeded.ToString();
	}

	protected override bool IsSpecialTriggered() {
		// Compares the character's size to the amount needed
		int characterSize = _independentControl.currentCharacter.GetComponent<CharacterSize>().GetSize();
		int dropsNeeded = _irrigate.dropsNeeded;
		return characterSize >= dropsNeeded;
	}

	public void OnIrrigate(Irrigate irrigated, GameObject irrigating, int dropsConsumed) {
		// Disables the help item
		enabled = false;
		Hide();
	}
}
