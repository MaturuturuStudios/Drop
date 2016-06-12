using UnityEngine;

/// <summary>
/// Shows some text on the help item.
/// </summary>
[RequireComponent(typeof(TextMesh))]
public abstract class TextHelpItem : HelpItem {

    /// <summary>
    /// Reference to the object's TextMesh component.
    /// </summary>
	private TextMesh _textRenderer;

	/// <summary>
	/// Reference to the TextMesh Transform's component.
	/// </summary>
	private Transform _textRendererTransform;

	/// <summary>
	/// Reference to the renderer for the text.
	/// </summary>
	private MeshRenderer _renderer;

	/// <summary>
	/// The sorting layer of the text. Used to separate it
	/// from the graphic.
	/// </summary>
	private static readonly string SORTING_LAYER_NAME = "foreground";

	protected override void OnAwake() {
		// Retrieves the desired components
		_textRenderer = GetComponent<TextMesh>();
		_renderer = GetComponent<MeshRenderer>();
		_textRendererTransform = _textRenderer.transform;
		_renderer.sortingLayerID = SortingLayer.NameToID(SORTING_LAYER_NAME);
    }

	protected override void OnUpdate() {
		// Changes the size in the text
		_textRenderer.text = GetText();
		_textRendererTransform.rotation = Camera.main.transform.rotation;
	}

	/// <summary>
	/// Abstract method for the children classes to determine
	/// which text should be shown.
	/// </summary>
	/// <returns>The text to show on the help item</returns>
	protected abstract string GetText();
}
