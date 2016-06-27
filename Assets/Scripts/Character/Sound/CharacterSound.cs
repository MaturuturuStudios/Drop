using System;
using UnityEngine;

/// <summary>
/// Manages and plays the sounds produced by the characters.
/// A copy of the object's AudioSource will be used as the
/// AudioSource for each sound.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterControllerCustom))]
[RequireComponent(typeof(CharacterSize))]
[RequireComponent(typeof(CharacterFusion))]
[RequireComponent(typeof(CharacterShoot))]
public class CharacterSound : MonoBehaviour, CharacterControllerListener, CharacterSizeListener, CharacterFusionListener, CharacterShootListener, EnemyBehaviourListener, IrrigateListener {

	#region Custom Classes

	/// <summary>
	/// Extension class for AudioInformation including some extra fields.
	/// </summary>
	[Serializable]
	public class WalkAudioInformation : SingleSoundAudioInformation {

		/// <summary>
		/// The minimum normalized speed the character should have to
		/// play this sound.
		/// </summary>
		[Range(0, 1)]
		public float minSpeedToPlay = 0.5f;
	}

	/// <summary>
	/// Extension class for AudioInformation including some extra fields.
	/// </summary>
	[Serializable]
	public class LandAudioInformation : SingleSoundAudioInformation {

		/// <summary>
		/// The minimum landing speed the character should have to
		/// play this sound.
		/// </summary>
		public float minSpeedToPlay = 3.0f;
	}

	/// <summary>
	/// Extension class for AudioInformation including some extra fields.
	/// </summary>
	[Serializable]
	public class ShootAudioInformation : AudioInformation {

		/// <summary>
		/// Sound played when the character enters shoot mode.
		/// </summary>
		public AudioClip enterShootModeSound;

		/// <summary>
		/// Sound played when the character exits shoot mode.
		/// </summary>
		public AudioClip exitShootModeSound;

		/// <summary>
		/// Sound played when the character shoots.
		/// </summary>
		public AudioClip shootSound;
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// AudioInformation for the walking sounds.
	/// </summary>
	public WalkAudioInformation walk;

	/// <summary>
	/// AudioInformation for the jumping sounds.
	/// </summary>
	public SingleSoundAudioInformation jump;

	/// <summary>
	/// AudioInformation for the landing sounds.
	/// </summary>
	public LandAudioInformation land;

	/// <summary>
	/// AudioInformation for the fusion sounds.
	/// </summary>
	public SingleSoundAudioInformation fuse;

	/// <summary>
	/// AudioInformation for the shooting sounds.
	/// </summary>
	public ShootAudioInformation shoot;

	/// <summary>
	/// AudioInformation for the hit sounds.
	/// </summary>
	public SingleSoundAudioInformation hit;

	/// <summary>
	/// AudioInformation for the irrigation sounds.
	/// </summary>
	public SingleSoundAudioInformation irrigate;

	#endregion

	#region Private Attributes

	/// <summary>
	/// Reference to the object's original AudioSource component which
	/// the values for the new ones will be copied from.
	/// </summary>
	private AudioSource _originalAudioSource;

	/// <summary>
	/// Reference to the object's CharacterControllerCustom component.
	/// </summary>
	private CharacterControllerCustom _ccc;

	/// <summary>
	/// Reference to the object's CharacterSize component.
	/// </summary>
	private CharacterSize _characterSize;

	/// <summary>
	/// Reference to the object's CharacterFusion component.
	/// </summary>
	private CharacterFusion _characterFusion;

	/// <summary>
	/// Reference to the object's CharacterShoot component.
	/// </summary>
	private CharacterShoot _characterShoot;

	#endregion

	#region Methods

	/// <summary>
	/// Unity's method called right after the object is created.
	/// </summary>
	void Awake() {
		// Retrieves the desired components
		_originalAudioSource = GetComponent<AudioSource>();
		_ccc = GetComponent<CharacterControllerCustom>();
		_characterSize = GetComponent<CharacterSize>();
		_characterFusion = GetComponent<CharacterFusion>();
		_characterShoot = GetComponent<CharacterShoot>();

		// The walk's audio source is the original one
		walk.audioSource = _originalAudioSource;

		// Creates a copy of the audio source for the other sounds
		jump.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
		land.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
		fuse.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
		shoot.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
		hit.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
		irrigate.audioSource = SoundUtility.CopyAudioSource(_originalAudioSource, gameObject);
	}

