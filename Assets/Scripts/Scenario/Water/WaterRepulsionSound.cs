using UnityEngine;

/// <summary>
/// Listener to the water events which plays a sound whenever a
/// character enters or leaves it.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(WaterRepulsion))]
public class WaterRepulsionSound : MonoBehaviour, WaterRepulsionListener {

	/// <summary>
	/// Sound clip played whenever a character enters the water.
	/// </summary>
	public AudioClip enterSound;

	/// <summary>
	/// Sound clip played whenever a character exits the water.
	/// </summary>
	public AudioClip exitSound;

	/// <summary>
	/// Reference to the entity's water repulsion component.
	/// </summary>
	private WaterRepulsion _waterRepulsion;

	/// <summary>
	/// Reference to the original AudioSource component which will be
	/// cloned to play every sound.
	/// </summary>
	private AudioSource _originalAudioSource;

	void Awake() {
		// Retrieves the desired components
		_waterRepulsion = GetComponent<WaterRepulsion>();
		_originalAudioSource = GetComponent<AudioSource>();
	}

	void Start() {
		// Subscribes itself to the water's events
		_waterRepulsion.AddListener(this);
	}

	public void OnWaterEnter(WaterRepulsion water, GameObject character) {
		SoundUtility.PlaySoundAtPosition(_originalAudioSource, enterSound, character.transform.position);
	}

	public void OnWaterExit(WaterRepulsion water, GameObject character, Vector3 repulsionVelocity) {
		SoundUtility.PlaySoundAtPosition(_originalAudioSource, exitSound, character.transform.position);
	}
}
