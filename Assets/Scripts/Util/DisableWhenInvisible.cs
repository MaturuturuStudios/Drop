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

	/// <summary>
	/// Reference to this entity's Renderer component.
	/// </summary>
	private Renderer _renderer;

	void Awake() {
		// Retrieves the desired components
		_renderer = GetComponent<Renderer>();
	}

	void OnEnable() {
		// Checks if the components should be enabled or not
		bool visible = _renderer.isVisible;
		foreach (MonoBehaviour component in components)
			component.enabled = visible;
	}

	void OnBecameVisible() {
		foreach (MonoBehaviour component in components)
			component.enabled = true;
	}

	void OnBecameInvisible() {
		foreach (MonoBehaviour component in components)
			component.enabled = false;
	}
}
