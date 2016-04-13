using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReorderableListAttribute), true)]
public class ReorderableListDrawer : PropertyDrawer {

	private ReorderableList _list;
	
	private ReorderableList GetList(SerializedProperty property) {
		if (_list == null) {
			SerializedProperty listProperty = property.FindPropertyRelative("list");
			_list = new ReorderableList(listProperty.serializedObject, listProperty, true, true, true, true);
			_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				EditorGUI.PropertyField(rect, listProperty.GetArrayElementAtIndex(index), true);
			};
			_list.drawHeaderCallback = rect => {
				EditorGUI.LabelField(rect, ObjectNames.NicifyVariableName(property.name));
			};
		}
		return _list;
	}


	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return GetList(property).GetHeight();
	}


	public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, GUIContent label) {
		ReorderableList list = GetList(property);
		SerializedProperty listProperty = property.FindPropertyRelative("list");
		float height = 0f;
		for (int i = 0; i < listProperty.arraySize; i++) {
			height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
		}
		list.elementHeight = height;
		list.DoList(position);
	}
}
