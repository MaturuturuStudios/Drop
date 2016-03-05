using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class DebugController : MonoBehaviour {

	public bool debugMode = false;

	public bool showInformation = false;
	public bool showState = false;
	public bool showParameters = false;
	
	public GameControllerIndependentControl _independentControl;

	public GameObject _debugPanel;

	public Text _informationText;
	public Text _stateText;
	public Text _parametersText;

	public void ToggleDebugMode() {
		debugMode = !debugMode;
	}

	public void ToggleShowInformation() {
		showInformation = !showInformation;
	}

	public void ToggleShowState() {
		showState = !showState;
	}

	public void ToggleShowParameters() {
		showParameters = !showParameters;
	}
	
	void LateUpdate () {
		if (Input.GetKeyDown(KeyCode.F1))
			ToggleDebugMode();

		_debugPanel.SetActive(debugMode);
		//Cursor.visible = debugMode;	Still not necessary

		if (!debugMode)
			return;

		GameObject currentCharacter = _independentControl.currentCharacter;

		ShowCharacterInformation(currentCharacter);
		ShowCharacterState(currentCharacter);
		ShowCharacterParameters(currentCharacter);

		ManageSizeChange(currentCharacter);
		ManageCharacterCreation();
		ManageCharacterDestruction(currentCharacter);

		ShowCharacterCollisions(currentCharacter);
    }

	private void ShowCharacterInformation(GameObject currentCharacter) {
		// Displays the text
		_informationText.gameObject.SetActive(showInformation);
        if (!showInformation)
			return;

		// Creates the string builder
		StringBuilder sb = new StringBuilder();

		// Name
		sb.Append("- Name: ");
		sb.Append(currentCharacter.name);
		sb.Append("\n");

		// Size
		sb.Append("- Size: ");
		sb.Append(currentCharacter.GetComponent<CharacterSize>().GetSize());
		sb.Append("\n");

		// Velocity
		sb.Append("- Velocity: ");
		sb.Append(currentCharacter.GetComponent<CharacterControllerCustom>().Velocity);
		sb.Append("\n");

		// Mass
		sb.Append("- Mass: ");
		sb.Append(currentCharacter.GetComponent<CharacterControllerCustom>().GetTotalMass());
		sb.Append("\n");

		// Gravity
		sb.Append("- Gravity: ");
		sb.Append(currentCharacter.GetComponent<CharacterControllerCustom>().Parameters.Gravity);
		sb.Append("\n");

		// Shooting state
		sb.Append("- Shooting?: ");
		sb.Append(currentCharacter.GetComponent<CharacterShoot>().isShooting());

		// Sets the text
		_informationText.text = sb.ToString();
    }

	private void ShowCharacterState(GameObject currentCharacter) {
		// Displays the text
		_stateText.gameObject.SetActive(showState);
		if (!showState)
			return;

		// Sets the text
		_stateText.text = currentCharacter.GetComponent<CharacterControllerCustom>().State.ToString();
	}

	private void ShowCharacterParameters(GameObject currentCharacter) {
		// Displays the text
		_parametersText.gameObject.SetActive(showParameters);
		if (!showParameters)
			return;

		// Sets the text
		_parametersText.text = currentCharacter.GetComponent<CharacterControllerCustom>().Parameters.ToString();
	}

	private void ManageSizeChange(GameObject currentCharacter) {
		// TODO
	}

	private void ManageCharacterCreation() {
		// TODO
	}

	private void ManageCharacterDestruction(GameObject currentCharacter) {
		// TODO
	}

	private void ShowCharacterCollisions(GameObject currentCharacter) {
		// TODO
	}
}