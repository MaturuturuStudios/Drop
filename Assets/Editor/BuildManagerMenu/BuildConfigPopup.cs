using UnityEngine;
using UnityEditor;
using System;

public class BuildConfigPopup : EditorWindow {

    bool start = true;
    BuildData bd;
    
    public static void Init() {
        BuildConfigPopup window = CreateInstance<BuildConfigPopup>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 280);
        window.ShowPopup();
    }

    void OnGUI() {
        if (start) {
            bd = BuildData.Load();
            if (bd == null)
                bd = new BuildData();
            start = !start;
        }
        
        GUILayout.BeginVertical();

        //Tittle
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Build options menu", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Build Name
        GUILayout.Label("Build Name:");
        GUILayout.BeginHorizontal();
        bd.buildName = EditorGUILayout.TextArea(bd.buildName);
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Build Path
        GUILayout.Label("Build Path:");
        GUILayout.BeginHorizontal();
        bd.path = EditorGUILayout.TextArea(bd.path);
        if (GUILayout.Button("Search", GUILayout.Width(80))) SearchPath();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Checks
        GUILayout.Label("Select target platforms:");
        bd.win = EditorGUILayout.Toggle("Windows", bd.win);
        bd.lin = EditorGUILayout.Toggle("Linux", bd.lin);
        bd.mac = EditorGUILayout.Toggle("Mac", bd.mac);
        GUILayout.Space(20);

        // Buttons
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Cancel", GUILayout.Width(80))) Close();
        if (GUILayout.Button("Save", GUILayout.Width(80))) Save();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void Save() {
        BuildData.Save(bd);
        Close();
    }

    private void SearchPath() {
        bd.path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", bd.path, "");
        bd.path = BuildManager.MakeRelative(@bd.path, @Application.dataPath);
    }
}