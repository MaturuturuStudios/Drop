using UnityEngine;

/// <summary>
/// Listenre for the CannonShoot class events.
/// </summary>
public interface CannonShootListener {

	/// <summary>
	/// Event called when an object is fired by the cannon.
	/// </summary>
	/// <param name="cannon">The cannon which is shooting the object</param>
	/// <param name="objectShot">The object shot by the cannon</param>
	/// <param name="velocity">Velocity the object is fired with</param>
	void OnCannonShoot(CannonShoot cannon, GameObject objectShot, Vector3 velocity);
}