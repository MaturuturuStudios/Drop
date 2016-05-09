using UnityEngine;

/// <summary>
/// Help item for showing characters' information.
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class CharacterHelpItem : HelpItem {

    /// <summary>
    /// Reference to the object's TextMesh component.
    /// </summary>
	private TextMesh _textRenderer;

    /// <summary>
    /// Reference to the parents's CharacterSize component.
    /// </summary>
    private CharacterSize _characterSize;

	protected override void OnAwake() {
		// Retrieves the desired components
		_textRenderer = GetComponent<TextMesh>();
		_characterSize = GetComponentInParent<CharacterSize>();
    }

	protected override void OnUpdate() {
		// Changes the size in the text
		int size = _characterSize.GetSize();
		_textRenderer.text = size.ToString();
    }
}
