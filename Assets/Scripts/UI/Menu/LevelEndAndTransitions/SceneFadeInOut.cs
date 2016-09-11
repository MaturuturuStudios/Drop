using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Fade in and out the scenes
/// </summary>
public class SceneFadeInOut : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// the texture that will overlay the screen. This can be a black image or a loading graphic
    /// </summary>
    public GameObject fadeOutTexture;
    
    /// <summary>
    /// Duration while fading
    /// </summary>
    public float fadeDuration = 0.8f;

    /// <summary>
    /// Async operation for load the next scene
    /// </summary>
    [HideInInspector]
    public AsyncOperation op;

    #endregion

    #region Private Attributes
    /// <summary>
    /// the texture's alpha value between 0 and 1
    /// </summary>
    private float _alpha = 1.0f;
    /// <summary>
    /// the direction to fade: in = -1 or out = 1
    /// </summary>
    private bool _isFadeIn = true;
    /// <summary>
    /// Dark background gameobject;
    /// </summary>
    private GameObject _fadeBackround;
    /// <summary>
    /// parent UI object
    /// </summary>
    private GameObject _parentUI;
    /// <summary>
    /// parent UI object
    /// </summary>
    //private LevelEndThanks _levelEndThanks;
    /// <summary>
    /// parent UI object
    /// </summary>
    private LoadingAnim _loadingAnim;
    #endregion


    #region Methods
    #region Public methods

    void Start() {

        // Parent UI object nedded to set it a gui element
        _parentUI = GameObject.FindGameObjectWithTag("Menus");

        // References to animations
        _loadingAnim = FindObjectOfType<LoadingAnim>();

        // Start fade in
        BeginFade(true);
    }

    void Update() {

        // fade out/in the alpha value using the desired time
        _alpha += Time.deltaTime / fadeDuration;

        // force (clamp) the number to be between 0 and 1 because GUI.color uses Alpha values between 0 and 1
        _alpha = Mathf.Clamp01(_alpha);

        // Set local alpha
        float alpha = _alpha;

        // Looks for direction
        if (_isFadeIn)
            alpha = 1 - _alpha;

        // Change tranparency
        if (_fadeBackround)
            _fadeBackround.GetComponent<Image>().color = new Color(_fadeBackround.GetComponent<Image>().color.r, _fadeBackround.GetComponent<Image>().color.g, _fadeBackround.GetComponent<Image>().color.b, alpha);
    }


    /// <summary>
    /// Sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
    /// </summary>
    /// <param name="isFadeIn"> fade: in = true or out = false</param>
    /// <param name="desiredFadeDuration">Duration of the fade effect</param>
    /// <returns>the duration of complete fading</returns>
    public float BeginFade(bool isFadeIn, float desiredFadeDuration = -1) {

        // Sets direction
        _isFadeIn = isFadeIn;

        // Reset alpha
        _alpha = 0;

        // If there isn't any fadeBackround create it
        if (!_fadeBackround)
            _fadeBackround = GameObject.Instantiate(fadeOutTexture);

        // Set object a ui element
        _fadeBackround.transform.SetParent(_parentUI.transform, false);

        // Put in the back of the animations
        _fadeBackround.transform.SetAsLastSibling();

        // Sets the desired duration
        if (desiredFadeDuration != -1)
            fadeDuration = desiredFadeDuration;

        return (desiredFadeDuration);
    }


    /// <summary>
    /// Change to the next scene with a fading. This is the method that should be called to validate the values
    /// </summary>
    /// <param name="nameScene">The name of the next scene</param>
    /// <param name="maxTime">Delay should wait before start the fade to the next scene</param>
    public void ChangeScene(string nameScene, float maxTime) {

        StartCoroutine(NextScene(nameScene, maxTime));
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Change the scenes
    /// Wait few seconds before starting the fade between actual scene and next scene
    /// </summary>
    /// <param name="nameScene">Next scene</param>
    /// <param name="maxTime">Wait before starting fading</param>
    /// <returns></returns>
    private IEnumerator NextScene(string nameScene, float maxTime) {
        
        // Preload next scene
        op = SceneManager.LoadSceneAsync(nameScene);
        
        // Don't load scene untill time has expired
        op.allowSceneActivation = false;

        // Show loading icon
        StartCoroutine(_loadingAnim.PlayLoadingAnim(op));
        
        // Wait max time to skip
        yield return MenuNavigator.WaitForRealSeconds(maxTime);

        // Start fade animation
        BeginFade(false, fadeDuration);

        // Wait for the fade duration
        yield return MenuNavigator.WaitForRealSeconds(fadeDuration);
        

        // When scene is loaded and time has expired, we proceed to load next scene
        op.allowSceneActivation = true;
    }
    #endregion
    #endregion
}
