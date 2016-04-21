using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for end Scene, you can configure the displayed message
/// </summary>
public class GameEndController : MonoBehaviour {

    #region Public Attributes


    /// <summary>
    /// Wait time showing message untill scene is changed
    /// </summary>
    public float waitTime = 3F;


    /// <summary>
    /// UI text object reference
    /// </summary>
    public Text levelCompleteText;


    /// <summary>
    /// Displayed phrase
    /// </summary>
    public string phrase = "Level Complete!";
    

    /// <summary>
    /// Next scene to load
    /// </summary>
    public Scene nextScene;

    #endregion

    #region Private Attributes

    /// <summary>
    /// Input controller to disable it when its necessary
    /// </summary>
    private GameControllerInput _gci;

    /// <summary>
    /// Reference to the collider of end region.
    /// </summary>
    private BoxCollider _collider;

    /// <summary>
    /// On trigger enter control
    /// </summary>
    private bool end = false;

    #endregion

    #region Methods


    /// <summary>
    /// Unity's method called when the entity is created.
    /// Recovers the desired componentes of the entity.
    /// </summary>
    public void Awake() {
        // Retrieves the components of the entity.
        _collider = gameObject.GetComponent<BoxCollider>();
    }


    /// <summary>
    /// Unity's method called on start script only one time
    /// </summary>
    void Start(){
        // Set the phrase to the text label and disable it
        levelCompleteText.text = phrase;
        levelCompleteText.enabled = false;

        // In case that the scene is empty, by default we use StartScene
        // TODO improve this part, try to avoid hardcoded strings
        if (nextScene.name == "Not" || nextScene.name == "") {
            Debug.LogWarning("Next Scene not setted, using StartScene by default, please, assign an scene");
            nextScene.name = "StartScene";
        }

        //Get input controller
        _gci = FindObjectOfType<GameControllerInput>();

    }

    /// <summary>
    /// Update the state of the trigger
    /// </summary>
	void Update () {

		// If the scene is ended
	    if (end){

            // Get the navigator
            MenuNavigator _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus)
                                     .GetComponent<MenuNavigator>();
            // Wait before starting the change
            _menuNavigator.ChangeScene(nextScene.name, waitTime);
        }
    }


	/// <summary>
    /// Activated when player enters on the trigger
    /// </summary>
    /// <param name="other">Collider who enters in the trigger</param>
    void OnTriggerEnter(Collider other){

        // Only ends game with player
        if (other.CompareTag(Tags.Player)){

            // Activate counter
            end = true;

            GameObject currentCharacter = FindObjectOfType<GameControllerIndependentControl>().currentCharacter;
            CharacterControllerCustomPlayer cccp = currentCharacter.GetComponent<CharacterControllerCustomPlayer>();

            // Stop player
            cccp.Stop();

            // Disable input
            _gci.StopInput();

            // Show message
            levelCompleteText.enabled = true;
        }
    }



    /// <summary>
    /// Unity's method called by the editor in order to draw the gizmos.
    /// Draws the volume on the editor.
    /// </summary>
    public void OnDrawGizmos() {

        // Calls the configuration functions
        if (!Application.isPlaying) {
            Awake();
            Update();
        }

        // Defines the color of the gizmo
        Color color = Color.red;
        color.a = 0.25f;
        Gizmos.color = color;

        // Draws the end game zone
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 pos = _collider.center;
        Gizmos.DrawCube(pos, _collider.size);
    }
    #endregion
}
