public interface InputProvider {

	float GetAxis(string input);

	float GetAxisRaw(string input);

	bool GetButtonDown(string input);

	bool GetButtonUp(string input);
}
