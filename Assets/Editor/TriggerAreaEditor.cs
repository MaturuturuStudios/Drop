using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor script for the TriggerArea script.
/// </summary>
[CustomEditor(typeof(TriggerArea))]
[CanEditMultipleObjects]
public class TriggerAreaEditor : Editor {
	
	/// <summary>
	/// Draws the information on the editor.
	/// </summary>
	public override void OnInspectorGUI() {
		TriggerArea myScript = target as TriggerArea;

		EditorUtility.SetDirty(target);

		// Draws the collider filter field
		myScript.colliderFilter = (TriggerArea.ColliderFilter)EditorGUILayout.EnumPopup("Object Filter", myScript.colliderFilter);

		// Draws the trigger mode field
		myScript.triggerMode = (TriggerArea.TriggerMode) EditorGUILayout.EnumPopup("Trigger Mode", myScript.triggerMode);

		// If it's a timed switch, draws the time field
		if (myScript.triggerMode == TriggerArea.TriggerMode.TimedSwitch)
			myScript.switchTime = EditorGUILayout.FloatField("Switch Time", myScript.switchTime);

		// Draws the auto disable field
		myScript.autoDisable = EditorGUILayout.Toggle("Auto Disable", myScript.autoDisable);

		// Draws the draw gizmos field
		myScript.drawGizmos = EditorGUILayout.Toggle("Draw Gizmos", myScript.drawGizmos);

		// Draws the OnEnter methods list
		SerializedObject serializedScript = new SerializedObject(target);
		SerializedProperty onEnter = serializedScript.FindProperty("onEnter");
		EditorGUILayout.PropertyField(onEnter, true);

		// Draws the OnStay methods list, but only if it's not a swtich
		if (myScript.triggerMode == TriggerArea.TriggerMode.Sensor) {
			SerializedProperty onStay = serializedScript.FindProperty("onStay");
			EditorGUILayout.PropertyField(onStay, true);
		}

		// Draws the OnExit methods list
		SerializedProperty onExit = serializedScript.FindProperty("onExit");
		EditorGUILayout.PropertyField(onExit, true);
	}
}

