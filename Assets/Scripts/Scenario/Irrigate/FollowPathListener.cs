using UnityEngine;

/// <summary>
/// Interface for the FollowPath script listeners.
/// </summary>
public interface FollowPathListener {

	/// <summary>
	/// Called when the object starts moving after being still
	/// or reaching a path point.
	/// </summary>
	/// <param name="position">The position of the object following the path</param>
	/// <param name="destination">The position of the next path destination point</param>
	/// <param name="velocity">The velocity the object is moving towards the desintation</param>
	void OnStartMoving(Vector3 position, Vector3 destination, Vector3 velocity);

	/// <summary>
	/// Called while the object keeps moving after along the path.
	/// </summary>
	/// <param name="position">The position of the object following the path</param>
	/// <param name="destination">The position of the next path destination point</param>
	/// <param name="velocity">The velocity the object is moving towards the desintation</param>
	void OnKeepMoving(Vector3 position, Vector3 destination, Vector3 velocity);

	/// <summary>
	/// Called when the object stops moving or reaches a path point.
	/// </summary>
	/// <param name="position">The position of the object following the path</param>
	void OnStopMoving(Vector3 position);
}
