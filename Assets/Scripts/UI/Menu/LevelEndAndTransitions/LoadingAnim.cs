using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Animation shown while scene is loading in background
/// </summary>
public class LoadingAnim : MonoBehaviour {

    #region Public attributes

    /// <summary>
    /// Loading animation prefab
    /// </summary>
    public GameObject loadingAnim;

    #endregion

    #region Private Attributes

    /// <summary>
    /// Parent UI object
    /// </summary>
    private GameObject _parentUI;

    #endregion

    #region Public methods

    public void Start() {
        _parentUI = GameObject.FindGameObjectWithTag("Menus");
    }


    /// <summary>
    /// Play animatión while scene is loading
    /// </summary>
    /// <param name="op">The operation by which we are waiting</param>
    /// <returns></returns>
    public IEnumerator PlayLoadingAnim(AsyncOperation op) {

        // Instantiate the animation at the botom right of the screen
        GameObject loadingIndicator = Instantiate(loadingAnim,new Vector3(-32f, 32f,0f) , Quaternion.identity) as GameObject;

        // Set the loading indicator part of the UI
        loadingIndicator.transform.SetParent(_parentUI.transform, false);

        // Set visible on top
        loadingIndicator.transform.SetAsLastSibling();

        // While progress is fewer than 0.9, it is still loading 
        while (op.progress < 0.9f) {
            yield return null;
        }

        // If progress is greater than 0.9 it is loaded, we can delete the animation
        Destroy(loadingIndicator);
    }

    #endregion
}
