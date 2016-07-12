using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Fade in and out the scenes
/// </summary>
public class LevelTransition : MonoBehaviour {

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
    #endregion


    #region Public methods

    public void Start() {
        // Get UI parent reference
        _parentUI = GameObject.FindGameObjectWithTag("Menus");

        // Get reference to audio source
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This method calls the apropiate animations for the level transitions
    /// </summary>
    /// <param name="totalDrops">Total of drops in game</param>
    /// <param name="wastedDrops">Total of drops wasted</param>
    /// <param name="startDelay">Time to start the animation</param>
    /// <param name="delayBetweenDrops">Wait before show next drop counter animation</param>
    public void BeginLevelTransitionAnim(int totalDrops, int wastedDrops, float startDelay, float delayBetweenDrops)
    {
        if (delayBetweenDrops <= -1) {
            delayBetweenDrops = waitTimeNextDrop;
        }

        // Start counter animation
        StartCoroutine(DropCounter(totalDrops, wastedDrops, startDelay, delayBetweenDrops));
    }


    /// <summary>
    /// Animation that shows the number of drops collected in the level
    /// </summary>
    /// <param name="totalDrops">Total of drops in the level</param>
    /// <param name="wastedDrops">Number of drops left in scene</param>
    /// <param name="startDelay">Wait before start animation</param>
    /// <param name="delayBetweenDrops">Wait before show next drop counter animation</param>
    public IEnumerator DropCounter(int totalDrops, int wastedDrops, float startDelay, float delayBetweenDrops) {

        // Wait to start fading
        yield return new WaitForSeconds(startDelay);

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
            yield return new WaitForSeconds(delayBetweenDrops);
        }
    }
    #endregion
}
