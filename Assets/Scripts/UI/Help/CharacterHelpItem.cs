/// <summary>
/// Help item for showing characters' information.
/// </summary>
public class CharacterHelpItem : TextHelpItem {

	/// <summary>
	/// Reference to the parents's CharacterSize component.
	/// </summary>
	private CharacterSize _characterSize;

	protected new void OnAwake() {
		// Retrieves the desired components
		base.OnAwake();
		_characterSize = GetComponentInParent<CharacterSize>();
    }

	protected override string GetText() {
		// Returns the character's size
		return _characterSize.GetSize().ToString();
	}
}
