using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour {

}

/// <summary>
/// Defines how will the entity move to the next point in the path.
/// </summary>
public enum FollowType {

    /// <summary>
    /// Moves towards the points at constant speed.
    /// </summary>
    MoveTowards,

    /// <summary>
    /// Moves towards the points at decreasing speed.
    /// </summary>
    Lerp
}

/// <summary>
/// Defines how the entity looks for the next point in the path.
/// </summary>
public enum PathType {

    /// <summary>
    /// After reaching the end, returns using the reverse path.
    /// </summary>
    BackAndForward,

    /// <summary>
    /// After reaching the end, returns to the first point in the path
    /// and starts again in the same order.
    /// </summary>
    Loop,
    /// <summary>
    /// Start and continue to random points
    /// </summary>
    Random
}