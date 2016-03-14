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
	/// Reference to the debug panel. Configured on the prefab. Should not
	/// be modified on the editor.
	/// </summary>
	public GameObject debugPanel;

	/// <summary>
	/// Reference to the cameras panel. Configured on the prefab. Should not
	/// be modified on the editor.
	/// </summary>
	public GameObject camerasPanel;

	/// <summary>
	/// Reference to the cameras panel main button. Configured on the prefab.
	/// Should not be modified on the editor.
	/// </summary>
	public GameObject camerasPanelButton;

	/// <summary>
	/// Prefab object of a camera's panel. Configured on the prefab. Should not
	/// be modified on the editor.
	/// </summary>
	public GameObject cameraPanelPrefab;

 	/// <summary>
  	/// Reference to the information text component. Configured on the prefab.
  	/// Should not be modified on the editor.
  	/// </summary>
 	public Text generalInformation;

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
	/// Reference to the input component from the scene's game controller.
	/// </summary>
	private GameControllerInput _input;

	/// <summary>
	/// Reference to the camera switcher component from the debug
	/// controller's children.
	/// </summary>
	private DebugCameraSwitcher _cameraSwitcher;

	/// <summary>
	/// Dictionary containing the information text associated to each of
	/// the debug cameras.
	/// </summary>
	private Dictionary<Camera, GameObject> _camerasInformationTexts;

	/// <summary>
	/// Reference to the character currently under control.
	/// </summary>
	private GameObject _currentCharacter;

	/// <summary>
	/// The original color of the currently controlled character.
	/// </summary>
	private Color _savedCharacterColor;

    /// <summary>
     /// Delta time used in fps calculation
     /// </summary>
     float deltaTime = 0.0f;
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
	/// Hides or unhides an UI element from the debug menu.
	/// </summary>
	public void ToggleUIElement(GameObject element) {
		element.SetActive(!element.activeInHierarchy);
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
		// Initializes the lists
		_collidersColors = new Dictionary<Collider, Color>();
		_camerasInformationTexts = new Dictionary<Camera, GameObject>();

		// Looks for the independent controller component
		_independentControl = FindObjectOfType<GameControllerIndependentControl>();

		// Looks for the input component
		_input = FindObjectOfType<GameControllerInput>();

		// Looks for the camera switcher component
		_cameraSwitcher = GetComponentInChildren<DebugCameraSwitcher>();

		// Initializes the current character
		_currentCharacter = null;

		// Adds the cameras to the camera panel
		for (int i = 0; i < _cameraSwitcher.GetCameras().Count; i++) {
			Camera camera = _cameraSwitcher.GetCameras()[i];

			// Creates the panel
			GameObject cameraPanel = Instantiate(cameraPanelPrefab);
			cameraPanel.transform.SetParent(camerasPanel.transform);
			cameraPanel.transform.SetSiblingIndex(i);

			// Sets the event of the button
			Button cameraButton = cameraPanel.transform.GetComponentInChildren<Button>();
			int iVal = i;
			cameraButton.onClick.AddListener(() => { _cameraSwitcher.SetActiveCamera(iVal); });

			// Sets the text of the button
			Text nameText = cameraButton.GetComponentInChildren<Text>();
			nameText.text = camera.name;

			// Stores the information text
			GameObject informationText = cameraPanel.transform.Find("Camera Information Text").gameObject;
			_camerasInformationTexts.Add(camera, informationText);
		}
    }

    //TODO: temporal, esto se borra luego
    public void Update() {
        temporal();
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

		// Reenables the controller's input component
		_input.enabled = true;

		// Checks if the debug mode is active
		debugPanel.SetActive(debugMode);
		camerasPanelButton.SetActive(debugMode);
		//Cursor.visible = debugMode;	Still not necessary
		if (!debugMode) {
			RestoreCharacterColor();
			_cameraSwitcher.SetActiveCamera(0);
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
        ShowGeneralInformation();
        ShowCharacterInformation();
		ShowCharacterState();
		ShowCharacterParameters();

		// Manages the events
		ManageSizeChange();
		ManageCharacterCreation();
		ManageCharacterSelection();
		ManageCharacterMovement();
		ManageCharacterDestruction();
		ManageCameraChange();
		ManageFreeCamera();

		// Shows the character's collisions
		ShowCharacterCollisions();

        
	}

    /// <summary>
    /// TODO: borrar luego de la presentación
    /// </summary>
    private void temporal() {
        if (Input.GetKeyDown(KeyCode.M)) {
           MenuNavigator _menuNavigator = GameObject.FindGameObjectWithTag("Menus")
                                .GetComponent<MenuNavigator>();
            _menuNavigator.ChangeScene("Test Scene 2");
        }else if (Input.GetKeyDown(KeyCode.N)) {
            MenuNavigator _menuNavigator = GameObject.FindGameObjectWithTag("Menus")
                                 .GetComponent<MenuNavigator>();
            _menuNavigator.ChangeScene("Test Scene");
        }
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
    /// Show basic and general information like fps
    /// </summary>
    private void ShowGeneralInformation() {
        StringBuilder sb = new StringBuilder();

        //fps
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        sb.AppendFormat("{0:0.0} ms ({1:0.} fps)", msec, fps);
        

        generalInformation.text = sb.ToString();

    }

    /// <summary>
    /// Updates the GUI text with the basic information of the current
    /// character.
    /// </summary>
    private void ShowCharacterInformation() {
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
		// Sets the text
		stateText.text = _currentCharacter.GetComponent<CharacterControllerCustom>().State.ToString();
	}

	/// <summary>
	/// Updates the GUI text with the parameters information of the current
	/// character.
	/// </summary>
	private void ShowCharacterParameters() {
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
			if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Keypad" + i, true)))
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

		// Finds the position to spawn the character
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane plane = new Plane(Vector3.back, Vector3.zero);
		float distance;
		if (plane.Raycast(ray, out distance))
			newDrop.transform.position = ray.GetPoint(distance);
	}

	/// <summary>
	/// Reads the input and selects a character.
	/// </summary>
	private void ManageCharacterSelection() {
		// Checks if the selection button has been pressed
		if (!Input.GetMouseButtonDown(0))
			return;

		// Casts a ray from the camera to look for the character
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray);
		foreach (RaycastHit hit in hits)
			if (hit.collider.CompareTag("Player")) {
				_independentControl.SetControl(hit.collider.gameObject);
				break;
			}
	}

	/// <summary>
	/// Reads the input and moves the character.
	/// </summary>
	private void ManageCharacterMovement() {
		// Checks if the movement button has been pressed
		if (!Input.GetMouseButton(2))
			return;

		// Finds the position to move the character to
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane plane = new Plane(Vector3.back, Vector3.zero);
		float distance;
		if (plane.Raycast(ray, out distance)) {
			_independentControl.currentCharacter.GetComponent<CharacterControllerCustom>().Stop();
            _independentControl.currentCharacter.transform.position = ray.GetPoint(distance);
		}
	}

	/// <summary>
	/// Reads the input and destroyes the current character.
	/// </summary>
	private void ManageCharacterDestruction() {
		// Checks if the destruction button has been pressed
		if (!Input.GetKeyDown(KeyCode.Delete))
			return;

		// Destroys the character
		DestroyCurrentlyControlledCharacter();
	}

	/// <summary>
	/// Reads the input and changes to the right camera.
	/// </summary>
	private void ManageCameraChange() {
		// Next Camera
		if (Input.GetKeyDown(KeyCode.F2))
			_cameraSwitcher.NextCamera();

		// Previous camera
		if (Input.GetKeyDown(KeyCode.F3))
			_cameraSwitcher.PreviousCamera();

		// Shows the information about the active camera
		foreach (KeyValuePair<Camera, GameObject> entry in _camerasInformationTexts)
			if (entry.Key == _cameraSwitcher.GetActiveCamera()) {
				// Activates the text
				entry.Value.SetActive(true);

				// Creates the string builder
				StringBuilder sb = new StringBuilder();
				Camera cam = _cameraSwitcher.GetActiveCamera();

				// Camera Type
				sb.Append("- Camera Type: ");
				string type;
				if (cam.GetComponent<MainCameraController>() != null)
					type = "Main Camera";
				else if (cam.GetComponent<FreeCameraController>() != null)
					type = "Free Camera";
				else if (cam.GetComponent<FollowPath>() != null)
					type = "Travelling Camera";
				else if (cam.GetComponent<MainCameraController>() != null)
					type = "Main Camera";
				else
					type = "Fixed Camera";
				sb.Append(type);
				sb.Append("\n");

				// Projection Type
				sb.Append("- Projection Type: ");
				sb.Append(cam.orthographic ? "Ortographic" : "Perspective");
				sb.Append("\n");

				// Position
				sb.Append("- Position: ");
				sb.Append(cam.transform.position);
				sb.Append("\n");

				// Rotation
				sb.Append("- Rotation: ");
				sb.Append(cam.transform.eulerAngles);
				sb.Append("\n");

				// Field Of View
				sb.Append("- Field Of View: ");
				sb.Append(cam.fieldOfView);

				// Sets the text
				entry.Value.GetComponent<Text>().text = sb.ToString();
			}
			else
				entry.Value.SetActive(false);
	}

	/// <summary>
	/// Sends the input to the active camera if it's a free camera.
	/// </summary>
	private void ManageFreeCamera() {
		// Looks if the active camera is a free camera
		FreeCameraController freeCamera = Camera.main.GetComponent<FreeCameraController>();
		if (freeCamera == null)
			return;

		// Free cameras are only controlled while holding the LShift key
		if (!Input.GetKey(KeyCode.LeftShift))
			return;

		// Disables game controller's input component
		_input.enabled = false;

		// Creates the input's movement vector
		Vector3 movement = new Vector3();
		movement.x = Input.GetAxis("Horizontal");
		movement.y = 0;
		if (Input.GetKey(KeyCode.Space))
			movement.y++;
		if (Input.GetKey(KeyCode.X))
			movement.y--;
		movement.z = Input.GetAxis("Vertical");

		// Creates the input
		Vector3 rotation = new Vector3();
		rotation.x = Input.GetAxis("Mouse Y");
		rotation.y = Input.GetAxis("Mouse X");
		rotation.z = 0;
		if (Input.GetKey(KeyCode.Q))
			rotation.z++;
		if (Input.GetKey(KeyCode.E))
			rotation.z--;

		// Moves the camera
		freeCamera.Move(movement);
		freeCamera.Rotate(rotation);
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
			if (collider != null && !_collidersColors.ContainsKey(collider)) {
				Renderer renderer = collider.GetComponent<Renderer>();
				if (renderer != null) {
					Material material = renderer.material;
					_collidersColors.Add(collider, material.GetColor("_Color"));
					material.SetColor("_Color", collisionColor);
				}
			}
    }

	#endregion

	#endregion
}