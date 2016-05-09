using UnityEngine;

/// <summary>
/// Manages the help elements on the object.
/// </summary>
public abstract class ShowHelp : MonoBehaviour {

	/// <summary>
	/// Reference to the game controller's help component.
	/// </summary>
	protected GameControllerHelp _helpController;

	protected bool _shown;

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	protected void Awake() {
		// Retrieves the desired components
		_helpController = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerHelp>();
	}

	/// <summary>
	/// Unity's method called at the beginning of the first frame
	/// this object is active.
	/// </summary>
	protected void Start() {
		// Hides the help by default
		Hide();
	}

	/// <summary>
	/// Unity's method called when this object becomes active.
	/// </summary>
	protected void OnEnable() {
		// Subscribes itself to the help controller
		_helpController.AddListener(this);
	}

	/// <summary>
	/// Unity's method called when this object becomes inactive.
	/// </summary>
	protected void OnDisable() {
		// Unsubscribes itself to the help controller
		_helpController.RemoveListener(this);
	}

	/// <summary>
	/// Shows the help element.
	/// </summary>
	public void Show() {
		_shown = true;
		OnShow();
	}

	/// <summary>
	/// Hides the help element.
	/// </summary>
	public void Hide() {
		_shown = false;
		OnHide();
	}

	/// <summary>
	/// Delegate for the child classes to specify how the help
	/// is shown.
	/// </summary>
	protected abstract void OnShow();
	
	/// <summary>
	/// Delegate for the child classes to specify how the help
	/// is hidden.
	/// </summary>
	protected abstract void OnHide();
}
