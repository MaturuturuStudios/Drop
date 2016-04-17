using UnityEngine;

/// <summary>
/// Simple script for opening and closing a door in
/// the scenario.
/// </summary>
public class DoorTriggerVertical : MonoBehaviour {

    private bool isopen = false;
	/// <summary>
	/// Opens the door.
	/// </summary>
	public void Open() {
        if (!isopen)
        {
            transform.Translate(new Vector3(0, 10, 0));
            isopen = true;
        }              
	}

	/// <summary>
	/// Closes the door.
	/// </summary>
	public void Close() {
        if (isopen)
        {
            transform.Translate(new Vector3(0, -10, 0));
            isopen = false;
        }
	}
}
