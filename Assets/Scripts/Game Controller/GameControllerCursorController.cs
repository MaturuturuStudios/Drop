using UnityEngine;

/// <summary>
/// The only responsability of this class is to hide
/// and show the cursor while the game is running.
/// This is needed as all menus are disabled when the
/// game starts.
/// </summary>
public class GameControllerCursorController : MonoBehaviour {

	/// <summary>
	/// If enabled, the cursor will not be hidden on
	/// the editor.
	/// </summary>
	public bool alwaysVisibleOnEditor = true;

	/// <summary>
	/// A reference to the canva's filter component.
	/// </summary>
	private RaycastFilter _canvasFilter;

	/// <summary>
	/// Unity's method called right after the object
	/// is created.
	/// </summary>
	void Awake() {
		_canvasFilter = GetComponentInChildren<RaycastFilter>();
		HideCursor();
	}

	/// <summary>
	/// Shows the cursor
	/// </summary>
	public void ShowCursor() {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		_canvasFilter.enabled = false;
	}

	/// <summary>
	/// Hides the cursor.
	/// </summary>
	public void HideCursor() {
		if (!alwaysVisibleOnEditor || !Application.isEditor) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			_canvasFilter.enabled = true;
		}
	}
}
