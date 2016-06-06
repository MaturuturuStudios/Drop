using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Defines an area which will fire events when something
/// enters, stays or leaves it. It's fully configurable on
/// the editor.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerArea : MonoBehaviour {

	#region Enumerations

	/// <summary>
	/// Defines which objects will interact with the trigger.
	/// </summary>
	public enum ColliderFilter {

		/// <summary>
		/// Any object will interact with the trigger.
		/// </summary>
		AnyObject,

		/// <summary>
		/// Only player's characters will interact with the trigger.
		/// </summary>
		OnlyCharacters,

		/// <summary>
		/// Only currently controlled player's character will interact
		/// with the trigger.
		/// </summary>
		OnlyControlledCharacter
	}

	/// <summary>
	/// Defines how the area will behave.
	/// </summary>
	public enum TriggerMode {

		/// <summary>
		/// Events will be called for every object that
		/// interacts with the trigger.
		/// </summary>
		Sensor,

		/// <summary>
		/// OnExit will only be called once all the objects have
		/// left the area. OnEnter will only be called once OnExit
		/// has been called.
		/// </summary>
		Switch,

		/// <summary>
		/// Similiar to the Switch mode, but OnExit will be called
		/// once an amount of time has passed.
		/// </summary>
		TimedSwitch
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// Defines which objects will interact with the trigger.
	/// </summary>
	public ColliderFilter colliderFilter = ColliderFilter.OnlyCharacters;

	/// <summary>
	/// Defines how the area will behave.
	/// </summary>
	public TriggerMode triggerMode = TriggerMode.Switch;

	/// <summary>
	/// If the area is a switch, stores if it's been activated or not.
	/// </summary>
	public bool switchActive = false;

	/// <summary>
	/// Amount of time after which the switch will be turned off.
	/// </summary>
	public float autoSwitchTime = 0.0f;

	/// <summary>
	/// If enabled, the trigger will be disabled after the OnExit
	/// call has been performed.
	/// </summary>
	public bool autoDisable = false;

	/// <summary>
	/// If enabled, the Gizmos will be drawn in the editor even
	/// if the entity is not selected.
	/// </summary>
	public bool drawGizmos = false;

	/// <summary>
	/// Methods triggered when an object enters the area.
	/// </summary>
	public ReorderableList_MethodInvoke onEnter = new ReorderableList_MethodInvoke();

	/// <summary>
	/// Methods triggered while an object stays in the area.
	/// </summary>
	public ReorderableList_MethodInvoke onStay = new ReorderableList_MethodInvoke();

	/// <summary>
	/// Methods triggered when an object leaves the area.
	/// </summary>
	public ReorderableList_MethodInvoke onExit = new ReorderableList_MethodInvoke();

	#endregion

	#region Private Attributes

	/// <summary>
	/// References to every collider attached to the object.
	/// </summary>
	private Collider[] _colliders;

	/// <summary>
	/// Reference to the Game Controller's independent control component.
	/// </summary>
	private GameControllerIndependentControl _gameControllerIndependentControl;

	/// <summary>
	/// List of colliders currently staying on the area.
	/// </summary>
	private List<Collider> _stayingColliders;

	/// <summary>
	/// The remaining time until the switch is deactivated.
	/// </summary>
	private float _remainingTimeToDeactivateSwitch;

	/// <summary>
	/// Flag to check if the currently controlled character is in the area.
	/// </summary>
	private bool _currentCharacterInArea = false;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the beginning of each frame.
	/// Checks if at least one trigger is attached to the object.
	/// </summary>
	void Start() {
		// Initialization
		_stayingColliders = new List<Collider>();
		GameObject gameController = GameObject.FindGameObjectWithTag(Tags.GameController);
		if (gameController == null)
			Debug.LogError("Error: No Game Controller was found on the scene.");
		else {
			_gameControllerIndependentControl = gameController.GetComponent<GameControllerIndependentControl>();
			if (_gameControllerIndependentControl == null)
				Debug.LogError("Error: No Independent Control component was found in the Game Controller.");
		}

		// Checks if the colliders are valid
		_colliders = GetComponents<Collider>();
		if (_colliders.Length == 0)
			Debug.LogWarning("Warning: No collider attached to the trigger area!");
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
				DoExit();
		}

		// Removes any destroyed collider
		_stayingColliders = _stayingColliders.Where(e => e != null).ToList();

		// Checks if the currently controlled character is on the area
		if (_stayingColliders.Where(e => _gameControllerIndependentControl.currentCharacter == e.gameObject).Count() != 0) {
			if (!_currentCharacterInArea) {
				_currentCharacterInArea = true;
				if (triggerMode == TriggerMode.Sensor && colliderFilter == ColliderFilter.OnlyControlledCharacter)
					DoEnter();
			}
		}
		else {
			if (_currentCharacterInArea) {
				_currentCharacterInArea = false;
				if (triggerMode == TriggerMode.Sensor && colliderFilter == ColliderFilter.OnlyControlledCharacter)
					DoExit();
			}
		}
	}

	/// <summary>
	/// Unity's method called when an entity enters the trigger
	/// area.
	/// If the trigger is a switch it will only be called if it's
	/// deactivated.
	/// </summary>
	/// <param name="other">The collider entering the area</param>
	void OnTriggerEnter(Collider other) {
		if (!enabled)
			return;

		// If the collider is the currently controlled character, sets the flag
		if (other.gameObject == _gameControllerIndependentControl.currentCharacter)
			_currentCharacterInArea = true;

		// Checks if it's a valid collider
		if (!IsValidCollider(other))
			return;

		// Adds the collider to the list
		_stayingColliders.Add(other);

		// Checks if the enter call should be performed
		if ((triggerMode == TriggerMode.Switch || triggerMode == TriggerMode.TimedSwitch) && switchActive)
			// It will not be called if it's a switch and it's activated
			return;

		// Performs the entrance
		DoEnter();
	}
	
	/// <summary>
	/// Unity's method called while an entity stays in the trigger
	/// area.
	/// WARNING: This method will NOT be called if the trigger is a switch.
	/// </summary>
	/// <param name="other">The collider staying int the area</param>
	void OnTriggerStay(Collider other) {
		if (!enabled)
			return;

		// Checks if it's a valid collider
		if (!IsValidCollider(other))
			return;

		// This method will not be called if the trigger is a swtich
		if (triggerMode == TriggerMode.Switch || triggerMode == TriggerMode.TimedSwitch)
			return;

		// Performs the method invocations
		foreach (MethodInvoke methodInvoke in onStay.AsList())
			methodInvoke.Invoke();
	}

	/// <summary>
	/// Unity's method called when an entity exits the trigger
	/// area.
	/// If the trigger is a switch it will only be called if it's
	/// activated and there's no colliders remaining in the area.
	/// </summary>
	/// <param name="other">The collider exiting the area</param>
	void OnTriggerExit(Collider other) {
		if (!enabled)
			return;

		// If the collider is the currently controlled character, sets the flag
		if (other.gameObject == _gameControllerIndependentControl.currentCharacter)
			_currentCharacterInArea = false;

		// Checks if it's a valid collider
		if (!IsValidCollider(other))
			return;

		// Removes the collider from the list
		_stayingColliders.Remove(other);

		// Checks if the enter call should be performed
		if (triggerMode == TriggerMode.Switch) {
			// It will not be called if it's a switch and it's not activated
			if (!switchActive)
				return;

			// If there are colliders remaining in the area, the invocations will not be performed
			if (_stayingColliders.Count > 0)
				return;
		}

		// It will not be called if it's a timed switch
		if (triggerMode == TriggerMode.TimedSwitch)
			return;

		// Performs the exit
		DoExit();
	}

	/// <summary>
	/// Performs the tasks when an object succesfully enters the area.
	/// </summary>
	private void DoEnter() {
		// Activates the switch and starts the timer
		switchActive = true;
		_remainingTimeToDeactivateSwitch = autoSwitchTime;

		// Performs the method invocations
		foreach (MethodInvoke methodInvoke in onEnter.AsList())
			methodInvoke.Invoke();
	}

	/// <summary>
	/// Performs the tasks when an object succesfully exits the area.
	/// </summary>
	private void DoExit() {
		// Deactivates the trigger
		switchActive = false;

		// Performs the method invocations
		foreach (MethodInvoke methodInvoke in onExit.AsList())
			methodInvoke.Invoke();

		// Checks if the trigger should be disabled
		if (autoDisable)
			enabled = false;
	}
	
	/// <summary>
	/// Checks if a collider can interact with the trigger.
	/// </summary>
	/// <param name="other">The collider to check the interaction</param>
	/// <returns>If the collider can interact with the trigger</returns>
	private bool IsValidCollider(Collider other) {
		if (colliderFilter == ColliderFilter.AnyObject)
			return true;

		if (other.CompareTag(Tags.Player))
			if (colliderFilter == ColliderFilter.OnlyControlledCharacter)
				return _gameControllerIndependentControl.currentCharacter == other.gameObject;
			else
				return true;

		return false;
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
		foreach (MethodInvoke methodInvoke in onEnter.AsList())
			DrawMethodInvoke(methodInvoke, separation);

		if (triggerMode == TriggerMode.Sensor) {
			Gizmos.color = Color.yellow;
			foreach (MethodInvoke methodInvoke in onStay.AsList())
				DrawMethodInvoke(methodInvoke);
		}

		Gizmos.color = Color.red;
		foreach (MethodInvoke methodInvoke in onExit.AsList())
			DrawMethodInvoke(methodInvoke, -separation);

		Gizmos.matrix = transform.localToWorldMatrix;
		Color colliderColor = Color.cyan;
		colliderColor.a = 0.5f;
		Gizmos.color = colliderColor;
		foreach (Collider collider in _colliders)
			DrawCollider(collider);
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
			Gizmos.DrawCube(methodInvoke.RectParameter.center - methodInvoke.RectParameter.size / 2, methodInvoke.RectParameter.size);
			Gizmos.color = temp;
		}
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