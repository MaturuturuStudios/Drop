using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor script for the CharacterSpawner script.
/// </summary>
[CustomEditor(typeof(CharacterSpawner))]
[CanEditMultipleObjects]
public class CharacterSpawnerEditor : Editor {
	
	/// <summary>
	/// Draws the information on the editor.
	/// </summary>
	public override void OnInspectorGUI() {
        CharacterSpawner myScript = target as CharacterSpawner;

		EditorUtility.SetDirty(target);

        myScript.addToControlList = EditorGUILayout.Toggle("Add to control list", myScript.addToControlList);

        if (myScript.addToControlList) {
            myScript.controlled = EditorGUILayout.Toggle("Controlled", myScript.controlled);
        }


		myScript.size = EditorGUILayout.IntSlider("Character size", myScript.size, 1, 20);

		myScript.height = EditorGUILayout.FloatField("Height elevation", myScript.height);

        int controlledCharacters = ControlledCharacters();


        // Check for invalid values
        if (controlledCharacters == 0) {
            Debug.LogWarning("There isn't any character setted as controlled, setting actual as controlled.");

            myScript.controlled = true;
        }

        if (controlledCharacters > 1) {
            Debug.LogWarning("There are more than one character setted as controlled, setting actual as controlled.");

            // Mark all drops as not controlled
            RemoveAllCharactersControl();

            // Control this
            myScript.controlled = true;
        }
    }



    /// <summary>
    /// Checks the number of characters setted as controlled
    /// </summary>
    /// <returns>The number of characters setted as controlled</returns>
    private int ControlledCharacters() {

        int found = 0;

        // Look for characters setted as controlled
        CharacterSpawner[] spawners = (CharacterSpawner[])FindObjectsOfType(typeof(CharacterSpawner));
        for (int i = 0; i < spawners.Length; ++i)
            if (spawners[i].controlled == true) ++found;

        return found;
    }



    /// <summary>
    /// Remove the control from all the spawns
    /// </summary>
    private void RemoveAllCharactersControl() {

        // Look for all the spawns
        CharacterSpawner[] spawners = (CharacterSpawner[])FindObjectsOfType(typeof(CharacterSpawner));
        for (int i = 0; i < spawners.Length; ++i)
            spawners[i].controlled = false;

    }


}

