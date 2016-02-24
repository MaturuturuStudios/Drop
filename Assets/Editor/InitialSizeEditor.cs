using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterSize))]
[CanEditMultipleObjects]
public class InitialSizeEditor : Editor {
    public void OnSceneGUI() {

        CharacterSize characterSize = (target as CharacterSize);

        Handles.color = Color.magenta;
        EditorGUI.BeginChangeCheck();

        Vector3 positionCharacter = characterSize.transform.position;
        float scaleHandle = HandleUtility.GetHandleSize(positionCharacter);
        float scale = Handles.ScaleSlider(characterSize.initialSize, positionCharacter, 
                                        Vector3.left, Quaternion.identity, scaleHandle, 0.1f);
        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Scale Value");

            int finalSize = (int)scale;
            if(finalSize <= 0) {
                finalSize = 1;
            }

            characterSize.initialSize = finalSize;
            characterSize.transform.localScale = Vector3.one * finalSize;
        }
    }
}