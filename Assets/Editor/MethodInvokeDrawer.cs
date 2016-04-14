using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// Custom drawer used to properly represent the information
/// and functionality of a MethodInvoke on the editor.
/// </summary>
[CustomPropertyDrawer(typeof(MethodInvoke))]
class MethodInvokeDrawer : PropertyDrawer {

	/// <summary>
	/// Overrides the height method of the element to return
	/// the height of the element.
	/// </summary>
	/// <param name="property">The property of the drawer</param>
	/// <param name="label">The content of the element</param>
	/// <returns></returns>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		// Returns a fixed height, since the space occupied by this element is fixed as well
		return 40;
	}

	/// <summary>
	/// Unity's method called to draw the element on the editor.
	/// </summary>
	/// <param name="position">The space occupied by the element</param>
	/// <param name="property">The property of the drawer</param>
	/// <param name="label">The content of the element</param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		// Draws the target selector field
		Rect targetRect = position;
		targetRect.width = 120;
		targetRect.height = 18;
		GameObject target = (GameObject)EditorGUI.ObjectField(targetRect, property.FindPropertyRelative("target").objectReferenceValue, typeof(GameObject), true);
		if (target != property.FindPropertyRelative("target").objectReferenceValue) {
			property.FindPropertyRelative("target").objectReferenceValue = target;
			property.FindPropertyRelative("selectedIndex").intValue = 0;
		}

		if (target != null) {

			// Draws the method selector field
			Rect popupRect = position;
			popupRect.y += 22;
			popupRect.width = 120;
			popupRect.height = 18;
			List<MethodInfo> methods = MethodInvoke.GetMethodsInfo(target);
			int selectedIndex = EditorGUI.Popup(popupRect, property.FindPropertyRelative("selectedIndex").intValue, methods.Select((e) => e.DeclaringType + "/" + e.Name).ToArray());
			property.FindPropertyRelative("selectedIndex").intValue = selectedIndex;

			// Checks for the number of parameters of the method and draws the parameter field if necessary
			MethodInfo selectedMethod = methods[selectedIndex];
			ParameterInfo[] parameters = selectedMethod.GetParameters();
			if (parameters.Length == 1) {
				Rect parameterRect = position;
				parameterRect.x += 125;
				parameterRect.width -= 125;
				parameterRect.height = 38;
				EditorGUIUtility.labelWidth = GUI.skin.box.CalcSize(new GUIContent(parameters[0].Name)).x;
				FieldByType(parameterRect, parameters[0].ParameterType, property, parameters[0].Name);
			}
			else if (parameters.Length > 1)
				Debug.LogWarning("Warning: The selected method has more than one parameter: " + selectedMethod.Name);
		}
	}

	/// <summary>
	/// Draws the right field used by the selected type on the
	/// given space.
	/// </summary>
	/// <param name="position">The space the field will occupy</param>
	/// <param name="type">The type of the field</param>
	/// <param name="property">The property stored on the field</param>
	/// <param name="label">The label shown with the field</param>
	private void FieldByType(Rect position, Type type, SerializedProperty property, string label = null) {
		property = property.FindPropertyRelative(type.Name + "Parameter");
		if (type == typeof(Int32))
			property.intValue = EditorGUI.IntField(position, label, property.intValue);
		else if (type == typeof(Int64))
			property.longValue = EditorGUI.LongField(position, label, property.longValue);
		else if (type == typeof(Single))
			property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
		else if (type == typeof(Double))
			property.doubleValue = EditorGUI.DoubleField(position, label, property.doubleValue);
		else if (type == typeof(Boolean))
			property.boolValue = EditorGUI.Toggle(position, label, property.boolValue);
		else if (type == typeof(String))
			property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
		else if (type == typeof(Color))
			property.colorValue = EditorGUI.ColorField(position, label, property.colorValue);
		else if (type == typeof(Rect))
			property.rectValue = EditorGUI.RectField(position, label, property.rectValue);
		else if (type == typeof(Bounds))
			property.boundsValue = EditorGUI.BoundsField(position, property.boundsValue);
		else if (type == typeof(Vector2))
			property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
		else if (type == typeof(Vector3))
			property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
		else if (type == typeof(Vector4))
			property.vector4Value = EditorGUI.Vector4Field(position, label, property.vector4Value);
		else if (type == typeof(AnimationCurve))
			property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue);
		else if (type == typeof(UnityEngine.Object))
			property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, type, true);
		else
			Debug.LogWarning("Warning: Unsupported parameter type: " + type.Name);
	}
}
