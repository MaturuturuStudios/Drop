using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class is for the canon plant shoot
/// </summary>
[ExecuteInEditMode]
public class CannonShoot : LaunchCharacter {

	/// <summary>
	/// List of listeners registered to this component's events.
	/// </summary>
	private List<CannonShootListener> _listeners = new List<CannonShootListener>();

	/// <summary>
	/// Subscribes a listener to the components's events.
	/// Returns false if the listener was already subscribed.
	/// </summary>
	/// <param name="listener">The listener to subscribe</param>
	/// <returns>If the listener was successfully subscribed</returns>
	public bool AddListener(CannonShootListener listener) {
		if (_listeners.Contains(listener))
			return false;
		_listeners.Add(listener);
		return true;
	}

	/// <summary>
	/// Unsubscribes a listener to the components's events.
	/// Returns false if the listener wasn't subscribed yet.
	/// </summary>
	/// <param name="listener">The listener to unsubscribe</param>
	/// <returns>If the listener was successfully unsubscribed</returns>
	public bool RemoveListener(CannonShootListener listener) {
		if (!_listeners.Contains(listener))
			return false;
		_listeners.Remove(listener);
		return true;
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	public void Update(){
		transform.eulerAngles = new Vector3(0, 0, GetAngle()); //this is to face in the direction you are aming
	}

	protected override bool OnAction(GameObject character)	{
        CharacterControllerCustom ccc = character.GetComponent<CharacterControllerCustom>();

        ccc.transform.position = this.transform.position;
        ccc.Stop();
		Vector3 velocity = GetNeededVelocityVector();
        ccc.SendFlying(velocity);

		// Notifies the listeners
		foreach (CannonShootListener listener in character.GetComponents<CannonShootListener>())
			listener.OnCannonShoot(this, character, velocity);
		foreach (CannonShootListener listener in _listeners)
			listener.OnCannonShoot(this, character, velocity);

		return true;
    }

    public void ChangeAngleCannon(float newAngle) {
        SetAngle(newAngle);
    }
}
