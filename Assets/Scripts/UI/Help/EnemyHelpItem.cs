using UnityEngine;

/// <summary>
/// Help item for showing enemies' information.
/// </summary>
public class EnemyHelpItem : TextHelpItem {

	/// <summary>
	/// Reference to the parents's AIBase component.
	/// </summary>
	private AIBase _aiComponent;

    /// <summary>
    /// Reference to the GameControllerIndependentControl component.
    /// </summary>
    private GameControllerIndependentControl _independentControl;

	protected override void OnAwake() {
		// Retrieves the desired components
		base.OnAwake();
		_aiComponent = GetComponentInParent<AIBase>();
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();
    }

	protected override string GetText() {
		// Changes the size in the text
		return _aiComponent.commonParameters.sizeLimitDrop.ToString();
	}

    protected override bool IsSpecialTriggered() {
        // Compares the character's size to the enemy's limit
        int characterSize = _independentControl.currentCharacter.GetComponent<CharacterSize>().GetSize();
        int enemySizeLimit = _aiComponent.commonParameters.sizeLimitDrop;
        return characterSize >= enemySizeLimit;
    }
}
