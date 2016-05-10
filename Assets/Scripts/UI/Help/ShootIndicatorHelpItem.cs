using UnityEngine;

/// <summary>
/// Help item for showing size indicators' information.
/// </summary>
[RequireComponent(typeof(TextMesh))]
public class ShootIndicatorHelpItem : HelpItem {

    /// <summary>
    /// Reference to the object's TextMesh component.
    /// </summary>
	private TextMesh _textRenderer;

    /// <summary>
    /// Reference to the GameControllerIndependentControl component.
    /// </summary>
    private GameControllerIndependentControl _independentControl;

	protected override void OnAwake() {
		// Retrieves the desired components
		_textRenderer = GetComponent<TextMesh>();
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();
    }

	protected override void OnUpdate() {
		// Changes the size in the text
		float shootSize = _independentControl.currentCharacter.GetComponent<CharacterShootTrajectory>().shootsize;
		_textRenderer.text = shootSize.ToString();
    }
}
