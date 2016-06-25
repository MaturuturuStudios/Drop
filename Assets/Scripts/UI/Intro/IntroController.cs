using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {

    public Scene sceneToLoad;

    public MovieTexture introMovie;

    public GameObject logoBackGround;

    public GameObject logo;

    public float logoDuration = 1f;

    public float logoFadeDuration = 0.5f;

    private AudioSource _audio;

    private AsyncOperation _op;
    /// <summary>
    /// the texture's alpha value between 0 and 1
    /// </summary>
    private float _alpha = 1.0f;

    /// <summary>
    /// the texture's alpha value between 0 and 1
    /// </summary>
    private Image[] _images;

    /// <summary>
    /// Elapsed time counter
    /// </summary>
    private float _elapsedTime = 0f;

    void Start () {
        GetComponentInChildren<RawImage>().texture = introMovie as MovieTexture;
        _audio = GetComponentInChildren<AudioSource>();
        _audio.clip = introMovie.audioClip;

        StartCoroutine(ScenePreloading(sceneToLoad.name, logoDuration));

        _images = GetComponentsInChildren<Image>();
    }
	
	// Update is called once per frame
	void Update () {

        _elapsedTime += Time.deltaTime;

        if(_elapsedTime >= logoDuration) {

            // fade out/in the alpha value using the desired time
            _alpha -= Time.deltaTime / logoFadeDuration;

            if (_images != null)
                for (int i = 0; i < _images.Length; ++i) {
                    _images[i].GetComponent<Image>().color = new Color(_images[i].GetComponent<Image>().color.r, _images[i].GetComponent<Image>().color.g, _images[i].GetComponent<Image>().color.b, _alpha);
                }
        }

        if (Input.anyKeyDown || (_elapsedTime > logoDuration + 0.1 && !introMovie.isPlaying))
            _op.allowSceneActivation = true;
    }

    private IEnumerator ScenePreloading(string nameScene, float timeToStart) {

        // Preload next scene
        _op = SceneManager.LoadSceneAsync(nameScene);

        // Don't load scene untill time has expired
        _op.allowSceneActivation = false;

        // Wait desired time to start the movie
        yield return MenuNavigator.WaitForRealSeconds(timeToStart);

        // Play the movie
        _audio.Play();
        introMovie.Play();
    }
}
