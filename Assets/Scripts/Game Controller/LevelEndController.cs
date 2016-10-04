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
    //private LevelTransition _levelTransition;


    /// <summary>
    /// Reference to level transition controller
    /// </summary>
    private LevelTransitionController _levelTransitionController;

    /// <summary>
    /// The next scene to load
    /// </summary>
	private Scene _nextScene;
    /// <summary>
    /// The next level to load
    /// </summary>
	private LevelInfo _nextLevel;
	/// <summary>
	/// The actual level.
	/// </summary>
	private LevelInfo _actualLevel;
    /// <summary>
    /// the data to ask for unlock the level
    /// </summary>
	private GameControllerData _data;
    /// <summary>
    /// Handles if we have entered the trigger before
    /// </summary>
	private bool _finished = false;
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
            _data = GameObject.FindGameObjectWithTag(Tags.GameData).GetComponent<GameControllerData>();
       
    }


    void Start() {

        // Get reference to input controller
        _gci = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerInput>();
        // Get reference to independent control
        _gcic = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameControllerIndependentControl>();
        // Get reference to level transition controller
        _levelTransitionController = GetComponent<LevelTransitionController>();

        if (_data == null) {
            _nextScene = new Scene();
            Debug.LogWarning("Next Scene not setted, using Menu by default, please, assign an scene");
            _nextScene.name = "Menu";


        } else {
            string nameScene = SceneManager.GetActiveScene().name;
            _nextScene = _data.GetNextScene(nameScene, out _nextLevel);
			_actualLevel = _data.GetInfoScene(nameScene);

            if (_nextScene==null) {
                _nextScene = _data.GetDefaultScene();
                _nextScene.name = "Thanks";
            }
        }
    }


	/// <summary>
    /// Activated when player enters on the trigger
    /// </summary>
    public void LevelEnd(){

        if (!_finished) {
            _finished = true;

            // Stops input
            _gci.StopInput(true, timeToSkip);

            // Disable start button
            _menuNavigator.SetPauseAvailable(false);

            //make sure is stored the first level is completed
            PlayerPrefs.SetInt(PlayerDataStoreKeys.PlayerFirstTime, 1);
            PlayerPrefs.Save();

            //store the level completed and its score
            int size = _gcic.currentCharacter.GetComponent<CharacterSize>().GetSize();
            _data.SetLevelScore(_actualLevel, size);

            //unlock the level
            _data.UnlockLevel(_nextLevel);
            // Load scene async
            _menuNavigator.ChangeScene(_nextScene.name, maxTime);

            // Start transition animation
            if (_levelTransitionController != null) {
                //get the max score of the level
                ScoreLevel score = _data.GetScoreLevel(_actualLevel);
                _levelTransitionController.BeginLevelTransition(score.max);
                StartCoroutine(_levelTransitionController.WaitMinTimeToSkip(timeToSkip));
            }
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
