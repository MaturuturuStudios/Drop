using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom drawer used to properly represent the information
/// and functionality of a AudioClipOnSize on the editor.
/// </summary>
[CustomPropertyDrawer(typeof(AudioClipOnSize))]
class AudioClipOnSizeDrawer : PropertyDrawer {

	/// <summary>
	/// Overrides the height method of the element to return
	/// the height of the element.
	/// </summary>
	/// <param name="property">The property of the drawer</param>
	/// <param name="label">The content of the element</param>
	/// <returns></returns>
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		// Returns a fixed height, since the space occupied by this element is fixed as well
		return 20;
	}

	/// <summary>
	/// Unity's method called to draw the element on the editor.
	/// </summary>
	/// <param name="position">The space occupied by the element</param>
	/// <param name="property">The property of the drawer</param>
	/// <param name="label">The content of the element</param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		// Draws the size selector
		Rect sizeRect = position;
		sizeRect.width = 240;
		sizeRect.height = 18;
		property.FindPropertyRelative("characterSize").intValue = EditorGUI.IntField(sizeRect, "Min Size To Play", property.FindPropertyRelative("characterSize").intValue);

		// Draws the audio clip selector
		Rect clipRect = position;
		clipRect.x += 245;
		clipRect.width -= 245;
		clipRect.height = 18;
		property.FindPropertyRelative("clip").objectReferenceValue = (AudioClip)EditorGUI.ObjectField(clipRect, property.FindPropertyRelative("clip").objectReferenceValue, typeof(AudioClip), true);
	}
}
