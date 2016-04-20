using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PathDefinition))]
public class PathDefinitionDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        SerializedProperty list = property.FindPropertyRelative("points");
        EditorGUI.PropertyField(position, list, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        SerializedProperty list = property.FindPropertyRelative("points");
        int sizeArray = list.arraySize;
        
        float height = EditorGUIUtility.singleLineHeight;
        if (list.isExpanded) {
            height += EditorGUIUtility.singleLineHeight;

            if (sizeArray > 0) {
                height += EditorGUIUtility.singleLineHeight * (sizeArray);
                height += EditorGUIUtility.standardVerticalSpacing * sizeArray + 1;
            }
        }
        return height;
    }


}
