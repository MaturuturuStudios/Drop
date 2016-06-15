#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AxisBoolean))]
public class AxisBooleanDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        //get data
        SerializedProperty axisX = property.FindPropertyRelative("axisX");
        SerializedProperty axisY = property.FindPropertyRelative("axisY");
        SerializedProperty axisZ = property.FindPropertyRelative("axisZ");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        
        float width = position.width;
        width /= 4.0f;
        position.Set(position.x, position.y, width, position.height);
        

        GUIContent labelAxis = new GUIContent("X");
        EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(labelAxis).x*3;
        axisX.boolValue = EditorGUI.Toggle(position, labelAxis, axisX.boolValue);
        position.x += width;

        labelAxis = new GUIContent("Y");
        axisY.boolValue = EditorGUI.Toggle(position, labelAxis, axisY.boolValue);
        position.x += width;

        labelAxis = new GUIContent("Z");
        axisZ.boolValue = EditorGUI.Toggle(position, labelAxis, axisZ.boolValue);
        

        EditorGUI.EndProperty();
    }
}
#endif