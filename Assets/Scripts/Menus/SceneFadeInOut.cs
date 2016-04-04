using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


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
    public Texture2D fadeOutTexture;
    /// <summary>
    /// The fading speed
    /// </summary>
    public float fadeSpeed = 0.8f;
    #endregion

    #region Private Attributes
    /// <summary>
    /// the texture's order in the draw hierarchy: a low number means it renders on top
    /// </summary>
    private int _drawDepth = -1000;
    /// <summary>
    /// the texture's alpha value between 0 and 1
    /// </summary>
    private float _alpha = 1.0f;
    /// <summary>
    /// the direction to fade: in = -1 or out = 1
    /// </summary>
    private int _fadeDir = -1;
    #endregion


    #region Methods
    #region Public methods
    public void OnGUI() {
        // fade out/in the alpha value using a direction, a speed and Time.deltaTime to convert the operation to seconds
        _alpha += _fadeDir * fadeSpeed * Time.deltaTime;
        // force (clamp) the number to be between 0 and 1 because GUI.color uses Alpha values between 0 and 1
        _alpha = Mathf.Clamp01(_alpha);

        // set color of our GUI (in this case our texture). All color values remain the same & the Alpha is set to the alpha variable
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, _alpha);
        // make the black texture render on top (drawn last)
        GUI.depth = _drawDepth;
        // draw the texture to fit the entire screen area
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    /// <summary>
    /// Sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
    /// </summary>
    /// <param name="direction"> fade: in = -1 or out = 1</param>
    /// <returns>the speed of complete fading</returns>
    public float BeginFade(int direction) {
        _fadeDir = direction;
        return (fadeSpeed);
    }

    /// <summary>
    /// OnLevelWasLoaded is called when a level is loaded.
    /// </summary>
    public void OnLevelWasLoaded() {
        //fade...
        BeginFade(-1);
    }

    /// <summary>
    /// Change to the next scene with a fading. This is the method that should be called
    /// </summary>
    /// <param name="nameScene">The name of the next scene</param>
    /// <param name="delayStart">Delay should wait before starting. By default -1 that means the public attribute delayStartChangeSeconds
    /// on this script will be used</param>
    /// /// <param name="delayEnd">Delay should wait after ending. By default -1 that means the public attribute delaySEndChangeSeconds
    /// on this script will be used</param>
    public void ChangeScene(string nameScene, float delayStart=-1, float delayEnd=-1) {
        if (delayStart <= -1) {
            delayStart = delayStartChangeSeconds;
        }

        if (delayEnd <= -1) {
            delayEnd = delayEndChangeSeconds;
        }

        StartCoroutine(NextScene(nameScene, delayStart, delayEnd));
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Change the scenes
    /// Wait few seconds before starting the fade between actual scene and next scene
    /// </summary>
    /// <param name="nameScene">Next scene</param>
    /// <param name="delayStart">Wait before starting</param>
    /// <param name="delayEnd">Wait after ending</param>
    /// <returns></returns>
    private IEnumerator NextScene(string nameScene, float delayStart, float delayEnd) {
        yield return MenuNavigator.WaitForRealSeconds(delayStart);

        float fadeTime = BeginFade(1);
        yield return MenuNavigator.WaitForRealSeconds(fadeTime);

        yield return MenuNavigator.WaitForRealSeconds(delayEnd);
        SceneManager.LoadScene(nameScene, LoadSceneMode.Single);
    }
    #endregion
    #endregion
}
