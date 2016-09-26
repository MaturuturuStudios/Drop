using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// Next scene to be loaded
    /// </summary>
    public Scene sceneToLoad;


    /// <summary>
    /// Movie to be displayed an start
    /// </summary>
    public MovieTexture introMovie;

    /// <summary>
    /// Movie audio source
    /// </summary>
    public AudioSource introAudio;


    /// <summary>
    /// Audio to be played with the logo
    /// </summary>
    public AudioClip maturuturuClip;


    /// <summary>
    /// Duration that logo will be displayed on screen
    /// </summary>
    public float logoDuration = 1f;


    /// <summary>
    /// Duration of the logo's fade
    /// </summary>
    public float logoFadeDuration = 0.5f;


    /// <summary>
    /// Time waiting untill intro can be skipped
    /// </summary>
    public float introLockedDuration = 2f;    

    #endregion

    #region Private attributes


    /// <summary>
    /// The operation to load the menu in background
    /// </summary>
    private AsyncOperation _op;


    /// <summary>
    /// The texture's alpha value between 0 and 1
    /// </summary>
    private float _alpha = 1.0f;


    /// <summary>
    /// List of imatges that compose the intro logo
    /// </summary>
    private Image[] _images;


    /// <summary>
    /// Elapsed time counter
    /// </summary>
    private float _elapsedTime = 0f;

    /// <summary>
    /// Control if the video has started
    /// </summary>
    private bool startedVideo = false;

    /// <summary>
    /// Control if the video has started
    /// </summary>
    private SceneFadeInOut _sfio;

    #endregion

    #region Public methods

    void Start () {
        // Get the movie texture
        GetComponentInChildren<RawImage>().texture = introMovie as MovieTexture;

        // Begin load of menu in background
        StartCoroutine(PlayLogoSound(logoDuration - 0.1f));

        // Begin load of menu in background
        StartCoroutine(ScenePreloading(sceneToLoad.name, logoDuration));

        // Get all the images that compose the logo
        _images = GetComponentsInChildren<Image>();

        // Get fade component
        _sfio = GetComponent<SceneFadeInOut>();
    }


	void Update () {
        // Count elapsed time
        _elapsedTime += Time.deltaTime;

        // If logo duration is exceded, start fading
        if (_elapsedTime >= logoDuration) {

            // fade out/in the alpha value using the desired time
            _alpha -= Time.deltaTime / logoFadeDuration;

            // Aply alpha
            if (_images != null)
                for (int i = 0; i < _images.Length; ++i) {
                    _images[i].GetComponent<Image>().color = new Color(_images[i].GetComponent<Image>().color.r, _images[i].GetComponent<Image>().color.g, _images[i].GetComponent<Image>().color.b, _alpha);
                }
        }

        // Skip intro
        if ((Input.GetButtonDown(Axis.Action) ||
            Input.GetButtonDown(Axis.Jump) ||
            Input.GetButtonDown(Axis.Irrigate) ||
            Input.GetButtonDown(Axis.ShootMode) ||
            Input.GetButtonDown(Axis.Start)) &&
            (_elapsedTime > logoDuration + introLockedDuration))
                StartCoroutine(SkipScene());

        //if finished video
        if (startedVideo && !introMovie.isPlaying)
            _op.allowSceneActivation = true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Threat that loads the menu in background
    /// </summary>
    /// <param name="nameScene"></param>
    /// <param name="timeToStart"></param>
    /// <returns></returns>
    private IEnumerator SkipScene() {

        // Do fade effect
        _sfio.BeginFade(false, 0.8f);

        // Wait for fade done
        yield return MenuNavigator.WaitForRealSeconds(0.8f);

        // Change scene
        _op.allowSceneActivation = true;
    }


    /// <summary>
    /// Threat that loads the menu in background
    /// </summary>
    /// <param name="nameScene"></param>
    /// <param name="timeToStart"></param>
    /// <returns></returns>
    private IEnumerator ScenePreloading(string nameScene, float timeToStart) {

        // Preload next scene
        _op = SceneManager.LoadSceneAsync(nameScene);

        // Don't load scene untill time has expired
        _op.allowSceneActivation = false;

        // Wait logo duration to start the movie
        yield return MenuNavigator.WaitForRealSeconds(timeToStart);

        // Play the movie and audio
        introAudio.Play();
        introMovie.Play();
        startedVideo = true;
    }

    /// <summary>
    /// Threat that loads the menu in background
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayLogoSound(float timeToStop) {

        // Load logo sound
        introAudio = GetComponentInChildren<AudioSource>();
        introAudio.clip = maturuturuClip;
        introAudio.Play();

        // Wait logo duration to start the movie
        yield return MenuNavigator.WaitForRealSeconds(timeToStop);

        // Get audio source from movie
        introAudio.clip = introMovie.audioClip;
    }

    #endregion
}
