using UnityEngine;

public class TriggerArea : MonoBehaviour {
	
	public bool drawGizmos = false;

	public ReorderableList_MethodInvoke onEnter;
	public ReorderableList_MethodInvoke onStay;
	public ReorderableList_MethodInvoke onExit;

	private Collider[] _colliders;

	void Start() {
		_colliders = GetComponents<Collider>();
		if (_colliders.Length == 0)
			Debug.LogError("Warning: No collider attached to the trigger area!");
		else {
			bool atLeastOneTrigger = false;
			foreach (Collider collider in _colliders)
				if (collider.isTrigger) {
					atLeastOneTrigger = true;
					break;
				}
			if (!atLeastOneTrigger)
				Debug.LogError("Warning: None of the attached colliders is a trigger!");
		}
	}

	void OnTriggerEnter(Collider other) {
		foreach (MethodInvoke methodInvoke in onEnter.AsList())
			methodInvoke.Invoke();
	}

	void OnTriggerStay(Collider other) {
		foreach (MethodInvoke methodInvoke in onStay.AsList())
			methodInvoke.Invoke();
	}

	void OnTriggerExit(Collider other) {
		foreach (MethodInvoke methodInvoke in onExit.AsList())
			methodInvoke.Invoke();
	}

	public void OnDrawGizmos() {
		if (drawGizmos)
			OnDrawGizmosSelected();
	}

	public void OnDrawGizmosSelected() {
		if (!Application.isPlaying)
			Start();
		
		Gizmos.matrix = Matrix4x4.identity;
		Color lineColor = Color.green;
		lineColor.a = 0.25f;
		Gizmos.color = lineColor;
		foreach (MethodInvoke methodInvoke in onEnter.AsList())
			DrawLineToTarget(methodInvoke);

		lineColor = Color.yellow;
		lineColor.a = 0.25f;
		Gizmos.color = lineColor;
		foreach (MethodInvoke methodInvoke in onStay.AsList())
			DrawLineToTarget(methodInvoke);

		lineColor = Color.red;
		lineColor.a = 0.25f;
		Gizmos.color = lineColor;
		foreach (MethodInvoke methodInvoke in onExit.AsList())
			DrawLineToTarget(methodInvoke);

		Gizmos.matrix = transform.localToWorldMatrix;
		Color colliderColor = Color.cyan;
		colliderColor.a = 0.25f;
		Gizmos.color = colliderColor;
		foreach (Collider collider in _colliders)
			DrawCollider(collider);
	}

	private void DrawLineToTarget(MethodInvoke methodInvoke) {
		if (methodInvoke.target != null)
			Gizmos.DrawLine(transform.position, methodInvoke.target.transform.position);
	}

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
}