	/// <summary>
	/// Unity's method called at the beginning of the first frame
	/// this component is enabled.
	/// </summary>
	void Start() {
		// Registers the listeners
		_ccc.AddListener(this);
		_characterSize.AddListener(this);
		_characterFusion.AddListener(this);
		_characterShoot.AddListener(this);
	}
	#endregion

	#region Events

	/// <summary>
	/// This method will be called by the animation each step of the walk cycle.
	/// </summary>
	public void OnWalkStep() {
		// Checks if the character is moving and plays de audio
		float speed = Mathf.Abs(_ccc.GetNormalizedSpeed());
		if (_ccc.State.IsGrounded && speed >= walk.minSpeedToPlay)
			walk.PlayAudio(speed / Mathf.Sqrt(_characterSize.GetSize()));
	}

	public void OnBeginJump(CharacterControllerCustom ccc, float delay) {
		// Plays the jump sound modified by the character rooted size
		jump.PlayAudio(1.0f / Mathf.Sqrt(_characterSize.GetSize()));
	}

	public void OnPerformJump(CharacterControllerCustom ccc) {
		// Do nothing
	}

	public void OnWallJump(CharacterControllerCustom ccc) {
		// Plays the jump sound
		OnBeginJump(ccc, 0);
	}

	public void OnPreCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		// Checks if the character is landing and plays the sound
		Vector3 hitVelocity = Vector3.Project(ccc.Velocity, hit.normal);
		if (!ccc.State.IsGrounded && hitVelocity.magnitude >= land.minSpeedToPlay * Mathf.Sqrt(ccc.GetComponent<CharacterSize>().GetSize()))
			land.PlayAudio();
	}

	public void OnPostCollision(CharacterControllerCustom ccc, ControllerColliderHit hit) {
		// Do nothing
	}

	public void OnChangeSizeEnd(GameObject character, Vector3 previousScale, Vector3 nextScale) {
		// Do nothing
	}

	public void OnChangeSizeStart(GameObject character, Vector3 previousScale, Vector3 nextScale) {
		// Do nothing
	}

	public void OnSpitDrop(GameObject character, GameObject spittedCharacter) {
		// Plays the shoot sound
		OnShoot(_characterShoot, spittedCharacter, Vector3.zero);
	}

	public void OnBeginFusion(CharacterFusion originalCharacter, GameObject fusingCharacter, ControllerColliderHit hit) {
		// Plays the fusion sound
		fuse.PlayAudio();
	}

	public void OnEndFusion(CharacterFusion finalCharacter) {
		// Do nothing
	}

	public void OnEnterShootMode(CharacterShoot character) {
		// Plays the enter shoot mode sound
		shoot.PlayAudio(shoot.enterShootModeSound);
	}

	public void OnExitShootMode(CharacterShoot character) {
		// Plays the exit shoot mode sound
		shoot.PlayAudio(shoot.exitShootModeSound);
	}

	public void OnShoot(CharacterShoot shootingCharacter, GameObject shotCharacter, Vector3 velocity) {
		// Plays the shoot sound
		shoot.PlayAudio(shoot.shootSound);
	}

	public void OnBeginChase(AIBase enemy, GameObject chasedObject) {
		// Do nothing
	}

	public void OnEndChase(AIBase enemy, GameObject chasedObject) {
		// Do nothing
	}

	public void OnAttack(AIBase enemy, GameObject attackedObject, Vector3 velocity) {
		// Plays the hit sound
		if (attackedObject == gameObject)
			hit.PlayAudio();
    }

	public void OnBeingScared(AIBase enemy, GameObject scaringObject, int scaringSize) {
		// Do nothing
	}

	public void OnStateAnimationChange(AnimationState previousState, AnimationState actualState) {
		// Do nothing
	}

	public void OnIrrigate(Irrigate irrigated, GameObject irrigating, int dropsConsumed) {
		// Plays the irrigation sound
		if (irrigating == gameObject)
			irrigate.PlayAudio();
	}

	#endregion
}