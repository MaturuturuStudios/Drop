using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor script for the FollowPath script.
/// </summary>
[CustomEditor(typeof(FollowPath))]
public class FollowPathEditor : Editor {
	
	/// <summary>
	/// Draws the information on the editor.
	/// </summary>
	public override void OnInspectorGUI() {
		FollowPath followPath = target as FollowPath;
		EditorUtility.SetDirty(target);

		// Shows the path information
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("path"), new GUIContent("Path"), true);
		serializedObject.ApplyModifiedProperties();

		// Shows the movement information
		followPath.movementType = (FollowPath.MovementType)EditorGUILayout.EnumPopup("Movement Type", followPath.movementType);
		switch (followPath.movementType) {
			case FollowPath.MovementType.SPEED:
				followPath.speed = EditorGUILayout.FloatField("Speed", followPath.speed);
				break;
			case FollowPath.MovementType.DELAY:
				followPath.delay = EditorGUILayout.FloatField("Delay", followPath.delay);
				break;
			default:
				Debug.LogError("Error: Invalid MovementType: " + followPath.movementType);
				return;
		}

		// Shows the smooth toggle
		followPath.smooth = EditorGUILayout.Toggle("Smooth", followPath.smooth);

		// Shows the automatic toggle
		followPath.automatic = EditorGUILayout.Toggle("Automatic", followPath.automatic);

		// Shows the orientation toggle
		followPath.useOrientation = EditorGUILayout.Toggle("Use Orientation", followPath.useOrientation);
    }
}

