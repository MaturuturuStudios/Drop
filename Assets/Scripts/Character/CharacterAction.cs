using UnityEngine;

/// <summary>
/// Character's component controlling the use of the 
/// action button. Checks for interactive objects
/// and notifies them.
/// </summary>
public class CharacterAction : MonoBehaviour {

	#region Private Attributes

	/// <summary>
	/// Reference to the Unity's CharacterController component
	/// attached to this entity.
	/// </summary>
	private CharacterController _controller;

	/// <summary>
	/// Reference to this entity's Transform component.
	/// </summary>
	private Transform _transform;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after this object is
	/// created, regardless if it's enabled or not.
	/// </summary>
	void Awake() {
		_controller = GetComponent<CharacterController>();
		_transform = transform;
	}

	/// <summary>
	/// Checks for interactive objects and notifies them to
	/// perform their actions.
	/// </summary>
	public void DoAction() {
		// Checks for colliders overlaping it's volume
		Vector3 center = _transform.TransformPoint(_controller.center);
		float radius = _transform.lossyScale.x * _controller.radius;
		Collider[] overlapingColliders = Physics.OverlapSphere(center, radius);

		// Checks if the colliders area interactive and notifies them
		foreach (Collider collider in overlapingColliders) {
			ActionPerformer actionPerformer = collider.GetComponent<ActionPerformer>();
			if (actionPerformer != null)
				actionPerformer.DoAction(gameObject);
		}
	}

	#endregion
}
