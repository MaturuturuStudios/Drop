using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class CharacterHelpItem : HelpItem {

	private TextMesh _textRenderer;

	private CharacterSize _characterSize;

	protected new void OnAwake() {
		// Retrieves the desired components
		_textRenderer = GetComponent<TextMesh>();
		_characterSize = GetComponentInParent<CharacterSize>();
    }

	protected new void OnUpdate() {
		// Changes the size in the text
		int size = _characterSize.GetSize();
		_textRenderer.text = size.ToString();
    }
}
