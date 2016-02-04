using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MovingPlatformPathDefinition : MonoBehaviour {

	// Public atributes
	public Transform[] points;

    public IEnumerator<Transform> getBackAndForwardEumerator() {
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

	public IEnumerator<Transform> getLoopEumerator() {
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

	public void OnDrawGizmos() {
        // At least 2 points are needed to draw a line
        if (points == null || points.Length < 2)
            return;

        for (int i = 1; i < points.Length; i++)
            Gizmos.DrawLine(points[i - 1].position, points[i].position);
    }
}
