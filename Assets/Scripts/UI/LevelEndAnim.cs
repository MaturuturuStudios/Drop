using UnityEngine;
using System.Collections;
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
    /// End mesage for publicity
    /// </summary>
    public GameObject EndMessageText;


    /// <summary>
    /// Wait time to start next drop animation, it is only used if no parameter getted in BeginLevelEndAnimation
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


    /// <summary>
    /// Duration while fading
    /// </summary>
    public float _fadeDuration = 0.8f;


    /// <summary>
    /// the texture's alpha value between 0 and 1
    /// </summary>
    private float _alpha = 0.0f;


    /// <summary>
    /// Reference to end message to make the fade effect
    /// </summary>
    GameObject _endeMessage;
    #endregion


    #region Methods
    #region Public methods

    public void Start() {
        // Get UI parent reference
        _parentUI = GameObject.FindGameObjectWithTag("Menus");

        // Get reference to audio source
        _audioSource = GetComponent<AudioSource>();
    }


    public void Update() {
        // If there is any end message
        if (_endeMessage) {

            // Calculate and set the alpha value
            _alpha += Time.deltaTime / _fadeDuration;
            _endeMessage.GetComponent<Text>().color = new Color(1, 1, 1, _alpha);
        }
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

        // Drop counter animation
        //StartCoroutine(DropCounter(totalDrops, wastedDrops, startDelay, delayBetweenDrops));
    }
    

    /// <summary>
    /// Shows the end message of thanks
    /// </summary>
    /// <param name="fadeDuration">Desired duration while fading</param>
    /// <returns></returns>
    public IEnumerator EndMessage(float fadeDuration) {

        // Set desired duration
        _fadeDuration = fadeDuration;

        // Instantiate an end message
        _endeMessage = Instantiate(EndMessageText, Vector3.zero, Quaternion.identity) as GameObject;

        // Set object a UI object
        _endeMessage.transform.SetParent(_parentUI.transform, false);

        // Set the message not visible
        _endeMessage.GetComponent<Text>().color = new Color(1, 1, 1, 0);
        yield return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Animation that shows the number of drops collected in the level
    /// </summary>
    /// <param name="totalDrops">Total of drops in the level</param>
    /// <param name="wastedDrops">Number of drops left in scene</param>
    /// <param name="startDelay">Wait before start animation</param>
    /// <param name="delayBetweenDrops">Wait before show next drop</param>
    private IEnumerator DropCounter(int totalDrops, int wastedDrops, float startDelay, float delayBetweenDrops) {

        // Wait to start fading
        yield return MenuNavigator.WaitForRealSeconds(startDelay);

        for (int i = 0; i < totalDrops; ++i) {

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

            // Play sound
            _audioSource.Play();

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


    /// <summary>
    /// Animation shown while level is loading
    /// </summary>
    /// <param name="totalDrops">Total of drops in the level</param>
    /// <param name="wastedDrops">Number of drops left in scene</param>
    /// <param name="startDelay">Wait before start animation</param>
    /// <param name="delayBetweenDrops">Wait before show next drop</param>
    /// <returns></returns>
    private IEnumerator LevelLoading(int totalDrops, int wastedDrops, float startDelay, float delayBetweenDrops) {

        // Wait to start fading
        yield return MenuNavigator.WaitForRealSeconds(startDelay);

        for (int i = 0; i < totalDrops; ++i) {

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

            // Play sound
            _audioSource.Play();

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
