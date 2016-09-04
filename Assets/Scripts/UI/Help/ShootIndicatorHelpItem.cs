using UnityEngine;

/// <summary>
/// Help item for showing size indicators' information.
/// </summary>
public class ShootIndicatorHelpItem : TextHelpItem {

    /// <summary>
    /// Reference to the GameControllerIndependentControl component.
    /// </summary>
    private GameControllerIndependentControl _independentControl;

	protected override void OnAwake() {
		// Retrieves the desired components
		base.OnAwake();
		_independentControl = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();
    }

	protected override string GetText() {
		// Changes the size in the text
		return _independentControl.currentCharacter.GetComponent<CharacterShootTrajectory>().shootSize.ToString();
    }
}
