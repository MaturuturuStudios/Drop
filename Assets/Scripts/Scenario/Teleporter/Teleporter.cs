using UnityEngine;

/// <summary>
/// Quick prototype for the Teleporter plant.
/// </summary>
public class Teleporter : ActionPerformer {

	/// <summary>
	/// The target point the character will be moved to.
	/// </summary>
	public Transform target;

	/// <summary>
	/// Moves the character when the Action button is pressed.
	/// </summary>
	/// <param name="character">The currently controlled character</param>
	/// <returns>If the action was performed successfully</returns>
	protected override bool OnAction(GameObject character) {
		character.transform.position = target.position;
		return true;
	}

	/// <summary>
	/// Unity's method for drawing helpers on the editor.
	/// </summary>
	void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, target.position);
	}

    /// <summary>
    /// Enables or disables the script.
    /// </summary>
    /// <param name="enabled">If the script should be enabled</param>
    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }
}