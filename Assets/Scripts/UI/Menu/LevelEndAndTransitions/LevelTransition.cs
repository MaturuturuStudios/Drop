using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Fade in and out the scenes
/// </summary>
public class LevelTransition : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// Level complete text
    /// </summary>
    public GameObject levelCompleteText;

    /// <summary>
    /// The animation of one drop collected
    /// </summary>
    public GameObject dropCounterUnitCollected;


    /// <summary>
    /// The animation of one drop wasted on the level
    /// </summary>
    public GameObject dropCounterUnitWasted;

    /// <summary>
    /// Sound played when the character enters shoot mode.
    /// </summary>
    public AudioClip countDropCollectedSound;


    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    public AudioClip countDropWastedSound;


    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    public AudioClip levelCompleteSound1;


    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    public AudioClip levelCompleteSound2;


    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    public AudioClip levelCompleteSound3;


    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    public string levelCompleteText1;


    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    public string levelCompleteText2;


    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    public string levelCompleteText3;


    /// <summary>
    /// Sound played when the character exits shoot mode.
    /// </summary>
    [Range (1,5)]
    public int maxDropsRequired;


    /// <summary>
    /// Height position of displayed drops
    /// </summary>
    [Range (-.5f,.5f)]
    public float dropsHeightPosition = 0f;

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
    /// <param name="collectedDrops">Number of drops collected in game</param>
    /// <param name="startDelay">Time to start the animation</param>
    /// <param name="delayBetweenDrops">Wait before show next drop counter animation</param>
    public void BeginLevelTransitionAnim(int totalDrops, float startDelay, float fadeDuration)
    {
        // Look for delay between drops animations
        float delayBetweenDrops = totalDrops > maxDropsRequired ? fadeDuration / totalDrops : fadeDuration / maxDropsRequired;

        // Start counter animation
        StartCoroutine(DropCounter(totalDrops, startDelay, delayBetweenDrops));
    }


    /// <summary>
    /// Animation that shows the number of drops collected in the level
    /// </summary>
    /// <param name="collectedDrops">Number of drops collected  in the level</param>
    /// <param name="startDelay">Wait before start animation</param>
    /// <param name="delayBetweenDrops">Wait before show next drop counter animation</param>
    public IEnumerator DropCounter(int collectedDrops, float startDelay, float delayBetweenDrops) {



        // Instialize the message and the sound
        if (collectedDrops < maxDropsRequired) {
            levelCompleteText.GetComponent<Text>().text = levelCompleteText1;
            _audioSource.clip = levelCompleteSound1;
        } else if (collectedDrops == maxDropsRequired) {
            levelCompleteText.GetComponent<Text>().text = levelCompleteText2;
            _audioSource.clip = levelCompleteSound2;
        } else {
            levelCompleteText.GetComponent<Text>().text = levelCompleteText3;
            _audioSource.clip = levelCompleteSound3;
        }

        // Instantiate the message and the sound
        GameObject _ThanksMessage = Instantiate(levelCompleteText, Vector3.zero, Quaternion.identity) as GameObject;
        _audioSource.Play();

        // Set object an UI object
        _ThanksMessage.transform.SetParent(_parentUI.transform, false);

        // Wait to start fading
        yield return new WaitForSeconds(startDelay);

        // Look if player exceded max drops
        int dropsToShow = collectedDrops > maxDropsRequired ? collectedDrops : maxDropsRequired;

        for (int i = 0; i < dropsToShow; ++i) {

            GameObject drop2DAnim;
            if (i < collectedDrops) {                
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
            float animSize = 1024 / (dropsToShow + ((dropsToShow - 1) / 2) + 2);

            // Set size of animation
            drop2DAnim.GetComponent<RectTransform>().sizeDelta = new Vector2(animSize, animSize);

            // Set position of animation
            float horizontalPosition = (animSize / 2) + animSize + (animSize * i) + (animSize * i / 2);
            if (dropsToShow % 2 == 0)
                horizontalPosition -= animSize / 4;
            drop2DAnim.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalPosition, Screen.height * dropsHeightPosition);

            // Wait for next animation
            yield return new WaitForSeconds(delayBetweenDrops);
        }
    }
    #endregion
}
