using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Fires events when a certain object is irrigated.
/// It's fully configurable on the editor.
/// </summary>
public class TriggerIrrigate : Irrigate {

	#region Public Attributes

	/// <summary>
	/// If enabled, the Gizmos will be drawn in the editor even
	/// if the entity is not selected.
	/// </summary>
	public bool drawGizmos = false;

	/// <summary>
	/// Events fired when the trigger is activated.
	/// </summary>
	public UnityEvent onIrrigate;

    #endregion

    #region Methods

    /// <summary>
    /// Fires the events when the object is irrigated.
    /// </summary>
    protected override void OnIrrigate() {
		// Performs the method invocations
		onIrrigate.Invoke();
    }

	#endregion
}