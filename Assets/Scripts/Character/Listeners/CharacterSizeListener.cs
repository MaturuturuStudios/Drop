using UnityEngine;

/// <summary>
/// Interface for the listeners to size changes.
/// </summary>
public interface CharacterSizeListener {

	/// <summary>
	/// Event called when the character starts changing it's size.
	/// Called each frame prior to the scale change.
	/// </summary>
	/// <param name="character">The character changing size.</param>
	/// <param name="previousScale">The scale prior to the scale change.</param>
	/// <param name="nextScale">The scale after the scale change.</param>
	void OnChangeSizeStart(GameObject character, Vector3 previousScale, Vector3 nextScale);

	/// <summary>
	/// Event called when the character stops changing it's size.
	/// Called each frame after the scale change.
	/// </summary>
	/// <param name="character">The character changing size.</param>
	/// <param name="previousScale">The scale prior to the scale change.</param>
	/// <param name="nextScale">The scale after the scale change.</param>
	void OnChangeSizeEnd(GameObject character, Vector3 previousScale, Vector3 nextScale);

	/// <summary>
	/// Event called when a character is spitted.
	/// </summary>
	/// <param name="character">The main character</param>
	/// <param name="spittedCharacter">The spitted character</param>
	void OnSpitDrop(GameObject character, GameObject spittedCharacter);
}

/// <summary>
/// Adapter for the CharacterSizeListener interface used to
/// avoid forcing each class to implement all it's methods.
/// </summary>
public class CharacterSizeAdapter : MonoBehaviour, CharacterSizeListener {

	public virtual void OnChangeSizeStart(GameObject character, Vector3 previousScale, Vector3 nextScale) {
		// Do nothing
	}

	public virtual void OnChangeSizeEnd(GameObject character, Vector3 previousScale, Vector3 nextScale) {
		// Do nothing
	}

	public virtual void OnSpitDrop(GameObject character, GameObject spittedCharacter) {
		// Do nothing
	}
}

