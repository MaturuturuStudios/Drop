using UnityEngine;

/// <summary>
/// Defines an area which will fire events when the player presses
/// the Action button on it. It's fully configurable on the editor.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerAction : ActionPerformer {

	#region Enumerations

	/// <summary>
	/// Defines how the trigger will behave.
	/// </summary>
	public enum TriggerMode {

		/// <summary>
		/// The trigger has only one state and will fire the same
		/// events every time it's used.
		/// </summary>
		Button,

		/// <summary>
		/// The trigger has two states and will alternate beetween
		/// them each time.
		/// </summary>
		Switch,

		/// <summary>
		/// Similiar to the Switch mode, but the state alternation
		/// will happen after a while.
		/// </summary>
		TimedSwitch
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// Defines how the trigger will behave.
	/// </summary>
	public TriggerMode triggerMode = TriggerMode.Switch;

	/// <summary>
	/// If the area is a switch, stores if it's been activated or not.
	/// </summary>
	public bool switchActive = false;

	/// <summary>
	/// Amount of time after which the switch will be turned off.
	/// </summary>
	public float switchTime = 0.0f;

	/// <summary>
	/// If enabled, the Gizmos will be drawn in the editor even
	/// if the entity is not selected.
	/// </summary>
	public bool drawGizmos = false;

	/// <summary>
	/// Methods triggered when the trigger is activated.
	/// </summary>
	public ReorderableList_MethodInvoke onActivate = new ReorderableList_MethodInvoke();

	/// <summary>
	/// Methods triggered when the trigger is deactivated.
	/// </summary>
	public ReorderableList_MethodInvoke onDeactivate = new ReorderableList_MethodInvoke();

	#endregion

	#region Private Attributes

	/// <summary>
	/// References to every collider attached to the object.
	/// </summary>
	private Collider[] _colliders;

	/// <summary>
	/// The remaining time until the switch is deactivated.
	/// </summary>
	private float _remainingTimeToDeactivateSwitch;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the beginning of each frame.
	/// Checks if at least one trigger is attached to the object.
	/// </summary>
	void Start() {
		// Checks if the colliders are valid
		_colliders = GetComponents<Collider>();
		if (_colliders.Length == 0)
			Debug.LogWarning("Warning: No collider attached to the trigger!");
		else {
			bool atLeastOneTrigger = false;
			foreach (Collider collider in _colliders)
				if (collider.isTrigger) {
					atLeastOneTrigger = true;
					break;
				}
			if (!atLeastOneTrigger)
				Debug.LogWarning("Warning: None of the attached colliders is a trigger!");
		}
	}

	/// <summary>
	/// Unity's method called each frame.
	/// </summary>
	void Update() {
		// Checks if the switch should be deactivated
		if (switchActive && triggerMode == TriggerMode.TimedSwitch) {
			_remainingTimeToDeactivateSwitch -= Time.deltaTime;

			if (_remainingTimeToDeactivateSwitch <= 0)
				Deactivate();
		}
	}

	protected override void OnAction(GameObject character) {
		switch (triggerMode) {
			case TriggerMode.Button:
				// Always calls the activatation methods
				Activate();
				break;
			case TriggerMode.Switch:
				// Checks for the trigger state
				if (switchActive)
					Deactivate();
				else
					Activate();
				break;
			case TriggerMode.TimedSwitch:
				// The button press only allows to activate the trigger
				if (!switchActive)
					Activate();
				break;
			default:
				Debug.LogError("Error: Unrecognized trigger mode: " + triggerMode);
				break;
		}
	}
	
	private void Activate() {
		// Activates the switch and starts the timer
		switchActive = true;
		_remainingTimeToDeactivateSwitch = switchTime;

		// Performs the method invocations
		foreach (MethodInvoke methodInvoke in onActivate.AsList())
			methodInvoke.Invoke();
	}
	
	private void Deactivate() {
		// Deactivates the trigger
		switchActive = false;

		// Performs the method invocations
		foreach (MethodInvoke methodInvoke in onDeactivate.AsList())
			methodInvoke.Invoke();
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
		if (!Application.isPlaying)
			Start();
		
		Gizmos.matrix = Matrix4x4.identity;
		Vector3 separation = new Vector3(0, 0.1f, 0);
		Gizmos.color = Color.green;
		foreach (MethodInvoke methodInvoke in onActivate.AsList())
			if (methodInvoke.target != null)
				Gizmos.DrawLine(transform.position + separation, methodInvoke.target.transform.position + separation);

		Gizmos.color = Color.red;
		foreach (MethodInvoke methodInvoke in onDeactivate.AsList())
			if (methodInvoke.target != null)
				Gizmos.DrawLine(transform.position - separation, methodInvoke.target.transform.position - separation);

		Gizmos.matrix = transform.localToWorldMatrix;
		Color colliderColor = Color.cyan;
		colliderColor.a = 0.5f;
		Gizmos.color = colliderColor;
		foreach (Collider collider in _colliders)
			DrawCollider(collider);
	}

	/// <summary>
	/// Draws a gizmos in the shape of a collider.
	/// Supports Box, Sphere and Mesh colliders.
	/// </summary>
	/// <param name="collider">The collider to draw</param>
	private void DrawCollider(Collider collider) {
		if (collider is BoxCollider) {
			BoxCollider box = (BoxCollider) collider;
			Gizmos.DrawWireCube(box.center, box.size);
		}
		else if (collider is SphereCollider) {
			SphereCollider sphere = (SphereCollider)collider;
			Gizmos.DrawWireSphere(sphere.center, sphere.radius);
		}
		else if (collider is MeshCollider) {
			MeshCollider mesh = (MeshCollider)collider;
			Gizmos.DrawWireMesh(mesh.sharedMesh);
		}
	}

	#endregion
}