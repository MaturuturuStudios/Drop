using UnityEngine;

/// <summary>
/// Simple script for opening and closing a door in
/// the scenario.
/// </summary>
public class DoorTrigger : MonoBehaviour {

	/// <summary>
	/// Opens the door.
	/// </summary>
	public void Open() {
		transform.Translate(new Vector3(0, 0, 5));
	}

	/// <summary>
	/// Closes the door.
	/// </summary>
	public void Close() {
		transform.Translate(new Vector3(0, 0, -5));
	}
}
