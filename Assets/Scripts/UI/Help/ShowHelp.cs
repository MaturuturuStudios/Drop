using UnityEngine;

/// <summary>
/// Manages the help elements on the object.
/// </summary>
public class ShowHelp : MonoBehaviour {

    /// <summary>
    /// The game object containing the help. It will be
    /// activated and deactivated.
    /// </summary>
    public HelpItem helpObject;

    /// <summary>
    /// Reference to the game controller's help component.
    /// </summary>
    protected GameControllerHelp _helpController;

	/// <summary>
	/// Flag that indicates the object has already made it's
	/// Start call.
	/// </summary>
	protected bool _started = false;

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
		// Subscribes itself to the help controller
		_helpController.AddListener(this);
		_started = true;
    }

	/// <summary>
	/// Unity's method called when this object becomes active.
	/// </summary>
	protected void OnEnable() {
		// Subscribes itself to the help controller
		if (_started)
			_helpController.AddListener(this);
	}

	/// <summary>
	/// Unity's method called when this object becomes inactive.
	/// </summary>
	protected void OnDisable() {
		// Unsubscribes itself to the help controller
		if (_started)
			_helpController.RemoveListener(this);
	}

	/// <summary>
	/// Shows the help element.
	/// </summary>
	public void Show() {
        helpObject.Show();
	}

	/// <summary>
	/// Hides the help element.
	/// </summary>
	public void Hide() {
        helpObject.Hide();
    }
}
