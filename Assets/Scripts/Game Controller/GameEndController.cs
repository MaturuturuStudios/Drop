using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script for end Scene, you can configure the displayed message
/// </summary>
public class GameEndController : MonoBehaviour {

    #region Attributes
    /// <summary>
    /// End Game configuration options
    /// </summary>
    public EndGame endGame;
    [System.Serializable]
	//  Movement Class
    public class EndGame {
        //wait time showing message untill scene is changed
        public float waitTime = 3F;
        //UI text object reference
        public Text levelCompleteText;
        //Displayed phrase
        public string phrase = "Level Complete!";
    }
    
    /// <summary>
    ///On trigger enter control
    /// </summary>
    private bool end = false;

    /// <summary>
    /// Reference to the collider where the wind force will be applied.
    /// </summary>
    private BoxCollider _collider;
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
        //Reset Text
        endGame.levelCompleteText.text = endGame.phrase;
        endGame.levelCompleteText.enabled = false;

    }

    /// <summary>
    /// Update the state of the trigger
    /// </summary>
	void Update () {
		//if the scene is ended
	    if (end){
            //get the navigator
            MenuNavigator _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus)
                                     .GetComponent<MenuNavigator>();
            //wait before starting the change
            _menuNavigator.ChangeScene("StartScene", endGame.waitTime);
        }
    }

	/// <summary>
    /// Activated when player enters on the trigger
    /// </summary>
    /// <param name="other">Collider who enters in the trigger</param>
    void OnTriggerEnter(Collider other){
        //only ends game with player
        if (other.CompareTag(Tags.Player)){
            // Activate counter
            end = true;

            GameObject currentCharacter = FindObjectOfType<GameControllerIndependentControl>().currentCharacter;
            CharacterControllerCustomPlayer cccp = currentCharacter.GetComponent<CharacterControllerCustomPlayer>();
            //Stop player
            cccp.Stop(true);
            cccp.enabled = false;
            //currentCharacter.SetActive(false);

            //Show message
            endGame.levelCompleteText.enabled = true;
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

        // Draws the cube
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 pos = _collider.transform.position - transform.position;
        Gizmos.DrawCube(pos, _collider.size);
    }
    #endregion
}
