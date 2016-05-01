using UnityEngine;

/// <summary>
/// Base class for an ingame menu. Adds control on
/// when is the cursor shown or hidden.
/// </summary>
public class IngameMenu : MonoBehaviour {

	/// <summary>
	/// Defines if the cursor should be shown.
	/// If enabled, the cursor will be shown as soon
	/// as the player moves it.
	/// </summary>
	//protected bool EnableCursor {
		//get {
		//	return _cursorEnabled;
		//}
		//set {
		//	_cursorEnabled = value;
		//	if (!_cursorEnabled)
		//		_cursorController.HideCursor();
		//}
	//}

	/// <summary>
	/// A reference to the menu's navigator.
	/// </summary>
	protected MenuNavigator menuNavigator;

	/// <summary>
	/// Backing field for the EnableCursor property.
	/// </summary>
	private bool _cursorEnabled = false;

	/// <summary>
	/// Reference to the Game Controller's cursor controller
	/// component.
	/// </summary>
	private GameControllerCursorController _cursorController;

	/// <summary>
	/// Unity's method called after the object is created.
	/// Retrieves the desired components.
	/// </summary>
	public void Awake() {
		//_cursorController = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerCursorController>();
		menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();
	}

	/// <summary>
	/// Unity's method called each frame.
	/// Shows the cursor if the player has moved it.
	/// </summary>
	protected void Update() {
		//if (_cursorEnabled)
		//	if (Input.GetAxis(Axis.MouseX) != 0 || Input.GetAxis(Axis.MouseY) != 0)
		//		_cursorController.ShowCursor();
	}

    /// <summary>
    /// Unity's method called when this entity is enabled.
    /// Enables the cursor.
    /// </summary>
    protected void OnEnable() {
        //EnableCursor = true;
    }

    /// <summary>
    /// Unity's method called when this entity is disabled.
    /// Disables the cursor.
    /// </summary>
    protected void OnDisable() {
        //EnableCursor = false;
    }
}
