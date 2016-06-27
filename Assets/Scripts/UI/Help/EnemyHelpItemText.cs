using UnityEngine;

/// <summary>
/// Help item for showing enemies' information.
/// </summary>
public class EnemyHelpItemText : TextHelpItem, EnemyBehaviourListener {

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

		// Registers itself to the AIBase events
		_aiComponent.AddListener(this);
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

	public void OnBeginChase(AIBase enemy, GameObject chasedObject) {
		// Do nothing
	}

	public void OnEndChase(AIBase enemy, GameObject chasedObject) {
		// Do nothing
	}

	public void OnAttack(AIBase enemy, GameObject attackedObject, Vector3 velocity) {
		// Do nothing
	}

	public void OnBeingScared(AIBase enemy, GameObject scaringObject, int scaringSize) {
		// Disables the help item
		enabled = false;
		Hide();
	}

	public void OnStateAnimationChange(AnimationState previousState, AnimationState actualState) {
		// Do nothing
	}
}
