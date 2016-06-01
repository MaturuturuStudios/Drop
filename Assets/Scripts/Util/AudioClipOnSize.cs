using UnityEngine;
using System;

/// <summary>
/// Utility class storing an audio clip and a size.
/// Used to play different sounds at different character's
/// sizes.
/// </summary>
[Serializable]
public class AudioClipOnSize {

	/// <summary>
	/// The clip to play.
	/// </summary>
	public AudioClip clip;

	/// <summary>
	/// Size of the character.
	/// </summary>
	public int characterSize;
}
