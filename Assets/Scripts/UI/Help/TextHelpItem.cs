using UnityEngine;

/// <summary>
/// Shows some text on the help item.
/// </summary>
[RequireComponent(typeof(TextMesh))]
public abstract class TextHelpItem : HelpItem {

	/// <summary>
	/// The layer for the help item's text.
	/// </summary>
	public string textLayerName = "help";

    /// <summary>
    /// Reference to the object's TextMesh component.
    /// </summary>
	private TextMesh _textRenderer;

	/// <summary>
	/// Reference to the renderer for the text.
	/// </summary>
	private MeshRenderer _renderer;

	protected override void OnAwake() {
		// Retrieves the desired components
		_textRenderer = GetComponent<TextMesh>();
		_renderer = GetComponent<MeshRenderer>();
		_renderer.sortingLayerID = SortingLayer.NameToID(textLayerName);
    }

	protected override void OnUpdate() {
		// Changes the size in the text
		_textRenderer.text = GetText();
	}

	/// <summary>
	/// Abstract method for the children classes to determine
	/// which text should be shown.
	/// </summary>
	/// <returns>The text to show on the help item</returns>
	protected abstract string GetText();
}
