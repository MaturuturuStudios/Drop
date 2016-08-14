using UnityEngine;
using UnityEditor;

/// <summary>
/// The MIT License (MIT)
/// Copyright (c) 2015 Kuutti Entertainment Ltd
/// </summary>
public class PlayerPrefsEditorUtility {

    [MenuItem("PlayerPrefs/Delete All")]
    static void DeletePlayerPrefs() {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted");
    }

    [MenuItem("PlayerPrefs/Save All")]
    static void SavePlayerPrefs() {
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs saved");
    }
}
