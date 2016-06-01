using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor script for the JumpMushroom script.
/// </summary>
[CustomEditor(typeof(JumpMushroom))]
[CanEditMultipleObjects]
public class JumpMushroomEditor : Editor {
	
	/// <summary>
	/// Draws the information on the editor.
	/// </summary>
	public override void OnInspectorGUI() {
		JumpMushroom myScript = target as JumpMushroom;

		EditorUtility.SetDirty(target);

		myScript.minHeight = EditorGUILayout.FloatField("Min Height", myScript.minHeight);
		myScript.maxHeight = EditorGUILayout.FloatField("Max Height", myScript.maxHeight);

		myScript.jumpFactor = EditorGUILayout.FloatField("Jump Compensation Factor", myScript.jumpFactor);

		myScript.keepPerpendicularSpeed = EditorGUILayout.Toggle("Keep Speed", myScript.keepPerpendicularSpeed);

		myScript.lostControl = EditorGUILayout.Toggle("Lost Control", myScript.lostControl);

		// If Lost Control is enabled, shows the Temporary option
		if (myScript.lostControl)
			myScript.temporary = EditorGUILayout.Toggle("Lost Control Temporary", myScript.temporary);

		// If Temporary is enabled, shows the Time field
		if (myScript.lostControl && myScript.temporary)
			myScript.time = EditorGUILayout.FloatField("Time", myScript.time);
		else
			myScript.time = 0.0f;
	}
}

