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
	void OnBeginFusion(CharacterFusion originalCharacter, GameObject fusingCharacter);

	/// <summary>
	/// Event called when two characters end fusing together.
	/// </summary>
	/// <param name="finalCharacter">The fused character</param>
	void OnBeginFusion(CharacterFusion finalCharacter);

	/// <summary>
	/// Event called when a character is spitted after a failed fusion.
	/// </summary>
	/// <param name="finalCharacter">The fused character</param>
	/// <param name="spittedCharacter">The spitted character</param>
	void OnSpitDrop(CharacterFusion finalCharacter, GameObject spittedCharacter);
}

/// <summary>
/// Adapter for the CharacterFusionListener interface used to
/// avoid forcing each class to implement all it's methods.
/// </summary>
public class CharacterFusionAdapter : CharacterFusionListener {

	public void OnBeginFusion(CharacterFusion originalCharacter, GameObject fusingCharacter) {
		// Do nothing
	}

	public void OnBeginFusion(CharacterFusion finalCharacter) {
		// Do nothing
	}

	public void OnSpitDrop(CharacterFusion finalCharacter, GameObject spittedCharacter) {
		// Do nothing
	}
}
