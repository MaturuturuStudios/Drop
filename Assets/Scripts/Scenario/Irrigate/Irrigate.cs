using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Allows an object to be irrigated, consuming drops and
/// performing a task in consequence.
/// This is an abstract class. Classes which extend this class
/// will define their own behaviour when irrigated.
/// </summary>
abstract public class Irrigate : ActionPerformer {

    #region Public Attributes

    /// <summary>
    /// Defines the number of drops needed to activate the event.
    /// </summary> 
    public int dropsNeeded;

	/// <summary>
	/// The prefab used for showing the help information on this object.
	/// </summary>
	public GameObject helpPrefab;

	#endregion

	#region Private Attributes

	/// <summary>
	/// List of listeners registered to this component's events.
	/// </summary>
	private List<IrrigateListener> _listeners = new List<IrrigateListener>();

	#endregion

	#region Methods

	/// <summary>
	/// Subscribes a listener to the components's events.
	/// Returns false if the listener was already subscribed.
	/// </summary>
	/// <param name="listener">The listener to subscribe</param>
	/// <returns>If the listener was successfully subscribed</returns>
	public bool AddListener(IrrigateListener listener) {
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
	public bool RemoveListener(IrrigateListener listener) {
		if (!_listeners.Contains(listener))
			return false;
		_listeners.Remove(listener);
		return true;
	}

	void Start() {
		// Creates the help item and adds parents it to the object
		if (helpPrefab == null)
			return;
		GameObject helpInstance = Instantiate(helpPrefab);
		helpInstance.transform.parent = transform;
		helpInstance.transform.localPosition = Vector3.zero;
    }

    protected override bool OnAction(GameObject character) {
        CharacterControllerCustom ccc = character.GetComponent<CharacterControllerCustom>();
		CharacterSize cs = character.GetComponent<CharacterSize>();
        if (ccc != null && cs != null)
            if (cs.GetSize() > dropsNeeded) {
				// Substracts the amount of drops consumed
                cs.SetSize(cs.GetSize() - dropsNeeded);

				// Calles the delegate
                OnIrrigate();

				// Notifies the listeners
				foreach (IrrigateListener listener in character.GetComponents<IrrigateListener>())
					listener.OnIrrigate(this, character, dropsNeeded);
				foreach (IrrigateListener listener in _listeners)
					listener.OnIrrigate(this, character, dropsNeeded);

				return true;
            }
		return false;
	}

	/// <summary>
	/// Delegate method. Defines how will the object behave when
	/// it is irrigated. This method will only be called once.
	/// </summary>
	protected abstract void OnIrrigate();

	#endregion
}
