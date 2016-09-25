using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HelpControlSwitchUI : MonoBehaviour {

	public Sprite keyboardImage;
	public Sprite controllerImage;

	private Image _renderer;

	void Start() {
		_renderer = GetComponent<Image>();
	}

	void Update() {
		if (Input.GetJoystickNames().Length == 0)
			_renderer.sprite = keyboardImage;
		else
			_renderer.sprite = controllerImage;
	}
}