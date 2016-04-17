using UnityEngine;
using UnityEditor;

public class BuildConfigPopup : EditorWindow {

    bool start = true;
    BuildData bd;
    
    public static void Init() {
        BuildConfigPopup window = CreateInstance<BuildConfigPopup>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 305);
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

        GUILayout.Space(10);

        // Build Name
        GUILayout.Label("Build name:");
        GUILayout.BeginHorizontal();
        bd.buildName = EditorGUILayout.TextArea(bd.buildName);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Build Path
        GUILayout.Label("Build path:");
        GUILayout.BeginHorizontal();
        bd.workPath = EditorGUILayout.TextArea(bd.workPath);
        if (GUILayout.Button("Search", GUILayout.Width(80))) bd.workPath = SearchPath(bd.workPath) != "" ? SearchPath(bd.workPath) : bd.workPath;
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Checks
        GUILayout.Label("Select target platforms:");
        bd.win = EditorGUILayout.Toggle("Windows", bd.win);
        bd.lin = EditorGUILayout.Toggle("Linux", bd.lin);
        bd.mac = EditorGUILayout.Toggle("Mac", bd.mac);

        GUILayout.Space(10);

        // Share folder & Path
        bd.share = EditorGUILayout.Toggle("Send to shared folder: ", bd.share);
        if (bd.share) {
            GUILayout.Label("Shared folder path:");
            GUILayout.BeginHorizontal();
            bd.sharedFolderPath = EditorGUILayout.TextArea(bd.sharedFolderPath);
            if (GUILayout.Button("Search", GUILayout.Width(80))) bd.sharedFolderPath = SearchPath(bd.sharedFolderPath) != ""? SearchPath(bd.sharedFolderPath) : bd.sharedFolderPath;
            GUILayout.EndHorizontal();
        }else {

            GUILayout.Space(38);
        }

        GUILayout.Space(10);

        // Buttons
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Zip", GUILayout.Width(80))) SearchZipExe();
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

    private string SearchPath(string currentPath) {
        string path;
        path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", BuildManager.MakeAbsolute(currentPath), "");
        if(path != "")
            path = BuildManager.MakeRelative(@path, @Application.dataPath);
        return path;
    }

    private string SearchZipExe() {
        string path;
        path = EditorUtility.SaveFilePanel("Search Zip.exe", BuildManager.MakeAbsolute(bd.zipPath), "zip.exe", "exe");
        if (path != "")
            path = BuildManager.MakeRelative(@path, @Application.dataPath);
        return path;
    }
}