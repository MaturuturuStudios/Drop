using UnityEngine;

/// <summary>
/// Listenre for the WaterRepulsionListener class events.
/// </summary>
public interface WaterRepulsionListener {

	/// <summary>
	/// Event called when a character falls into the water.
	/// </summary>
	/// <param name="irrigated">The water object</param>
	/// <param name="irrigating">The character entering the water</param>
	void OnWaterEnter(WaterRepulsion water, GameObject character);


	/// <summary>
	/// Event called when a character is thrown out of the water.
	/// </summary>
	/// <param name="irrigated">The water object</param>
	/// <param name="irrigating">The character being thrown out</param>
	/// <param name="repulsionVelocity">The velocity which the character is thrown out</param>
	void OnWaterExit(WaterRepulsion water, GameObject character, Vector3 repulsionVelocity);
}

/// <summary>
/// Adapter for the WaterRepulsionListener interface used to
/// avoid forcing each class to implement all it's methods.
/// </summary>
public class WaterRepulsionAdapter : MonoBehaviour, WaterRepulsionListener {

	public virtual void OnWaterEnter(WaterRepulsion water, GameObject character) {
		// Do nothing
	}

	public virtual void OnWaterExit(WaterRepulsion water, GameObject character, Vector3 repulsionVelocity) {
		// Do nothing
	}
}