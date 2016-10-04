using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HelpControlSwitchSprite : MonoBehaviour {

	public Sprite keyboardSprite;
	public Sprite controllerSprite;

	private SpriteRenderer _renderer;

	void Start() {
		_renderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		if (Input.GetJoystickNames().Length == 0)
			_renderer.sprite = keyboardSprite;
		else
			_renderer.sprite = controllerSprite;
	}
}