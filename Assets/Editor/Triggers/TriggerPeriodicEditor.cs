using UnityEditor;
using System;

/// <summary>
/// Editor script for the TriggerPeriodic script.
/// </summary>
[CustomEditor(typeof(TriggerPeriodic))]
[CanEditMultipleObjects]
public class TriggerPeriodicEditor : Editor {

	/// <summary>
	/// The saved serialized property.
	/// </summary>
	private SerializedObject _serializedScript;

	/// <summary>
	/// Unity's method called when the component is enabled.
	/// </summary>
	public void OnEnable() {
		_serializedScript = new SerializedObject(target);
	}
	
	/// <summary>
	/// Draws the information on the editor.
	/// </summary>
	public override void OnInspectorGUI() {

		// Draws the selection mode field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("selectionMode"), true);

		// Draws the events list
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("events"), true);

		// Draws the current event index field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("currentEventIndex"), true);

		// Draws the delay mode field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("delayMode"), true);
		TriggerPeriodic.DelayMode delayMode = (TriggerPeriodic.DelayMode)Enum.GetValues(typeof(TriggerPeriodic.DelayMode)).GetValue(_serializedScript.FindProperty("delayMode").enumValueIndex);

		// If it's set to same, draws the common delay field
		if (delayMode == TriggerPeriodic.DelayMode.Same)
			EditorGUILayout.PropertyField(_serializedScript.FindProperty("commonDelay"), true);
		// If it's set to specific, draws the specific delays field
		else if (delayMode == TriggerPeriodic.DelayMode.Specific)
			EditorGUILayout.PropertyField(_serializedScript.FindProperty("specificDelays"), true);

		// Draws the draw gizmos field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("drawGizmos"), true);

		// Applys the changes to the serialized object
		_serializedScript.ApplyModifiedProperties();
	}
}

