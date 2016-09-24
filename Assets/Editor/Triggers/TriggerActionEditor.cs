using UnityEditor;
using System;

/// <summary>
/// Editor script for the TriggerAction script.
/// </summary>
[CustomEditor(typeof(TriggerAction))]
[CanEditMultipleObjects]
public class TriggerActionEditor : Editor {

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

		// Draws the trigger mode field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("triggerMode"), true);
		TriggerAction.TriggerMode triggerMode = (TriggerAction.TriggerMode)Enum.GetValues(typeof(TriggerAction.TriggerMode)).GetValue(_serializedScript.FindProperty("triggerMode").enumValueIndex);

		// Draws the delay field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("delayBetweenUses"), true);

		// If it's a switch, draws the active toggle field
		if (triggerMode == TriggerAction.TriggerMode.Switch || triggerMode == TriggerAction.TriggerMode.TimedSwitch)
			EditorGUILayout.PropertyField(_serializedScript.FindProperty("switchActive"), true);

		// If it's a timed switch, draws the time field
		if (triggerMode == TriggerAction.TriggerMode.TimedSwitch)
			EditorGUILayout.PropertyField(_serializedScript.FindProperty("autoSwitchTime"), true);

		// Draws the auto disable field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("autoDisable"), true);

		// Draws the only once per frame field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("onlyOncePerFrame"), true);

		// Draws the draw gizmos field
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("drawGizmos"), true);

		// Draws the OnActivate methods list
		EditorGUILayout.PropertyField(_serializedScript.FindProperty("onActivate"), true);
		
		// Draws the OnDeactivate methods list, but only if it's a swtich
		if (triggerMode == TriggerAction.TriggerMode.Switch || triggerMode == TriggerAction.TriggerMode.TimedSwitch)
			EditorGUILayout.PropertyField(_serializedScript.FindProperty("onDeactivate"), true);

		// Applys the changes to the serialized object
		_serializedScript.ApplyModifiedProperties();
	}
}

