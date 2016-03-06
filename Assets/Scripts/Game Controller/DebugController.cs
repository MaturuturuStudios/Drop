using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Class that controls the debug mode. It will read the desired input
/// commands and show all the important information.
/// </summary>
public class DebugController : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// If the debug mode is activated or not. If enabled on the editor,
	/// the debug mode will start activated.
	/// </summary>
	public bool debugMode = false;

	/// <summary>
	/// If the important information about the character should be visible
	/// while on debug mode. If enabled on the editor this information 
	/// will start visible.
	/// </summary>
	public bool showInformation = false;

	/// <summary>
	/// If the character's state information should be visible while on
	/// debug mode. If enabled on the editor this information will start
	/// visible.
	/// </summary>
	public bool showState = false;


	/// <summary>
	/// If the character's parameters information should be visible while on
	/// debug mode. If enabled on the editor this information will start
	/// visible.
	/// </summary>
	public bool showParameters = false;

	/// <summary>
	/// Reference to the debug panel. Configured on the prefab. Should not
	/// be modified on the editor.
	/// </summary>
	public GameObject debugPanel;

	/// <summary>
	/// Reference to the information text component. Configured on the prefab.
	/// Should not be modified on the editor.
	/// </summary>
	public Text informationText;

	/// <summary>
	/// Reference to the state text component. Configured on the prefab.
	/// Should not be modified on the editor.
	/// </summary>
	public Text stateText;

	/// <summary>
	/// Reference to the parameters text component. Configured on the prefab.
	/// Should not be modified on the editor.
	/// </summary>
	public Text parametersText;

	/// <summary>
	/// The color to tint the character currently under control.
	/// </summary>
	public Color characterColor = Color.blue;

	/// <summary>
	/// The color to tint the colliders the character collides with.
	/// </summary>
	public Color collisionColor = Color.red;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Dictionary containing the original color of each collider tint by
	/// the debug mode. Use when restoring the color.
	/// </summary>
	private Dictionary<Collider, Color> _collidersColors;

	/// <summary>
	/// Reference to the independent control component from the scene's
	/// game controller.
	/// </summary>
	private GameControllerIndependentControl _independentControl;

	/// <summary>
	/// Reference to the character currently under control.
	/// </summary>
	private GameObject _currentCharacter;

	/// <summary>
	/// The original color of the currently controlled character.
	/// </summary>
	private Color _savedCharacterColor;

	#endregion

	#region Methods

	#region Public GUI Methods

	/// <summary>
	/// Toggles the debug mode, which displays important information and
	/// allows some extra operations.
	/// </summary>
	public void ToggleDebugMode() {
		debugMode = !debugMode;
	}

	/// <summary>
	/// Hides or unhides the current character's basic information.
	/// </summary>
	public void ToggleShowInformation() {
		showInformation = !showInformation;
	}

	/// <summary>
	/// Hides or unhides the current character's state information.
	/// </summary>
	public void ToggleShowState() {
		showState = !showState;
	}

	/// <summary>
	/// Hides or unhides the current character's parameters information.
	/// </summary>
	public void ToggleShowParameters() {
		showParameters = !showParameters;
	}

	/// <summary>
	/// Destroy the current controlled character.
	/// </summary>
	public void DestroyCurrentlyControlledCharacter() {
		// Destroys the currently controlled character
		_independentControl.DestroyDrop(_independentControl.currentCharacter, true);
	}

	#endregion

	/// <summary>
	/// Unity's method called at the beginning of the first frame this
	/// script is active.
	/// Initializes the script.
	/// </summary>
	void Start() {
		// Initializes the collisions list
		_collidersColors = new Dictionary<Collider, Color>();

		// Looks for the independent controller component
		_independentControl = FindObjectOfType<GameControllerIndependentControl>();

		// Initializes the current character
		_currentCharacter = null;
    }
	
	/// <summary>
	/// Unity's method called at the end of each frame if this script is
	/// active.
	/// Handles the input and shows the necessary information.
	/// </summary>
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
		if (!debugMode) {
			RestoreCharacterColor();
            return;
		}

		// Checks if the current character has changed and retrieves it
		if (_currentCharacter != _independentControl.currentCharacter) {
			// Restores last current character's color
			RestoreCharacterColor();

			// Updates the current character and changes it's color
			_currentCharacter = _independentControl.currentCharacter;
			Material characterMaterial = _currentCharacter.GetComponentInChildren<Renderer>().material;
			_savedCharacterColor = characterMaterial.GetColor("_Color");
			characterMaterial.SetColor("_Color", characterColor);
		}

		// Updates the texts
		ShowCharacterInformation();
		ShowCharacterState();
		ShowCharacterParameters();

		// Manages the events
		ManageSizeChange();
		ManageCharacterCreation();
		ManageCharacterSelection();
		ShowCharacterCollisions();
	}

	/// <summary>
	/// Restores current character's color
	/// </summary>
	private void RestoreCharacterColor() {
		if (_currentCharacter != null)
			_currentCharacter.GetComponentInChildren<Renderer>().material.SetColor("_Color", _savedCharacterColor);
		_currentCharacter = null;
    }

	#region Show Information Methods

	/// <summary>
	/// Updates the GUI text with the basic information of the current
	/// character.
	/// </summary>
	private void ShowCharacterInformation() {
		// Displays the text
		informationText.gameObject.SetActive(showInformation);
        if (!showInformation)
			return;

		// Creates the string builder
		StringBuilder sb = new StringBuilder();

		// Name
		sb.Append("- Name: ");
		sb.Append(_currentCharacter.name);
		sb.Append("\n");

		// Size
		sb.Append("- Size: ");
		sb.Append(_currentCharacter.GetComponent<CharacterSize>().GetSize());
		sb.Append("\n");

		// Velocity
		sb.Append("- Velocity: ");
		sb.Append(_currentCharacter.GetComponent<CharacterControllerCustom>().Velocity);
		sb.Append("\n");

		// Mass
		sb.Append("- Mass: ");
		sb.Append(_currentCharacter.GetComponent<CharacterControllerCustom>().GetTotalMass());
		sb.Append("\n");

		// Gravity
		sb.Append("- Gravity: ");
		sb.Append(_currentCharacter.GetComponent<CharacterControllerCustom>().Parameters.Gravity);
		sb.Append("\n");

		// Shooting state
		sb.Append("- Shooting?: ");
		sb.Append(_currentCharacter.GetComponent<CharacterShoot>().isShooting());

		// Sets the text
		informationText.text = sb.ToString();
    }

	/// <summary>
	/// Updates the GUI text with the state information of the current
	/// character.
	/// </summary>
	private void ShowCharacterState() {
		// Displays the text
		stateText.gameObject.SetActive(showState);
		if (!showState)
			return;

		// Sets the text
		stateText.text = _currentCharacter.GetComponent<CharacterControllerCustom>().State.ToString();
	}

	/// <summary>
	/// Updates the GUI text with the parameters information of the current
	/// character.
	/// </summary>
	private void ShowCharacterParameters() {
		// Displays the text
		parametersText.gameObject.SetActive(showParameters);
		if (!showParameters)
			return;

		// Sets the text
		parametersText.text = _currentCharacter.GetComponent<CharacterControllerCustom>().Parameters.ToString();
	}

	#endregion

	#region Handle Events Methods

	/// <summary>
	/// Reads the input and resizes the current character.
	/// </summary>
	/// <param name="currentCharacter">The current character</param>
	private void ManageSizeChange() {
		// Retrieves the character's size component
		CharacterSize sizeComponent = _currentCharacter.GetComponent<CharacterSize>();

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

	/// <summary>
	/// Reads the input and creates new characters.
	/// </summary>
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

	/// <summary>
	/// Reads the input and selects a character.
	/// </summary>
	private void ManageCharacterSelection() {
		// Checks if the creation button has been pressed
		if (!Input.GetMouseButtonDown(0))
			return;

		// Finds out the position to select the character
		Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		position.z = 0;

		// Finds the first player's character in that position
		Collider[] collidersOnPosition = Physics.OverlapSphere(position, 0);
		foreach (Collider collider in collidersOnPosition)
			if (collider.CompareTag("Player")) {
				_independentControl.SetControl(collider.gameObject);
				break;
			}
	}

	/// <summary>
	/// Changes the color of every game object the current character
	/// has collided with this frame.
	/// </summary>
	/// <param name="currentCharacter">The current character</param>
	private void ShowCharacterCollisions() {
		// Retrieves the character's controller component
		CharacterControllerCustom controllerComponent = _currentCharacter.GetComponent<CharacterControllerCustom>();

		// Changes the material of the collisions game object
		foreach (Collider collider in controllerComponent.Collisions)
			if (!_collidersColors.ContainsKey(collider)) {
				Material material = collider.GetComponent<Renderer>().material;
				_collidersColors.Add(collider, material.GetColor("_Color"));
				material.SetColor("_Color", collisionColor);
			}
    }

	#endregion

	#endregion
}