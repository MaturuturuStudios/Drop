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
    /// Movie audio source
    /// </summary>
    private AudioSource _audio;


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

    #endregion
    
    #region Public methods

    void Start () {
        // Get the movie texture
        GetComponentInChildren<RawImage>().texture = introMovie as MovieTexture;

        // Get audio source from movie
        _audio = GetComponentInChildren<AudioSource>();
        _audio.clip = introMovie.audioClip;

        // Begin load of menu in background
        StartCoroutine(ScenePreloading(sceneToLoad.name, logoDuration));

        // Get all the images that compose the logo
        _images = GetComponentsInChildren<Image>();
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
        if (Input.anyKeyDown && (_elapsedTime > logoDuration + introLockedDuration))
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
    private IEnumerator ScenePreloading(string nameScene, float timeToStart) {

        // Preload next scene
        _op = SceneManager.LoadSceneAsync(nameScene);

        // Don't load scene untill time has expired
        _op.allowSceneActivation = false;

        // Wait logo duration to start the movie
        yield return MenuNavigator.WaitForRealSeconds(timeToStart);

        // Play the movie and audio
        _audio.Play();
        introMovie.Play();
    }

    #endregion
}
