using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Fade in and out the scenes
/// </summary>
public class LevelEndThanks : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// End mesage for publicity
    /// </summary>
    public GameObject endMessageText;


    /// <summary>
    /// End composion with www and background image
    /// </summary>
    public GameObject endContactComposion;

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
    /// the texture's alpha value between 0 and 1 of thanks text
    /// </summary>
    private float _alpha = 0.0f;


    /// <summary>
    /// the texture's alpha value between 0 and 1 of back image
    /// </summary>
    private float _alphaBack = 0.0f;


    /// <summary>
    /// the texture's alpha value between 0 and 1 of about text
    /// </summary>
    private float _alphaBackText = 0.0f;


    /// <summary>
    /// Reference to thanks message to make the fade effect
    /// </summary>
    GameObject _ThanksMessage;


    /// <summary>
    /// Reference to end composition to make the fade effect
    /// </summary>
    Text[] _endCompositionTexts;


    /// <summary>
    /// Reference to thanks back image
    /// </summary>
    Image _endBackImage;

    #endregion

    
    #region Public methods

    public void Start() {
        // Get UI parent reference
        _parentUI = GameObject.FindGameObjectWithTag("Menus");

        // Get reference to audio source
        _audioSource = GetComponent<AudioSource>();

        // Set _endComposionText an empty list

        _endCompositionTexts = new Text[0];
    }


    public void Update() {
        // If there is any end message, display it
        if (_ThanksMessage) {

            // Calculate and set the alpha value
            _alpha += Time.deltaTime / _fadeDuration;
            _ThanksMessage.GetComponent<Text>().color = new Color(1, 1, 1, _alpha);
        }

        // If there is any end composion text, display it
        if (_endCompositionTexts != null && _endCompositionTexts.Length > 0) {

            // Calculate and set the alpha value
            _alphaBackText += Time.deltaTime / 2;
            for (int i = 0; i < _endCompositionTexts.Length; ++i) {
                _endCompositionTexts[i].GetComponent<Text>().color = new Color(1, 1, 1, _alphaBackText);
            }
        }

        // If there is any end composion text, display it
        if (_endBackImage) {

            // Calculate and set the alpha value
            _alphaBack += Time.deltaTime / 2;
            _endBackImage.color = new Color(1, 1, 1, _alphaBack);
        }
    }
    

    /// <summary>
    /// Shows the end message of thanks
    /// </summary>
    /// <param name="fadeDuration">Desired duration while fading</param>
    /// <returns></returns>
    public IEnumerator EndMessage(float fadeDuration) {

        // Set desired duration
        _fadeDuration = fadeDuration;

        // Instantiate the end composition
        GameObject endCompostion = Instantiate(endContactComposion, Vector3.zero, Quaternion.identity) as GameObject;

        // Set composition an UI object
        endCompostion.transform.SetParent(_parentUI.transform, false);

        // Instantiate an end message
        _ThanksMessage = Instantiate(endMessageText, Vector3.zero, Quaternion.identity) as GameObject;

        // Set object an UI object
        _ThanksMessage.transform.SetParent(_parentUI.transform, false);

        // Set the message not visible
        _ThanksMessage.GetComponent<Text>().color = new Color(1, 1, 1, 0);

        // Wait for display the next elements
        yield return new WaitForSeconds(3f);

        // Get back image
        _endBackImage = endCompostion.GetComponentInChildren<Image>();

        // Wait for display the next elements
        yield return new WaitForSeconds(2f);

        // Get Contact texts
        _endCompositionTexts = endCompostion.GetComponentsInChildren<Text>();

        // Wait for play thanks sound
        //yield return new WaitForSeconds(2f);
        //_audioSource.Play();

        yield return true;
    }
    #endregion
}
