using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the path an entity will follow using the MovingPlatformFollowPath script.
/// </summary>
public abstract class SimplePathDefinition : MonoBehaviour {

    #region Public Attributes

    /// <summary>
    /// List containing the points of the path. The platform will
    /// go from one point to the next.
    /// </summary>
    public List<Transform> points;

    /// <summary>
    /// Variable for save the current position in the list.
    /// </summary>
    protected int current = 0;
    #endregion

    #region Methods

    /// <summary>
    /// Get the current index position in the chain
    /// </summary>
    /// <returns>Current index position of the chain</returns>
    public int GetCurrentIndex() {
        return current;
    }


    /// <summary>
    /// Get the transform on the current position
    /// </summary>
    /// <returns>Current position of the chain</returns>
    public Transform GetCurrent() {
        if (points.Count == 0) {
            Debug.LogError("Path definition cannot be empty!", gameObject);
            return null;
        }
        if (current < 0 || current >= points.Count)
            return null;
        return points[current];
    }


    /// <summary>
    /// Set the current position the index position and return it
    /// </summary>
    /// <returns>Last position of the chain</returns>
    public Transform MoveAt(int index) {
        if (points.Count == 0) {
            Debug.LogError("Path definition cannot be empty!", gameObject);
            return null;
        }
        if (index < 0 || index >= points.Count)
            return null;
        current = index;
        return points[index];
    }


    /// <summary>
    /// Set the current position the fisrt and return it
    /// </summary>
    /// <returns>First position of the chain</returns>
    public Transform MoveFirst() {
        if (points.Count == 0) {
            Debug.LogError("Path definition cannot be empty!", gameObject);
            return null;
        }
        current = 0;
        return points[current];
    }


    /// <summary>
    /// Set the current position the last and return it
    /// </summary>
    /// <returns>Last position of the chain</returns>
    public Transform MoveLast() {
        if (points.Count == 0) {
            Debug.LogError("Path definition cannot be empty!", gameObject);
            return null;
        }
        current = points.Count - 1;
        return points[current];
    }


    /// <summary>
    /// Unity's method called by the editor in order to draw the gizmos.
    /// Draws the path on the editor.
    /// </summary>
    public void OnDrawGizmos() {
        // At least 2 points are needed to draw a line
        if (points == null || points.Count  < 2)
            return;

        for (int i = 1; i < points.Count; i++)
			if (points[i - 1] != null && points[i] != null)
				Gizmos.DrawLine(points[i - 1].position, points[i].position);
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Advance current position X steps
    /// </summary>
    /// <param name="steps">Number of steps to advance</param>
    /// <returns>Advanced position</returns>
    public abstract Transform MoveNext(int steps = 1);


    /// <summary>
    /// Goes back current position X steps
    /// </summary>
    /// <param name="steps">Number of steps to go back</param>
    /// <returns>retroceded position</returns>
    public abstract Transform MovePrevious(int steps = 1);

    #endregion
}
