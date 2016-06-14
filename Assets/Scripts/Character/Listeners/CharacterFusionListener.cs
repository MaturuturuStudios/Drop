using UnityEngine;

/// <summary>
/// Interface for the observers listening for the
/// character fusion's events.
/// </summary>
public interface CharacterFusionListener {

	/// <summary>
	/// Event called when two characters begin fusing together.
	/// </summary>
	/// <param name="originalCharacter">The character beginning the fusion</param>
	/// <param name="fusingCharacter">The character disappearing after the fusion</param>
	/// <param name="hit">Information about the hit</param>
	void OnBeginFusion(CharacterFusion originalCharacter, GameObject fusingCharacter, ControllerColliderHit hit);

	/// <summary>
	/// Event called when two characters end fusing together.
	/// </summary>
	/// <param name="finalCharacter">The fused character</param>
	void OnEndFusion(CharacterFusion finalCharacter);
}

/// <summary>
/// Adapter for the CharacterFusionListener interface used to
/// avoid forcing each class to implement all it's methods.
/// </summary>
public class CharacterFusionAdapter : MonoBehaviour, CharacterFusionListener {

	public void OnBeginFusion(CharacterFusion originalCharacter, GameObject fusingCharacter, ControllerColliderHit hit) {
		// Do nothing
	}

	public void OnEndFusion(CharacterFusion finalCharacter) {
		// Do nothing
	}
}
