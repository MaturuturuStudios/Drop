using UnityEngine;

/// <summary>
/// Fires events at periodic rates.
/// </summary>
public class TriggerPeriodic : MonoBehaviour {

	#region Enumerations

	/// <summary>
	/// Defines how the next event will be determined.
	/// </summary>
	public enum SelectionMode {

		/// <summary>
		/// The next event fired will be the nest one on the list.
		/// After reaching the last one, the sequence will begin again.
		/// </summary>
		Loop,

		/// <summary>
		/// The next event fired will be the nest one on the list.
		/// After reaching the last one, the sequence will be reversed.
		/// </summary>
		BackAndForward,

		/// <summary>
		/// The next event will be selected randomly.
		/// </summary>
		Random
	}

	/// <summary>
	/// Defines the delay for the next event to be fired.
	/// </summary>
	public enum DelayMode {

		/// <summary>
		/// All events will be fired after the same delay.
		/// </summary>
		Same,

		/// <summary>
		/// Each event will have it's specific delay.
		/// </summary>
		Specific
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// Defines how the next event will be determined.
	/// </summary>
	public SelectionMode selectionMode = SelectionMode.Loop;

	/// <summary>
	/// Methods invoked when the events are fired.
	/// </summary>
	public ReorderableList_MethodInvoke events = new ReorderableList_MethodInvoke();

	/// <summary>
	/// The index of the last fired event.
	/// </summary>
	public int currentEventIndex;

	/// <summary>
	/// Defines the delay for the next event to be fired.
	/// </summary>
	public DelayMode delayMode = DelayMode.Same;

	/// <summary>
	/// If the delay mode is set Same, the delay beetween
	/// firing events.
	/// </summary>
	public float commonDelay = 2.0f;

	/// <summary>
	/// If the delay mode is set Specific, the specific delay
	/// for each event.
	/// </summary>
	public float[] specificDelays;

	/// <summary>
	/// If enabled, the Gizmos will be drawn in the editor even
	/// if the entity is not selected.
	/// </summary>
	public bool drawGizmos = false;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Amount of time since the alst event was fired.
	/// </summary>
	private float _currentDelayTime;

	/// <summary>
	/// The current state of the back and forward iterator.
	/// </summary>
	private int _backAndForwardOrder;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the beginning of each frame.
	/// </summary>
	void Start() {
		// Checks if the current index is valid
		if (currentEventIndex < 0 || currentEventIndex >= events.Length) {
			Debug.LogWarning("Warning: The initial index is not valid. Selecting the first one by default.");
			currentEventIndex = 0;
		}

		// Checks if the number of events is the same as the delays
		if (delayMode == DelayMode.Specific && events.Length != specificDelays.Length) {
			Debug.LogWarning("Error: The delay mode is set to specific but there are a different amount of delays than events. Asuming 0 for each not specified.");

			// Rewriting the delays to fit the size
			float[] newDelays = new float[events.Length];
			for (int i = 0; i < newDelays.Length; i++) {
				if (i >= specificDelays.Length)
					newDelays[i] = 0;
				else
					newDelays[i] = specificDelays[i];
			}
			specificDelays = newDelays;
		}

		_currentDelayTime = 0;
    }

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Updates the timer
		_currentDelayTime += Time.deltaTime;

		// Checks if the delay time has passed
		if (_currentDelayTime >= GetCurrentDelay()) {
			Next(true, true);
		}
    }

	/// <summary>
	/// Modifies the next event to be fired.
	/// </summary>
	/// <param name="fire">If the current event should be fired</param>
	/// <param name="reset">If the delay timer should be reset</param>
	public void Next(bool fire = false, bool reset = true) {
		// Checks if the current event should be fired
		if (fire)
			events[currentEventIndex].Invoke();

		// Selectes the next event to fire
		switch (selectionMode) {
			case SelectionMode.Loop:
				currentEventIndex = (currentEventIndex + 1) % events.Length;
				break;
			case SelectionMode.BackAndForward:
				if (currentEventIndex <= 0 || currentEventIndex >= events.Length - 1)
					_backAndForwardOrder *= -1;
				currentEventIndex += _backAndForwardOrder;
				break;
			case SelectionMode.Random:
				currentEventIndex = Random.Range(0, events.Length);
				break;
			default:
				Debug.LogError("Error: Invalid selection mode: " + selectionMode);
				break;

		}

		// Checks if the delay timer should be reset
		if (reset)
			Reset();
	}

	/// <summary>
	/// Returns the delay of the current event.
	/// </summary>
	/// <returns>The dealy of the current event</returns>
	public float GetCurrentDelay() {
		if (delayMode == DelayMode.Same)
			return commonDelay;
		else
			return specificDelays[currentEventIndex];
    }

	/// <summary>
	/// Resets the current delay time.
	/// </summary>
	public void Reset() {
		_currentDelayTime = 0;
	}

	/// <summary>
	/// Modifies the enabled state of the script.
	/// </summary>
	/// <param name="enabled"></param>
	public void SetEnable(bool enabled) {
		this.enabled = enabled;
	}

	/// <summary>
	/// Unity's method called on the editor to draw helpers.
	/// </summary>
	public void OnDrawGizmos() {
		if (drawGizmos)
			OnDrawGizmosSelected();
	}

	/// <summary>
	/// Unity's method called on the editor to draw helpers only
	/// while the object is selected.
	/// </summary>
	public void OnDrawGizmosSelected() {		
		Gizmos.matrix = Matrix4x4.identity;
		Vector3 separation = new Vector3(0, 0.1f, 0);
		Gizmos.color = Color.green;
		foreach (MethodInvoke methodInvoke in events.AsList())
			DrawMethodInvoke(methodInvoke, separation);
	}

	/// <summary>
	/// Draws a line to the target of a method invoke.
	/// Also draws a rect if it's parameter is one.
	/// </summary>
	/// <param name="methodInvoke">The method invoke to draw</param>
	/// <param name="separation">The separation of the line</param>
	private void DrawMethodInvoke(MethodInvoke methodInvoke, Vector3 separation = new Vector3()) {
		if (methodInvoke.target != null) {
			Gizmos.DrawLine(transform.position + separation, methodInvoke.target.transform.position + separation);
			Color temp = Gizmos.color;
			Color newColor = Color.yellow;
			newColor.a = 0.25f;
			Gizmos.color = newColor;
			Gizmos.DrawCube(methodInvoke.RectParameter.center, methodInvoke.RectParameter.size);
			Gizmos.color = temp;
		}
	}

	#endregion
}