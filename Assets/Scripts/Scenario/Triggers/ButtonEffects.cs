using UnityEngine;

/// <summary>
/// Contains methods to create the effects of the buttos.
/// </summary>
public class ButtonEffects : MonoBehaviour {

    /// <summary>
    /// Key for the animator field.
    /// </summary>
    public static readonly string ANIMATOR_KEY = "pressed";

    /// <summary>
    /// Reference to the animator.
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Sound played when the button is pressed.
    /// </summary>
    public AudioClip pressSound;

    /// <summary>
    /// Sound played when the button is released.
    /// </summary>
    public AudioClip releaseSound;

    /// <summary>
    /// Particle effect played when the button is pressed.
    /// </summary>
    public EffectInformation particleEffect;

    /// <summary>
    ///  Reference to this entity's AudioSource component.
    /// </summary>
    private AudioSource _audioSource;

    /// <summary>
    ///  Reference to this entity's Transform component.
    /// </summary>
    private Transform _transform;

    void Start() {
        // Retrieves the desired components
        _audioSource = GetComponent<AudioSource>();
        _transform = transform;
    }

    /// <summary>
    /// Plays the press effects.
    /// </summary>
    public void Press() {
        // Plays the animation
        animator.SetBool(ANIMATOR_KEY, true);

        // Plays the sound
        _audioSource.clip = pressSound;
        _audioSource.Play();

        // Creates the particle effect
        particleEffect.PlayEffect(_transform.position, _transform.rotation);
    }

    /// <summary>
    /// Plays the release effects.
    /// </summary>
    public void Release() {
        // Plays the animation
        animator.SetBool(ANIMATOR_KEY, false);

        // Plays the sound
        _audioSource.clip = releaseSound;
        _audioSource.Play();
    }
}