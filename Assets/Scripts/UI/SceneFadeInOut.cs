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
    /// Wait this seconds before starting with the fading
    /// </summary>
    public float delayStartChangeSeconds = 1.0f;
    /// <summary>
    /// Wait this seconds before starting with the fading
    /// </summary>
    public float delayEndChangeSeconds = 0f;
    /// <summary>
    /// the texture that will overlay the screen. This can be a black image or a loading graphic
    /// </summary>
    public GameObject fadeOutTexture;
    /// <summary>
    /// Duration while fading
    /// </summary>
    public float fadeDuration = 0.8f;
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
    private LevelEndAnim _levelEndAnim;
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
        _levelEndAnim = FindObjectOfType<LevelEndAnim>();
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
        _fadeBackround.transform.SetAsFirstSibling();

        // Sets the desired duration
        if (desiredFadeDuration != -1)
            fadeDuration = desiredFadeDuration;

        return (desiredFadeDuration);
    }


    /// <summary>
    /// Change to the next scene with a fading. This is the method that should be called to validate the values
    /// </summary>
    /// <param name="nameScene">The name of the next scene</param>
    /// <param name="delayStart">Delay should wait before starting. By default -1 that means the public attribute delayStartChangeSeconds
    /// on this script will be used</param>
    /// <param name="fadeTime">Delay time should elapse the fade effect</param>
    /// <param name="delayEnd">Delay should wait after ending. By default -1 that means the public attribute delaySEndChangeSeconds
    /// on this script will be used</param>
    /// <param name="showUI">Set true to show ui elements like in level end, set false for menus</param>
    public void ChangeScene(string nameScene, float delayStart = -1, float fadeTime = -1, float delayEnd = -1, bool showUI = false) {
        if (delayStart <= -1) {
            delayStart = delayStartChangeSeconds;
        }

        if (delayEnd <= -1) {
            delayEnd = delayEndChangeSeconds;
        }

        StartCoroutine(NextScene(nameScene, delayStart, fadeTime, delayEnd, showUI));
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Change the scenes
    /// Wait few seconds before starting the fade between actual scene and next scene
    /// </summary>
    /// <param name="nameScene">Next scene</param>
    /// <param name="delayStart">Wait before starting</param>
    /// <param name="desiredFadeDuration">Elapsed fade duration/param>
    /// <param name="delayEnd">Wait after ending</param>
    /// <returns></returns>
    private IEnumerator NextScene(string nameScene, float delayStart, float desiredFadeDuration, float delayEnd, bool showUI) {

        // Start loading animation
        //_levelEndAnim.StartCoroutine(LevelLoading(totalDrops, wastedDrops, startDelay, delayBetweenDrops));
        // Preload next scene
        AsyncOperation op = SceneManager.LoadSceneAsync(nameScene);
        
        // Don't load scene untill time has expired
        op.allowSceneActivation = false;

        yield return MenuNavigator.WaitForRealSeconds(delayStart);

        // Start fade animation
        BeginFade(false, desiredFadeDuration);
        if (showUI) {
            StartCoroutine(_levelEndAnim.EndMessage(fadeDuration));
        }
        StartCoroutine(_loadingAnim.PlayLoadingAnim(op));
        // Wait for the fade duration
        yield return MenuNavigator.WaitForRealSeconds(fadeDuration);

        yield return MenuNavigator.WaitForRealSeconds(delayEnd);

        // When scene is loaded and time has expired, we proceed to load next scene
        op.allowSceneActivation = true;
    }
    #endregion
    #endregion
}
