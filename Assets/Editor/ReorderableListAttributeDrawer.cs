using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Custom drawer for the ReorderableListAttributeClass.
/// Creates a list where adding, deleting, and sorting
/// elements is easier than regular lists.
/// </summary>
[CustomPropertyDrawer(typeof(ReorderableListAttribute), true)]
public class ReorderableListDrawer : PropertyDrawer {

	/// <summary>
	/// Reference to Unity's ReorderableList component.
	/// </summary>
	private ReorderableList _list;
	
	/// <summary>
	/// Returns the ReorderableList or creates it if it
	/// wasn't created before.
	/// </summary>
	/// <param name="property">The property to store in the list</param>
	/// <returns>The ReorderableList component</returns>
	private ReorderableList GetList(SerializedProperty property) {
		if (_list == null) {
			// If it doesn't exists, creates it
			SerializedProperty listProperty = property.FindPropertyRelative("list");
			_list = new ReorderableList(listProperty.serializedObject, listProperty, true, true, true, true);

			// Tells the list to draw it's elements
			_list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				EditorGUI.PropertyField(rect, listProperty.GetArrayElementAtIndex(index), true);
			};

			// Changes the header of the list
			_list.drawHeaderCallback = rect => {
				EditorGUI.LabelField(rect, ObjectNames.NicifyVariableName(property.name));
			};
		}
		return _list;
	}

	/// <summary>
	/// Overrides the height method of the element to return
	/// the height of the ReorderableList component.
	/// </summary>
	/// <param name="property">The property of the drawer</param>
	/// <param name="label">The content of the element</param>
	/// <returns></returns>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return GetList(property).GetHeight();
	}
	
	/// <summary>
	/// Unity's method called to draw the element on the editor.
	/// </summary>
	/// <param name="position">The space occupied by the element</param>
	/// <param name="property">The property of the drawer</param>
	/// <param name="label">The content of the element</param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
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
