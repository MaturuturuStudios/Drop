using UnityEditor;
using System;

/// <summary>
/// Editor script for the TriggerArea script.
/// </summary>
[CustomEditor(typeof(TriggerArea))]
[CanEditMultipleObjects]
public class TriggerAreaEditor : Editor {

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

		// Draws the collider filter field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("colliderFilter"), true);

		// Draws the trigger mode field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("triggerMode"), true);
		TriggerArea.TriggerMode triggerMode = (TriggerArea.TriggerMode)Enum.GetValues(typeof(TriggerArea.TriggerMode)).GetValue(_serializedScript.FindProperty("triggerMode").enumValueIndex);

		// If it's a switch, draws the active toggle field
		if (triggerMode == TriggerArea.TriggerMode.Switch || triggerMode == TriggerArea.TriggerMode.TimedSwitch)
			EditorGUILayout.PropertyField(_serializedScript.FindProperty("switchActive"), true);

		// If it's a timed switch, draws the time field
		if (triggerMode == TriggerArea.TriggerMode.TimedSwitch)
			EditorGUILayout.PropertyField(_serializedScript.FindProperty("switchTime"), true);

		// Draws the auto disable field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("autoDisable"), true);

		// Draws the draw gizmos field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("drawGizmos"), true);

		// Draws the OnEnter methods list
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("onEnter"), true);

		// Draws the OnStay methods list, but only if it's not a swtich
		if (triggerMode == TriggerArea.TriggerMode.Sensor)
			EditorGUILayout.PropertyField(_serializedScript.FindProperty("onStay"), true);

		// Draws the OnExit methods list
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("onExit"), true);

		// Applys the changes to the serialized object
		_serializedScript.ApplyModifiedProperties();
	}
}

