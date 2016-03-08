using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Defines the path an entity will follow using the MovingPlatformFollowPath script.
/// </summary>
public class PathDefinition : MonoBehaviour {

	#region Public Attributes

	/// <summary>
	/// Array containing the points of the path. The platform will
	/// go from one point to the next.
	/// </summary>
	public Transform[] points;

	#endregion

	#region Methods

	/// <summary>
	/// Returns an enumerator that reverses the path once it reaches
	/// it's end.
	/// </summary>
	/// <returns>A back and forward path enumerator</returns>
	public IEnumerator<Transform> GetBackAndForwardEumerator() {
		// Only one point is required
        if (points == null || points.Length < 1)
            yield break;

		bool returning = false;
        int index = 0;
        while (true) {
			// Lazy return
            yield return points[index];

			// If there is only one point, no further calculations are needed
			if (points.Length == 1)
				continue;

			if (index <= 0)
				returning = false;
			else if (index >= points.Length - 1)
				returning = true;

			index += returning ? -1 : 1;
        }
	}

	/// <summary>
	/// Returns an enumerator that starts the path from the beginning
	/// once it reaches it's end, in the same order.
	/// </summary>
	/// <returns>A loop path enumerator</returns>
	public IEnumerator<Transform> GetLoopEumerator() {
		if (points == null || points.Length < 1)
			yield break;
		
		int index = 0;
		while (true) {
			// Lazy return
			yield return points[index];

			if (index >= points.Length - 1)
				index = 0;
			else
				index++;
		}
	}

	/// <summary>
	/// Unity's method called by the editor in order to draw the gizmos.
	/// Draws the path on the editor.
	/// </summary>
	public void OnDrawGizmos() {
        // At least 2 points are needed to draw a line
        if (points == null || points.Length < 2)
            return;

        for (int i = 1; i < points.Length; i++)
            Gizmos.DrawLine(points[i - 1].position, points[i].position);
    }

	#endregion
}
