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

	void Awake() {
		HideCursor();
	}

	// Shows the cursor
	public void ShowCursor() {
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	/// <summary>
	/// Hides the cursor.
	/// </summary>
	public void HideCursor() {
		if (!alwaysVisibleOnEditor || !Application.isEditor) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}
