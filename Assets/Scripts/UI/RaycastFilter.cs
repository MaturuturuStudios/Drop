using UnityEngine;

/// <summary>
/// Makes a GUI component ignore the raycasts casted
/// from the cursor.
/// </summary>
public class RaycastFilter : MonoBehaviour, ICanvasRaycastFilter {

	/// <summary>
	/// Unity's method which determines if the object
	/// is a valid raycast target.
	/// </summary>
	/// <param name="sp">The screen position of the hit</param>
	/// <param name="eventCamera">The event that fired the raycast</param>
	/// <returns></returns>
	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {
		return !enabled;
	}
}
