using UnityEngine;

public class TriggerArea : MonoBehaviour {
	
	public bool drawGizmos = false;

	public ReorderableList_MethodInvoke onEnter = new ReorderableList_MethodInvoke();
	public ReorderableList_MethodInvoke onStay = new ReorderableList_MethodInvoke();
	public ReorderableList_MethodInvoke onExit = new ReorderableList_MethodInvoke();

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