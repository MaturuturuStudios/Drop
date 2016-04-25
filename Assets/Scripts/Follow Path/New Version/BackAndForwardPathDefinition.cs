using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines the path an entity will follow using the MovingPlatformFollowPath script.
/// </summary>
public class BackAndForwardPathDefinition : SimplePathDefinition {

    #region Attributes

    /// <summary>
    /// Change direction when reaches the end
    /// </summary>
    public bool reverseAtEnd = false;

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Advance current position X steps
    /// </summary>
    /// <param name="steps">Number of steps to advance</param>
    /// <returns>Advanced position</returns>
    public override Transform MoveNext(int steps = 1) {

        steps = Mathf.Clamp(steps, 0, points.Count - 1);

        if (points.Count < 2) {
            return GetCurrent();
        }

        int target = current + steps;

        if (target >= points.Count) {
            target = ((points.Count - 1) - (target - (points.Count - 1)));
            if(!reverseAtEnd)
                return null;

        }

        current = target;

        return points[current];
    }


    /// <summary>
    /// Goes back current position X steps
    /// </summary>
    /// <param name="steps">Number of steps to go back</param>
    /// <returns>retroceded position</returns>
    public override Transform MovePrevious(int steps = 1) {

        steps = Mathf.Clamp(steps, 0, points.Count - 1);

        if (points.Count < 2) {
            return GetCurrent();
        }

        int target = current - steps;

        if (target < 0) {
            target *= -1;
            if (!reverseAtEnd)
                return null;
        }

        current = target;

        return points[current];
    }

    #endregion
}
