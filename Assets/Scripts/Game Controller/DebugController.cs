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
		_independentControl.DestroyDrop(_independentControl.currentCharacter);
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

		// Looks for the indpendent controller component
		_independentControl = FindObjectOfType<GameControllerIndependentControl>();
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
		if (!debugMode)
			return;

		// Retrieves the current character
		GameObject currentCharacter = _independentControl.currentCharacter;

		// Updates the texts
		ShowCharacterInformation(currentCharacter);
		ShowCharacterState(currentCharacter);
		ShowCharacterParameters(currentCharacter);

		// Manages the events
		ManageSizeChange(currentCharacter);
		ManageCharacterCreation();
		ShowCharacterCollisions(currentCharacter);
	}

	#region Show Information Methods

	/// <summary>
	/// Updates the GUI text with the basic information of the current
	/// character.
	/// </summary>
	/// <param name="currentCharacter">The current character</param>
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

	/// <summary>
	/// Updates the GUI text with the state information of the current
	/// character.
	/// </summary>
	/// <param name="currentCharacter">The current character</param>
	private void ShowCharacterState(GameObject currentCharacter) {
		// Displays the text
		stateText.gameObject.SetActive(showState);
		if (!showState)
			return;

		// Sets the text
		stateText.text = currentCharacter.GetComponent<CharacterControllerCustom>().State.ToString();
	}

	/// <summary>
	/// Updates the GUI text with the parameters information of the current
	/// character.
	/// </summary>
	/// <param name="currentCharacter">The current character</param>
	private void ShowCharacterParameters(GameObject currentCharacter) {
		// Displays the text
		parametersText.gameObject.SetActive(showParameters);
		if (!showParameters)
			return;

		// Sets the text
		parametersText.text = currentCharacter.GetComponent<CharacterControllerCustom>().Parameters.ToString();
	}

	#endregion

	#region Handle Events Methods

	/// <summary>
	/// Reads the input and resizes the current character.
	/// </summary>
	/// <param name="currentCharacter">The current character</param>
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
	/// Changes the color of every game object the current character
	/// has collided with this frame.
	/// </summary>
	/// <param name="currentCharacter">The current character</param>
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

	#endregion

	#endregion
}