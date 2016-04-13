using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer(typeof(MethodInvoke))]
class MethodInvokeDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		Rect targetRect = position;
		targetRect.width = 120;
		GameObject target = (GameObject)EditorGUI.ObjectField(targetRect, property.FindPropertyRelative("target").objectReferenceValue, typeof(GameObject), true);
		if (target != property.FindPropertyRelative("target").objectReferenceValue) {
			property.FindPropertyRelative("target").objectReferenceValue = target;
			property.FindPropertyRelative("selectedIndex").intValue = 0;
		}

		if (target != null) {
			Rect popupRect = position;
			popupRect.x += targetRect.width;
			popupRect.width = position.width - targetRect.width;
			List<MethodInfo> methods = MethodInvoke.GetMethodsInfo(target);
			int selectedIndex = EditorGUI.Popup(popupRect, property.FindPropertyRelative("selectedIndex").intValue, methods.Select((e) => e.DeclaringType + "/" + e.Name).ToArray());
			property.FindPropertyRelative("selectedIndex").intValue = selectedIndex;
		}
	}
}
