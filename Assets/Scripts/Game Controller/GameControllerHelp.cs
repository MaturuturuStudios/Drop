using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the help elements from the scene and shows
/// and hides them depending on the input received.
/// </summary>
public class GameControllerHelp : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Time for the help to hide itself when it is
	/// toggled.
	/// </summary>
	public float autotHideTime = 5.0f;

	/// <summary>
	/// If enabled, the help will be automatically
	/// shown if the player stays without moving for
	/// a while.
	/// </summary>
	public bool autoShow = true;

	/// <summary>
	/// Time for the help to show itself when the
	/// player is not moving the character.
	/// </summary>
	public float autoShowTime = 10.0f;

	#endregion

	#region Private Attributes
	
	/// <summary>
	/// List of the help elements registed on the
	/// scene.
	/// </summary>
	private List<ShowHelp> _helpListeners = new List<ShowHelp>();

	#endregion

	#region Varaibles

	/// <summary>
	/// Flag that indicates if the help should be
	/// hidden after a while.
	/// </summary>
	private bool _autoHide;

	/// <summary>
	/// Time left for the elements to be hidden
	/// automatically.
	/// </summary>
	private float _autoHideTimer;

	/// <summary>
	/// Time left for the elements to be shown
	/// automatically.
	/// </summary>
	private float _autoShowTimer;
	
	/// <summary>
	/// Flag that indicates if the elements are
	/// being currently shown.
	/// </summary>
	private bool _helpShown;

	/// <summary>
	/// Flag that indicates if the help was shown
	/// automatically.
	/// </summary>
	private bool _autoShown;

	#endregion

	#region Methods

	/// <summary>
	/// Toggles the help elements, showing or
	/// hiding them. The elements will be shown
	/// only for a while.
	/// </summary>
	public void ToggleHelp() {
		if (!_helpShown)
			ShowHelp(autotHideTime);
		else
			HideHelp();
	}

	/// <summary>
	/// Shows the help. It won't be hidden until the
	/// hide method is called.
	/// </summary>
	public void ShowHelp() {
		_helpShown = true;
		foreach (ShowHelp listener in _helpListeners)
			listener.Show();
	}

	/// <summary>
	/// Shows the help. It will be hidden after the
	/// specified amount of time has passed.
	/// </summary>
	/// <param name="duration">Time for the help to hide itself</param>
	public void ShowHelp(float duration) {
		ShowHelp();
		_autoHideTimer = duration;
		_autoHide = true;
	}

	/// <summary>
	/// Hides the help.
	/// </summary>
	public void HideHelp() {
		_helpShown = false;
		_autoShown = false;
		_autoShowTimer = autoShowTime;
		foreach (ShowHelp listener in _helpListeners)
			listener.Hide();
	}

	/// <summary>
	/// Used by the game controller to know how long
	/// since the player has moved the character.
	/// </summary>
	/// <param name="horizontalInput">The horizontal input</param>
	/// <param name="verticalInput">The vertical input</param>
	public void UpdateAutoShow(float horizontalInput, float verticalInput) {
		if (horizontalInput != 0 || verticalInput != 0)
			if (autoShow)
				_autoShowTimer = autoShowTime;
	}

	/// <summary>
	/// Unity's method called when the component is
	/// enabled.
	/// </summary>
	void OnEnable() {
		_autoShowTimer = autoShowTime;
		_autoShown = false;
    }

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		_autoHideTimer -= Time.deltaTime;
		if (!_autoShown && _helpShown && _autoHide && _autoHideTimer < 0)
			HideHelp();

		_autoShowTimer -= Time.deltaTime;
		if (autoShow && _autoShowTimer < 0) {
			ShowHelp();
			_autoShown = true;
        }
		else if (autoShow && _autoShown && _helpShown) {
			HideHelp();
		}
	}

	/// <summary>
	/// Registers a help element.
	/// </summary>
	/// <param name="listener">The element to register</param>
	/// <returns>If the element was succesfully registered</returns>
	public bool AddListener(ShowHelp listener) {
		if (_helpListeners.Contains(listener))
			return false;
		_helpListeners.Add(listener);
		if (_helpShown)
			listener.Show();
		return true;
	}

	/// <summary>
	/// Unregisters a help element.
	/// </summary>
	/// <param name="listener">The element to unregister</param>
	/// <returns>If the element was succesfully unregistered</returns>
	public bool RemoveListener(ShowHelp listener) {
		if (!_helpListeners.Contains(listener))
			return false;
		_helpListeners.Remove(listener);
		if (!_helpShown)
			listener.Hide();
		return true;
	}

	#endregion
}
