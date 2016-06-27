using UnityEngine;

/// <summary>
/// Manages the help elements on the object.
/// </summary>
public class ShowHelp : MonoBehaviour {

	/// <summary>
	/// Default scale for the help items.
	/// </summary>
	public float helpSize = 1.0f;

    /// <summary>
    /// The game objects containing the help. They will be
    /// activated and deactivated.
    /// </summary>
    protected HelpItem[] helpObjects;

    /// <summary>
    /// Reference to the game controller's help component.
    /// </summary>
    protected GameControllerHelp _helpController;

	/// <summary>
	/// Reference to the game controller's independent control component.
	/// </summary>
	protected GameControllerIndependentControl _gcic;

	/// <summary>
	/// Reference to this entity's Transform component.
	/// </summary>
	protected Transform _transform;

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
		_transform = transform;
		_helpController = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerHelp>();
		_gcic = _helpController.GetComponent<GameControllerIndependentControl>();
		helpObjects = GetComponentsInChildren<HelpItem>();
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
	/// Unity's method called each frame.
	/// </summary>
	protected void Update() {
		// Scales the object to match the character's size
		float targetScale = helpSize * _gcic.currentCharacter.GetComponent<CharacterSize>().GetSize();
		Vector3 lossyScale = _transform.parent.lossyScale;
		_transform.localScale = new Vector3(targetScale / lossyScale.x, targetScale / lossyScale.y, targetScale / lossyScale.z);
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
		foreach (HelpItem helpObject in helpObjects)
			if (helpObject.isActiveAndEnabled)
				helpObject.Show();
	}

	/// <summary>
	/// Hides the help element.
	/// </summary>
	public void Hide() {
		foreach (HelpItem helpObject in helpObjects)
			if (helpObject.isActiveAndEnabled)
				helpObject.Hide();
    }
}
