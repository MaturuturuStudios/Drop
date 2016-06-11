using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Defines a path of transforms.
/// </summary>
[Serializable]
public class PathDefinition : IEnumerator<Transform> {

	#region Custom Enumerations

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
		/// Always returns a random point in the path.
		/// </summary>
		Random
	}

	#endregion

	#region Public Attributes

	/// <summary>
	/// Array containing the points of the path.
	/// </summary>
	public Transform[] points;

	/// <summary>
	/// Defines how the path will select the next or previous point in
	/// the path.
	/// </summary>
	public PathType pathType = PathType.BackAndForward;

	#endregion

	#region Properties

	/// <summary>
	/// Returns the current element of the enumerator.
	/// </summary>
	public Transform Current {
		get {
			if (_currentIndex < 0)
				throw new InvalidOperationException("Enumerator still not initialized. Use the method MoveNext to access the first element.");
			return points[_currentIndex];
		}
	}

	/// <summary>
	/// Returns the current element of the enumerator.
	/// </summary>
	object IEnumerator.Current {
		get {
			return Current;
		}
	}

	#endregion

	#region Variables

	/// <summary>
	/// The index of the current point on the path.
	/// </summary>
	private int _currentIndex = -1;

	/// <summary>
	/// If the path type is set to BackAndForward, flag that indicates
	/// if the path is going backwards.
	/// </summary>
	private bool backwards = false;

	#endregion

	#region Methods

	/// <summary>
	/// Resets the enumerator to it's initial state.
	/// </summary>
	public void Reset() {
		_currentIndex = -1;
    }

	/// <summary>
	/// Destroys the Enumerator.
	/// </summary>
	public void Dispose() {
		throw new NotSupportedException();
	}

	/// <summary>
	/// Moves the Enumerator to it's next value. This method
	/// must be called at least once before using the Current
	/// property.
	/// </summary>
	/// <returns>If the Enumerator was able to move</returns>
	public bool MoveNext() {
		Next();
		return true;
	}

	/// <summary>
	/// Moves the path to the next point.
	/// </summary>
	public void Next() {
		switch(pathType) {
			case PathType.BackAndForward:
				if (backwards) {
					_currentIndex--;
					if (_currentIndex < 0) {
						_currentIndex = 1;
                        backwards = false;
					}
				}
				else {
					_currentIndex++;
					if (_currentIndex >= points.Length) {
						_currentIndex = points.Length - 2;
						backwards = true;
					}
				}
				break;
			case PathType.Loop:
				_currentIndex++;
				if (_currentIndex >= points.Length)
					_currentIndex = 0;
				break;
			case PathType.Random:
				Random();
				break;
			default:
				Debug.LogError("Error: Unsuported path type: " + pathType);
				break;
		}
	}

	/// <summary>
	/// Moves the path to the previous point.
	/// </summary>
	public void Previous() {
		switch (pathType) {
			case PathType.BackAndForward:
				if (!backwards) {
					_currentIndex--;
					if (_currentIndex < 0) {
						_currentIndex = 1;
						backwards = false;
					}
				}
				else {
					_currentIndex++;
					if (_currentIndex >= points.Length) {
						_currentIndex = points.Length - 2;
						backwards = true;
					}
				}
				break;
			case PathType.Loop:
				_currentIndex--;
				if (_currentIndex < 0)
					_currentIndex = points.Length - 1;
				break;
			case PathType.Random:
				Random();
				break;
			default:
				Debug.LogError("Error: Unsuported path type: " + pathType);
				break;
		}
	}

	/// <summary>
	/// Moves the path to a random point.
	/// </summary>
	public void Random() {
		_currentIndex = UnityEngine.Random.Range(0, points.Length - 1);
	}

	/// <summary>
	/// Directly modifies the current index of the path.
	/// </summary>
	/// <param name="index">The new index of the path</param>
	public void SetIndex(int index) {
		_currentIndex = index;
	}

	/// <summary>
	/// Jumps the specified steps on the path. The amount can be
	/// negative, which will make the path to move backwards. If
	/// the end is reached, it will behave as the path type would.
	/// </summary>
	/// <param name="amount">Number of steps to jump</param>
	public void Jump(int amount) {
		if (amount >= 0)
			for (int i = 0; i < amount; i++)
				Next();
		else
			for (int i = 0; i < -amount; i++)
				Previous();
	}

	/// <summary>
	/// Unity's method called by the editor in order to draw the gizmos.
	/// Draws the path on the editor. Note that since this class is not
	/// a MonoBehaviour, this method will need to be called from one.
	/// </summary>
	public void OnDrawGizmos() {
		// At least 2 points are needed to draw a line
		if (points == null || points.Length < 2)
			return;

		// Draw the lines
		for (int i = 1; i < points.Length; i++)
			if (points[i - 1] != null && points[i] != null)
				Gizmos.DrawLine(points[i - 1].position, points[i].position);
	}

	#endregion
}
