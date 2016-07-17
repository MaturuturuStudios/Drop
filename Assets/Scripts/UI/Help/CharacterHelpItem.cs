using UnityEngine;

/// <summary>
/// Help item for showing characters' information.
/// </summary>
public class CharacterHelpItem : TextHelpItem {

	/// <summary>
	/// Reference to the parents's CharacterSize component.
	/// </summary>
	private CharacterSize _characterSize;

	/// <summary>
	/// Renference to all the renderers on the object's hierarchy.
	/// </summary>
	private Renderer[] _renderers;

	/// <summary>
	/// Reference to the Game Controller's GameControllerIndependentControl
	/// component.
	/// </summary>
	private GameControllerIndependentControl _gcic;

	/// <summary>
	/// Reference to the character this object belongs to.
	/// </summary>
	private GameObject _character;

	/// <summary>
	/// The index this object will have when sorting
	/// it's renderers.
	/// </summary>
	private int _index;

	/// <summary>
	/// Flag used to control if the renderers have or not being
	/// already rearanged when the character is under control.
	/// </summary>
	private bool _wasControlled;

	/// <summary>
	/// Class variable. Used to ensure no two objects used the same
	/// sorting index.
	/// </summary>
	private static int nextAvailableIndex = 1;  // 0 is used for the currently controlled character

	/// <summary>
	/// Step for the increment of the index. This value should be
	/// the number of renderers the object will have.
	/// </summary>
	private static readonly int indexStep = 2;

	protected override void OnAwake() {
		base.OnAwake();

		// Retrieves the desired components
		_characterSize = GetComponentInParent<CharacterSize>();
		_renderers = GetComponentsInChildren<Renderer>();
		_gcic = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();
		_character = _characterSize.gameObject;

		// Selects an index for this object
		_index = -indexStep * nextAvailableIndex;	// The index is negative
		nextAvailableIndex++;

		// Sets the controlled flag to false by default
		_wasControlled = false;

		// Sets the renderers' order to the index
		foreach (Renderer renderer in _renderers)
			renderer.sortingOrder += _index;
	}

	protected override void OnUpdate() {
		base.OnUpdate();

		// Checks if the character is currently controlled
		if (!_wasControlled && _gcic.currentCharacter == _character) {
			// Sets the controlled flag
			_wasControlled = true;

			// Sets the renderers' order to 0
			foreach (Renderer renderer in _renderers)
				renderer.sortingOrder -= _index;
		}
		else if (_wasControlled && _gcic.currentCharacter != _character) {
			// Sets the controlled flag
			_wasControlled = false;

			// Sets the renderers' order to the index
			foreach (Renderer renderer in _renderers)
				renderer.sortingOrder += _index;
		}
	}

	protected override string GetText() {
		// Returns the character's size
		return _characterSize.GetSize().ToString();
	}
}
