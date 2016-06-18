using UnityEngine;

/// <summary>
/// Listenre for the WindTube class events.
/// </summary>
public interface WindTubeListener {

	/// <summary>
	/// Event called when an object enters a wind tube.
	/// </summary>
	/// <param name="windTube">The wind tube object</param>
	/// <param name="character">The object entering the wind tube</param>
	void OnWindTubeEnter(WindTube windTube, GameObject character);

	/// <summary>
	/// Event called when an object stays on a wind tube and force is applied to it.
	/// </summary>
	/// <param name="windTube">The wind tube object</param>
	/// <param name="character">The object entering the wind tube</param>
	/// <param name="forceApplied">The force applied to the object by the wind tube</param>
	void OnWindTubeStay(WindTube windTube, GameObject character, Vector3 forceApplied);


	/// <summary>
	/// Event called when an object exits a wind tube.
	/// </summary>
	/// <param name="windTube">The wind tube object</param>
	/// <param name="character">The object exiting the wind tube</param>
	void OnWindTubeExit(WindTube windTube, GameObject character);
}

/// <summary>
/// Adapter for the WindTubeListener interface used to
/// avoid forcing each class to implement all it's methods.
/// </summary>
public class WindTubeAdapter : MonoBehaviour, WindTubeListener {

	public virtual void OnWindTubeEnter(WindTube windTube, GameObject character) {
		// Do nothing
	}

	public virtual void OnWindTubeStay(WindTube windTube, GameObject character, Vector3 forceApplied) {
		// Do nothing
	}

	public virtual void OnWindTubeExit(WindTube windTube, GameObject character) {
		// Do nothing
	}
}