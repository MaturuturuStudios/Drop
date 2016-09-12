using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for end Scene, you can configure the displayed message
/// </summary>
public class LevelEndController : MonoBehaviour {

    #region Public Attributes

    /// <summary>
    /// Wait time to start fading 
    /// </summary>
    public float timeToSkip = 3F;

    /// <summary>
    /// Max time waiting before fade
    /// </summary>
    public float maxTime = 5F;

    #endregion

    #region Private Attributes

    /// <summary>
    /// Input controller to disable it when its necessary
    /// </summary>
    private GameControllerInput _gci;


    /// <summary>
    /// Reference to independent controller
    /// </summary>
    private GameControllerIndependentControl _gcic;


    /// <summary>
    /// Reference to the collider of end region.
    /// </summary>
    private BoxCollider _collider;


    /// <summary>
    /// UI Reference
    /// </summary>
    private MenuNavigator _menuNavigator;


    /// <summary>
    /// Reference to level transition
    /// </summary>
    private LevelTransition _levelTransition;


    /// <summary>
    /// Reference to level transition controller
    /// </summary>
    private LevelTransitionController _levelTransitionController;

    /// <summary>
    /// The next scene to load
    /// </summary>
    private Scene nextScene;
    /// <summary>
    /// The next level to load
    /// </summary>
    private LevelInfo nextLevel;
    /// <summary>
    /// the data to ask for unlock the level
    /// </summary>
    private GameControllerData data;
    #endregion

    #region Methods

    /// <summary>
    /// Unity's method called when the entity is created.
    /// Recovers the desired componentes of the entity.
    /// </summary>
    public void Awake() {
        // Retrieves the components of the entity.
        _collider = gameObject.GetComponent<BoxCollider>();

        // Retrieves the UI Reference
        _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus).GetComponent<MenuNavigator>();

        //Get data game and store the next level
        if(Application.isPlaying)
            data = GameObject.FindGameObjectWithTag(Tags.GameData).GetComponent<GameControllerData>();
       
    }


    /// <summary>
    /// Unity's method called on start script only one time
    /// </summary>
    void Start(){
        if (data == null) {
            nextScene = new Scene();
            Debug.LogWarning("Next Scene not setted, using StartScene by default, please, assign an scene");
            nextScene.name = "StartScene";

        } else {
            string nameScene = SceneManager.GetActiveScene().name;
            nextScene = data.GetNextScene(nameScene, out nextLevel);

            if (nextScene==null) {
                nextScene = data.GetDefaultScene();
            }
        }

        //Get input controller
        _gci = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerInput>();
        //Get input controller
        _gcic = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();
        //Get input controller
        _levelTransition = GetComponent<LevelTransition>();
        _levelTransitionController = GetComponent<LevelTransitionController>();
    }


	/// <summary>
    /// Activated when player enters on the trigger
    /// </summary>
    public void LevelEnd(){
        // Stops input
        _gci.StopInput(true, timeToSkip);

        //make sure is stored the first level is completed
        PlayerPrefs.SetInt(PlayerDataStoreKeys.PlayerFirstTime, 1);
        PlayerPrefs.Save();

        //unlock the level
        data.UnlockLevel(nextLevel);

        // Load scene async
        _menuNavigator.ChangeScene(nextScene.name, maxTime);

        // Start transition animation
        if (_levelTransitionController != null) {
            int size = _gcic.currentCharacter.GetComponent<CharacterSize>().GetSize();
            _levelTransitionController.BeginLevelTransition(size);
            StartCoroutine(_levelTransitionController.WaitMinTimeToSkip(timeToSkip));
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
