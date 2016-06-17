using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Fade in and out the scenes
/// </summary>
public class LevelEndAnim : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// The animation of one drop collected
    /// </summary>
    public GameObject dropCounterUnitCollected;


    /// <summary>
    /// The animation of one drop wasted on the level
    /// </summary>
    public GameObject dropCounterUnitWasted;


    /// <summary>
    /// Wait time to start next drop animation
    /// </summary>
    public float waitTimeNextDrop;


    /// <summary>
    /// Height position of displayed drops
    /// </summary>
    [Range (-.5f,.5f)]
    public float heightPosition = 0.1f;

    /// <summary>
    /// Sound played when the character enters shoot mode.
    /// </summary>
    public AudioClip countDropCollectedSound;

    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    public AudioClip countDropWastedSound;

    #endregion

    #region Private Attributes

    /// <summary>
    /// parent UI object
    /// </summary>
    private GameObject _parentUI;

    /// <summary>
    /// Reference to the object's original AudioSource component which
    /// the values for the new ones will be copied from.
    /// </summary>
    private AudioSource _audioSource;

    #endregion


    #region Methods
    #region Public methods

    public void Start() {
        _parentUI = GameObject.FindGameObjectWithTag("Menus");
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnGUI() {

    }

    /// <summary>
    /// Change to the next scene with a fading. This is the method that should be called
    /// </summary>
    /// <param name="nameScene">The name of the next scene</param>
    /// <param name="delayStart">Delay should wait before starting. By default -1 that means the public attribute delayStartChangeSeconds
    /// on this script will be used</param>
    /// <param name="desiredFadeDuration">Time should delay fading the screen to black. By default -1 that means the public attribute fadeDuration
    /// on this script will be used</param>
    /// /// <param name="delayEnd">Delay should wait after ending. By default -1 that means the public attribute delaySEndChangeSeconds
    /// on this script will be used</param>
    public void BeginLevelEndAnimation(int totalDrops, int wastedDrops, float startDelay, float delayBetweenDrops) {
        if (delayBetweenDrops <= -1) {
            delayBetweenDrops = waitTimeNextDrop;
        }

        StartCoroutine(DropCounter(totalDrops, wastedDrops, startDelay, delayBetweenDrops));
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Animation that shows the number of drops collected in the level
    /// </summary>
    /// <param name="totalDrops">Total of drops in the level</param>
    /// <param name="wastedDrops">Number of drops left in scene</param>
    /// <param name="delayBetweenDrops">Wait before show next drop</param>
    /// <returns></returns>
    private IEnumerator DropCounter(int totalDrops, int wastedDrops, float startDelay, float delayBetweenDrops) {

        // Wait to start fading
        yield return MenuNavigator.WaitForRealSeconds(startDelay);

        for ( int i = 0; i < totalDrops; ++i) {

            // Play sound
            GameObject drop2DAnim; 
            if (i < totalDrops - wastedDrops) {
                drop2DAnim = GameObject.Instantiate(dropCounterUnitCollected);
                _audioSource.clip = countDropCollectedSound;
                drop2DAnim.GetComponent<Animator>().SetBool("collected", true);
            } else {
                drop2DAnim = GameObject.Instantiate(dropCounterUnitWasted);
                _audioSource.clip = countDropWastedSound;
                drop2DAnim.GetComponent<Animator>().SetBool("collected", false);
            }

            _audioSource.Play();


            // Create a new animation

            // Set animation UI Component
            drop2DAnim.transform.SetParent(_parentUI.transform, false);

            drop2DAnim.transform.SetAsLastSibling();

            // Calculate size of animation
            float animSize = 1024 / (totalDrops + ((totalDrops - 1) / 2) + 2);

            // Set size of animation
            drop2DAnim.GetComponent<RectTransform>().sizeDelta = new Vector2(animSize, animSize);

            // Set position of animation
            float horizontalPosition = (animSize / 2) + animSize + (animSize * i) + (animSize * i / 2);
            if (totalDrops % 2 == 0)
                horizontalPosition -= animSize / 4;
            drop2DAnim.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalPosition, Screen.height * heightPosition);

            // Wait for next animation
            yield return MenuNavigator.WaitForRealSeconds(delayBetweenDrops);
        }
    }
    #endregion
    #endregion
}
