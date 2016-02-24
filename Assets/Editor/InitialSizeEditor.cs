using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterSize))]
[CanEditMultipleObjects]
public class InitialSizeEditor : Editor {
    public void OnSceneGUI() {
        //get the character size of the game object if have one
        CharacterSize characterSize = (target as CharacterSize);

        //set color of handle
        Handles.color = Color.magenta;

        EditorGUI.BeginChangeCheck();
        Vector3 positionCharacter = characterSize.transform.position;
        //get an adecuate scale for the handle
        float scaleHandle = HandleUtility.GetHandleSize(positionCharacter);
        //get the change value of scale when sliding the control
        //it asociated to the initial size of the character
        float scale = Handles.ScaleSlider(characterSize.initialSize, positionCharacter, 
                                        Vector3.left, Quaternion.identity, scaleHandle, 0.1f);

        if(EditorGUI.EndChangeCheck()) {
            //set the changes
            Undo.RecordObject(target, "Scale Value");

            //truncate the value
            int finalSize = (int)scale;
            if(finalSize <= 0) {
                finalSize = 1;
            }
            //set both initial size and scale size of character
            characterSize.initialSize = finalSize;
            characterSize.transform.localScale = Vector3.one * finalSize;
        }
    }
}