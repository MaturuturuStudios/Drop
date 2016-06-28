using UnityEngine;

/// <summary>
/// Class which plays the different sounds produced by the ant.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AntSounds : MonoBehaviour, EnemyBehaviourListener {

	/// <summary>
	/// Sound played when the ant starts chaising a character.
	/// </summary>
	public AudioClip chaseSound;

	/// <summary>
	/// Sound played when the ant is scared by a character.
	/// </summary>
	public AudioClip scaredSound;

	/// <summary>
	/// Reference to the parent's AI component.
	/// </summary>
	private AIAnt _ai;

	/// <summary>
	/// Reference to this entity's AudioSource component.
	/// </summary>
	private AudioSource _audioSource;

	void Awake() {
		// Retrieves the desired components
		_ai = GetComponentInParent<AIAnt>();
		_audioSource = GetComponent<AudioSource>();
	}

	void Start() {
		// Subscribes itself to the AI events
		_ai.AddListener(this);
	}

	public void OnAttack(AIBase enemy, GameObject attackedObject, Vector3 velocity) {
		// Do nothing
	}

	public void OnBeginChase(AIBase enemy, GameObject chasedObject) {
		// Plays the chase sound
		_audioSource.clip = chaseSound;
		_audioSource.Play();
	}

	public void OnEndChase(AIBase enemy, GameObject chasedObject) {
		// Do nothing
	}

	public void OnBeingScared(AIBase enemy, GameObject scaringObject, int scaringSize) {
		// Plays the scared sound
		_audioSource.clip = scaredSound;
		_audioSource.Play();
	}

	public void OnStateAnimationChange(AnimationState previousState, AnimationState actualState) {
		// Do nothing
	}
}
