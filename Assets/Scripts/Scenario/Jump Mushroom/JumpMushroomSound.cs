using UnityEngine;

/// <summary>
/// Listener to the mushroom's events that plays the bouncing sound.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(JumpMushroom))]
public class JumpMushroomSound : MonoBehaviour, JumpMushroomListener {

	/// <summary>
	/// If enabled, the pitch of the sound will be modified by the
	/// bouncing character's size;
	/// </summary>
	public bool modifyPitch = true;

	/// <summary>
	/// Reference to this entity's AudioSource component.
	/// </summary>
	private AudioSource _audioSource;

	void Awake() {
		// Retireves the desired components
		_audioSource = GetComponent<AudioSource>();
	}

	void Start() {
		// Subscribes itself to thepublisher
		GetComponent<JumpMushroom>().AddListener(this);
	}

	public void OnBounce(JumpMushroom mushroom, GameObject bouncingCharacter, Vector3 bounceVelocity) {
		if (modifyPitch) {
			CharacterSize characterSize = bouncingCharacter.GetComponent<CharacterSize>();
			_audioSource.pitch = 1.0f;
			if (characterSize != null)
				_audioSource.pitch /= Mathf.Sqrt(characterSize.GetSize());
		}
		_audioSource.Play();
	}
}
