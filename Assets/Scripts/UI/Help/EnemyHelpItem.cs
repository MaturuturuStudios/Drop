using UnityEngine;

/// <summary>
/// Help item for showing enemies' information.
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class EnemyHelpItem : HelpItem {

    /// <summary>
    /// Reference to the object's TextMesh component.
    /// </summary>
	private TextMesh _textRenderer;

	/// <summary>
	/// Reference to the TextMesh Transform's component.
	/// </summary>
	private Transform _textRendererTransform;

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
		_textRenderer = GetComponent<TextMesh>();
		_textRendererTransform = _textRenderer.transform;
		_aiComponent = GetComponentInParent<AIBase>();
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();
    }

	protected override void OnUpdate() {
		// Changes the size in the text
		int size = _aiComponent.commonParameters.sizeLimitDrop;
		_textRenderer.text = size.ToString();
		_textRendererTransform.rotation = Camera.main.transform.rotation;
	}

    protected override bool IsSpecialTriggered() {
        // Compares the character's size to the enemy's limit
        int characterSize = _independentControl.currentCharacter.GetComponent<CharacterSize>().GetSize();
        int enemySizeLimit = _aiComponent.commonParameters.sizeLimitDrop;
        return characterSize >= enemySizeLimit;
    }
}
