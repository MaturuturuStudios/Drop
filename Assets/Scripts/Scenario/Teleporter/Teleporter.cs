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
	protected override void OnAction(GameObject character) {
		character.transform.position = target.position;
	}

	/// <summary>
	/// Unity's method for drawing helpers on the editor.
	/// </summary>
	void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, target.position);
	}
}