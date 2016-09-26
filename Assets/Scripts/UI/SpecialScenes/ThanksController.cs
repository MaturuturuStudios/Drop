using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ThanksController : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// Next scene to be loaded
    /// </summary>
    public Scene sceneToLoad;


    /// <summary>
    /// Screenshoots to be displayed an start
    /// </summary>
    public Sprite[] screenshoots;


    /// <summary>
    /// Image container to be instiantated
    /// </summary>
    public GameObject imageContainer;


    /// <summary>
    /// Duration to show next image
    /// </summary>
    public float imageDuration = 5f;


    /// <summary>
    /// Time waiting untill intro can be skipped
    /// </summary>
    public float lockedDuration = 15f;

    #endregion

    #region Private attributes


    /// <summary>
    /// The operation to load the menu in background
    /// </summary>
    private AsyncOperation _op;


    /// <summary>
    /// Elapsed time counter
    /// </summary>
    private float _elapsedTime = 0f;

    /// <summary>
    /// Control if the video has started
    /// </summary>
    private SceneFadeInOut _sfio;


    /// <summary>
    /// Index of image showing
    /// </summary>
    private int _actualImage = -1;

    #endregion

    #region Public methods

    void Start() {

        // Preload next scene
        _op = SceneManager.LoadSceneAsync(sceneToLoad.name);

        // Don't load scene untill time has expired
        _op.allowSceneActivation = false;

        if (screenshoots.Length > 2) {
            // Begin background animation
            StartCoroutine(BackgroundEffect());
        }

        // Get fade component
        _sfio = GameObject.FindObjectOfType<SceneFadeInOut>();
    }


    void Update() {
        // Count elapsed time
        _elapsedTime += Time.deltaTime;

        // Skip intro
        if ((Input.GetButtonDown(Axis.Action) ||
            Input.GetButtonDown(Axis.Jump) ||
            Input.GetButtonDown(Axis.Irrigate) ||
            Input.GetButtonDown(Axis.ShootMode) ||
            Input.GetButtonDown(Axis.Start)) &&
            (_elapsedTime > lockedDuration)) {
                StartCoroutine(SkipScene());
        }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Threat that loads the menu in background
    /// </summary>
    private IEnumerator SkipScene() {

        // Do fade effect
        _sfio.BeginFade(false, 0.8f);

        // Wait for fade done
        yield return MenuNavigator.WaitForRealSeconds(0.8f);

        // Change scene
        _op.allowSceneActivation = true;
    }

    /// <summary>
    /// Threat that loads different images in background
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackgroundEffect() {

        while (true) {
            // Instantiate the next image
            GameObject background = Instantiate(imageContainer, transform, false) as GameObject;

            // Set it child of canvas
            background.transform.SetParent(transform);

            // Assign current image
            background.GetComponent<Image>().sprite = GetNextImage();

            // Wait logo duration to start the movie
            yield return MenuNavigator.WaitForRealSeconds(imageDuration);
        }
    }

    private Sprite GetNextImage() {
        ++_actualImage;

        if(_actualImage >= screenshoots.Length) {
            _actualImage = 0;
        }

        return screenshoots[_actualImage];
    }

    #endregion
}
