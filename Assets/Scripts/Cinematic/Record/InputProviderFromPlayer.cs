using UnityEngine;

public class InputProviderFromPlayer : InputProvider {

	public float GetAxis(string input) {
		return Input.GetAxis(input);
	}

	public float GetAxisRaw(string input) {
		return Input.GetAxisRaw(input);
	}

	public bool GetButtonDown(string input) {
		return Input.GetButtonDown(input);
	}

	public bool GetButtonUp(string input) {
		return Input.GetButtonUp(input);
	}
}
