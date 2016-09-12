using UnityEngine;

/// <summary>
/// Interface for scripts that perform an action when the
/// player presses the action button on their volume.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class ActionPerformer : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// If enabled, this script will be disabled after the
	/// DoAction call.
	/// </summary>
	public bool autoDisable;

	/// <summary>
	/// If enabled, the action will be performed only once per
	/// frame. Usefull if the volume is formed by 2 or more
	/// shapes and the character overlaps whith more than one.
	/// </summary>
	public bool onlyOncePerFrame;

	#endregion

	#region Variables

	/// <summary>
	/// Flag to store if the action has been already called this
	/// frame.
	/// </summary>
	private bool _alreadyCalledThisFrame = false;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called each frame after the Update call.
	/// </summary>
	void LateUpdate() {
		// Resets the already call flag
		_alreadyCalledThisFrame = false;
	}

	/// <summary>
	/// Performs the action. Relies on an abstract method to
	/// allow it's children to modify it's behaviour.
	/// </summary>
	/// <param name="character"></param>
	/// <returns>If the action succeeded or not</returns>
	public bool DoAction(GameObject character) {
		// Only performs the action if this component is enabled
		if (!enabled)
			return false;

		// If the only once call flag is set, checks if it's been already called this frame
		if (onlyOncePerFrame && _alreadyCalledThisFrame)
			return false;
		_alreadyCalledThisFrame = true;

		// Performs the action
		bool success = OnAction(character);

		// If the auto disable flag is set, disables itself
		if (autoDisable && success)
			enabled = false;
		return success;
	}

	#endregion

	#region Abstract Methods

	/// <summary>
	/// Performs an action when the player presses the
	/// action button while staying on this object's volume.
	/// </summary>
	/// <param name="character">The character staying on the volume</param>
	/// <returns>If the action was performed succesfully</returns>
	protected abstract bool OnAction(GameObject character);

	#endregion
}
