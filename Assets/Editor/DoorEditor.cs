using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor script for the Door script.
/// </summary>
[CustomEditor(typeof(Door))]
public class DoorEditor : Editor {
	
	/// <summary>
	/// Draws the information on the editor.
	/// </summary>
	public override void OnInspectorGUI() {
		Door door = target as Door;
		EditorUtility.SetDirty(target);

		// Initializes the array of points
		if (door.path.points.Length != 2)
			door.path.points = new Transform[2];

		// Shows the two positions of the door
		door.path.points[0] = EditorGUILayout.ObjectField("Closed Position", door.path.points[0], typeof(Transform), true) as Transform;
		door.path.points[1] = EditorGUILayout.ObjectField("Opened Position", door.path.points[1], typeof(Transform), true) as Transform;

		// Shows the initial state of the door
		door.initialState = (Door.DoorState) EditorGUILayout.EnumPopup("Initial State", door.initialState);

		// Shows the information about the FollowPath script
		door.movementType = (FollowPath.MovementType)EditorGUILayout.EnumPopup("Movement Type", door.movementType);
		switch (door.movementType) {
			case FollowPath.MovementType.SPEED:
				door.speed = EditorGUILayout.FloatField("Speed", door.speed);
				break;
			case FollowPath.MovementType.DELAY:
				door.delay = EditorGUILayout.FloatField("Delay", door.delay);
				break;
			default:
				Debug.LogError("Error: Invalid MovementType: " + door.movementType);
				return;
		}
		door.smooth = EditorGUILayout.Toggle("Smooth", door.smooth);
		door.useOrientation = EditorGUILayout.Toggle("Use Orientation", door.useOrientation);
	}
}

