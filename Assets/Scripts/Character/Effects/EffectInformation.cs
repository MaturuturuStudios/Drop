using UnityEngine;
using System;

/// <summary>
/// Constains the information from an effect which will be
/// played by the the character.
/// </summary>
[Serializable]
public class EffectInformation {

	/// <summary>
	/// The output mixer for the sound.
	/// </summary>
	public GameObject effectPrefab;

	/// <summary>
	/// Time for the effect to be destroyed.
	/// </summary>
	public float duration = 1.5f;

	/// <summary>
	/// If enabled, the effect will be scaled with the
	/// character's size.
	/// </summary>
	public bool scaleWithSize = true;

	/// <summary>
	/// If enabled, the square root of the size will be used
	/// for the effect's scale.
	/// </summary>
	public bool rootedScale = false;

	/// <summary>
	/// Plays the effect.
	/// </summary>
	/// <param name="position">The position of the effect</param>
	/// <param name="rotation">The rotation of the effect</param>
	/// <param name="scale">The scale the effect should be played with (optional)</param
	/// <returns>The created effect</returns>
	public GameObject PlayEffect(Vector3 position, Quaternion rotation, float scale = 1.0f) {
		GameObject effect = UnityEngine.Object.Instantiate(effectPrefab, position, rotation) as GameObject;
		if (scaleWithSize) {
			if (rootedScale)
				scale = Mathf.Sqrt(scale);
			effect.transform.localScale = Vector3.one * scale;
		}
		UnityEngine.Object.Destroy(effect, duration);
		return effect;
	}
}

/// <summary>
/// Extension class for EffectInformation including some extra fields.
/// </summary>
[Serializable]
public class MinSpeedEffectInformation : EffectInformation {

	/// <summary>
	/// The minimum speed the character should have to play the effect.
	/// </summary>
	public float minSpeed = 3.0f;

	/// <summary>
	/// If enabled, the minimum speed will be scaled with the
	/// character's size.
	/// </summary>
	public bool scaleSpeed = true;

	/// <summary>
	/// If enabled, the square root of the size will be used
	/// for the speed's scale.
	/// </summary>
	public bool speedRootScale = false;
	
	/// <summary>
	/// Returns the minimum speed for the selected scale.
	/// </summary>
	/// <param name="scale">The scale which speed should be calculated</param>
	/// <returns>The minimum speed for the selected scale</returns>
	public float GetMinSpeed(float scale) {
		if (!scaleSpeed)
			return minSpeed;
		else
			return minSpeed * (speedRootScale ? Mathf.Sqrt(scale) : scale);
	}
}