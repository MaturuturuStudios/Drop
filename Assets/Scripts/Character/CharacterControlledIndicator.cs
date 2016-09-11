using UnityEngine;
using System.Collections;

public class CharacterControlledIndicator : MonoBehaviour {

    #region Public attributes
    
    /// <summary>
    /// Drop switch sound effect
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// DAnimator controller
    /// </summary>
    public Animator animator;

    #endregion

    #region Public Methods

    public void PlayChangeDropEffect() {

        StartCoroutine(PlayChangeDropCoroutine());

    }

    private IEnumerator PlayChangeDropCoroutine() {
                   
        // Play the sound effect
        audioSource.Play();

        animator.SetBool("isSelected", false);
        //returning 0 will make it wait 1 frame
        yield return 0;

        animator.SetBool("isSelected", true);


    }

    #endregion
}
