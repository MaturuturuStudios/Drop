using UnityEngine;

/// <summary>
/// Listenre for the Irrigate class events.
/// </summary>
public interface IrrigateListener {

	/// <summary>
	/// Event called when an object is irrigated.
	/// </summary>
	/// <param name="irrigated">The irrigated object</param>
	/// <param name="irrigating">The object irrigating the first one</param>
	/// <param name="dropsConsumed">Amount of drops consumed on the irrigation</param>
	void OnIrrigate(Irrigate irrigated, GameObject irrigating, int dropsConsumed);
}