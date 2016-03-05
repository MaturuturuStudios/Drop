using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

public class DebugController : MonoBehaviour {

	public bool debugMode = false;

	public bool showInformation = false;
	public bool showState = false;
	public bool showParameters = false;
	
	public GameControllerIndependentControl _independentControl;

	public GameObject debugPanel;

	public Text informationText;
	public Text stateText;
	public Text parametersText;

	public Color collisionColor;

	private Dictionary<Collider, Color> _collidersColors;

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

	void Start() {
		// Initializes the collisions list
		_collidersColors = new Dictionary<Collider, Color>();
    }
	
	void LateUpdate () {
		// Toggles debug mode
		if (Input.GetKeyDown(KeyCode.F1))
			ToggleDebugMode();

		// Restores the collider's color to their original ones
		foreach (KeyValuePair<Collider, Color> entry in _collidersColors)
			entry.Key.GetComponent<Renderer>().material.SetColor("_Color", entry.Value);
		_collidersColors.Clear();

		// Checks if the debug mod is active
		debugPanel.SetActive(debugMode);
		//Cursor.visible = debugMode;	Still not necessary
		if (!debugMode)
			return;

		// Retrieves the current character
		GameObject currentCharacter = _independentControl.currentCharacter;

		// Updates the texts
		ShowCharacterInformation(currentCharacter);
		ShowCharacterState(currentCharacter);
		ShowCharacterParameters(currentCharacter);

		// Manages the debug input
		ManageSizeChange(currentCharacter);
		ManageCharacterCreation();

		// Shows the collisions of the character
		ShowCharacterCollisions(currentCharacter);
    }

	private void ShowCharacterInformation(GameObject currentCharacter) {
		// Displays the text
		informationText.gameObject.SetActive(showInformation);
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
		informationText.text = sb.ToString();
    }

	private void ShowCharacterState(GameObject currentCharacter) {
		// Displays the text
		stateText.gameObject.SetActive(showState);
		if (!showState)
			return;

		// Sets the text
		stateText.text = currentCharacter.GetComponent<CharacterControllerCustom>().State.ToString();
	}

	private void ShowCharacterParameters(GameObject currentCharacter) {
		// Displays the text
		parametersText.gameObject.SetActive(showParameters);
		if (!showParameters)
			return;

		// Sets the text
		parametersText.text = currentCharacter.GetComponent<CharacterControllerCustom>().Parameters.ToString();
	}

	private void ManageSizeChange(GameObject currentCharacter) {
		// Retrieves the character's size component
		CharacterSize sizeComponent = currentCharacter.GetComponent<CharacterSize>();

		// Handles the incremental size input
		if (Input.GetKeyDown(KeyCode.KeypadPlus))
			sizeComponent.IncrementSize();
		if (Input.GetKeyDown(KeyCode.KeypadMinus))
			sizeComponent.DecrementSize();

		// Handles the direct size input
		for (int i = 1; i < 10; i++)
			if (Input.GetKeyDown(i.ToString()))
				sizeComponent.SetSize(i);
	}

	private void ManageCharacterCreation() {
		// Checks if the creation button has been pressed
		if (!Input.GetMouseButtonDown(1))
			return;

		// Spawns the character
		GameObject newDrop = _independentControl.CreateDrop(true);

		// Finds out the position to spawn the new character
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		position.z = 0;
		newDrop.transform.position = position;
	}

	public void DestroyCurrentlyControlledCharacter() {
		// Destroys the currently controlled character
		_independentControl.KillDrop(_independentControl.currentCharacter);
	}

	private void ShowCharacterCollisions(GameObject currentCharacter) {
		// Retrieves the character's controller component
		CharacterControllerCustom controllerComponent = currentCharacter.GetComponent<CharacterControllerCustom>();

		// Changes the material of the collisions game object
		foreach (Collider collider in controllerComponent.Collisions)
			if (!_collidersColors.ContainsKey(collider)) {
				Material material = collider.GetComponent<Renderer>().material;
				_collidersColors.Add(collider, material.GetColor("_Color"));
				material.SetColor("_Color", collisionColor);
			}
    }
}