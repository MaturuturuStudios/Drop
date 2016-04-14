using UnityEngine;

/// <summary>
/// Defines an area which will fire events when something
/// enters, stays or leaves it. It's fully configurable on
/// the editor.
/// </summary>
public class TriggerArea : MonoBehaviour {

	#region Public Attributes

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

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called at the beginning of each frame.
	/// Checks if at least one trigger is attached to the object.
	/// </summary>
	void Start() {
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
	/// Unity's method called when an entity enters the trigger
	/// area.
	/// </summary>
	/// <param name="other">The collider entering the area</param>
	void OnTriggerEnter(Collider other) {
		foreach (MethodInvoke methodInvoke in onEnter.AsList())
			methodInvoke.Invoke();
	}
	
	/// <summary>
	/// Unity's method called while an entity stays in the trigger
	/// area.
	/// </summary>
	/// <param name="other">The collider staying int the area</param>
	void OnTriggerStay(Collider other) {
		foreach (MethodInvoke methodInvoke in onStay.AsList())
			methodInvoke.Invoke();
	}

	/// <summary>
	/// Unity's method called when an entity exits the trigger
	/// area.
	/// </summary>
	/// <param name="other">The collider exiting the area</param>
		void OnTriggerExit(Collider other) {
		foreach (MethodInvoke methodInvoke in onExit.AsList())
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
		foreach (MethodInvoke methodInvoke in onEnter.AsList())
			if (methodInvoke.target != null)
				Gizmos.DrawLine(transform.position + separation, methodInvoke.target.transform.position + separation);

		Gizmos.color = Color.yellow;
		foreach (MethodInvoke methodInvoke in onStay.AsList())
			if (methodInvoke.target != null)
				Gizmos.DrawLine(transform.position, methodInvoke.target.transform.position);

		Gizmos.color = Color.red;
		foreach (MethodInvoke methodInvoke in onExit.AsList())
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