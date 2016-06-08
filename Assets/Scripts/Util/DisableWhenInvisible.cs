using UnityEngine;

/// <summary>
/// Disables a set of components whenever the entity is not
/// visible by the camera. Reenables if it becomes visible
/// again.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class DisableWhenInvisible : MonoBehaviour {

	/// <summary>
	/// Set of components to disable.
	/// </summary>
	public MonoBehaviour[] components;

	void OnBecameVisible() {
		foreach (MonoBehaviour component in components)
			component.enabled = true;
	}

	void OnBecameInvisible() {
		foreach (MonoBehaviour component in components)
			component.enabled = false;
	}
}
