using UnityEngine;

public class InheritParentVelocity : MonoBehaviour {

	public float forceFactor = 1;

	private Transform _transform;

	private Transform _parent;

	private Rigidbody _rigidbody;

	private Vector3 _lastParentPosition;

	void Awake() {
		_transform = transform;
		_parent = _transform.parent;
		_rigidbody = GetComponent<Rigidbody>();
		_lastParentPosition = _parent.position;
	}

	void Update() {
		Vector3 displacement = _parent.position - _lastParentPosition;
		_rigidbody.AddForce(- forceFactor * displacement / Time.deltaTime);
		_lastParentPosition = _parent.position;
	}
}
