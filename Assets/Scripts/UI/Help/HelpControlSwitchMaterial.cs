using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HelpControlSwitchMaterial : MonoBehaviour {

	public Material keyboardMaterial;
	public Material controllerMaterial;

	private Renderer _renderer;

	void Start() {
		_renderer = GetComponent<Renderer>();
	}

	void Update() {
		if (Input.GetJoystickNames().Length == 0)
			_renderer.material = keyboardMaterial;
		else
			_renderer.material = controllerMaterial;
	}
}