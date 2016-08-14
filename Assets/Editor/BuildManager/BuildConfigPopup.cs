using UnityEngine;
using UnityEditor;

/// <summary>
/// Popup for set the build configuration
/// </summary>
public class BuildConfigPopup : EditorWindow {

    #region Private Attributes

    /// <summary>
    /// Handle to set the non static values when popup is started
    /// </summary>
    private bool start = true;

    /// <summary>
    /// Attribute for allow the data loaded and the data to save
    /// </summary>
    private BuildData bd;

    #endregion

    #region Public Methods

    /// <summary>
    /// Static method call to create the popup
    /// </summary>
    public static void Init() {

        // Create an instance of the popup
        BuildConfigPopup window = CreateInstance<BuildConfigPopup>();
            
        // Set window dimensions
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 350);

        // Set the popup visible
        window.ShowPopup();
    }

    #endregion

    #region Private Methods

    private void OnGUI() {

        // Set the non static values when popup is started
        if (start) {

            // Try to find saved data
            bd = BuildData.Load();

            if (bd == null)
                // Setting default data
                bd = new BuildData();

            // Toggle start to false
            start = false;
        }
        
        //Popup Start
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
        if (GUILayout.Button("Search", GUILayout.Width(80))) bd.workPath = SearchPath(bd.workPath);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Checks
        GUILayout.Label("Select target platforms:");
        bd.win32 = EditorGUILayout.Toggle("  Windows32", bd.win32);
        bd.win64 = EditorGUILayout.Toggle("  Windows64", bd.win64);
        bd.lin = EditorGUILayout.Toggle("  Linux", bd.lin);
        bd.mac = EditorGUILayout.Toggle("  Mac", bd.mac);

        GUILayout.Space(10);

        // Use scene validation
        bd.countScenes = EditorGUILayout.Toggle("Scene validation: ", bd.countScenes);

        GUILayout.Space(10);

        // Share folder & Path
        bd.share = EditorGUILayout.Toggle("Send to shared folder: ", bd.share);
        if (bd.share) {

            GUILayout.Label("Shared folder path:");
            GUILayout.BeginHorizontal();
            bd.sharedFolderPath = EditorGUILayout.TextArea(bd.sharedFolderPath);
            if (GUILayout.Button("Search", GUILayout.Width(80))) bd.sharedFolderPath = SearchPath(bd.sharedFolderPath);
            GUILayout.EndHorizontal();
        }else {

            // If option disable substitute it for an empty space
            GUILayout.Space(38);
        }

        GUILayout.Space(10);

        // Buttons
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Search zip.exe", GUILayout.Width(120))) SearchZipExe();
        GUILayout.Space(40);
        if (GUILayout.Button("Cancel", GUILayout.Width(80))) Close();
        if (GUILayout.Button("Save", GUILayout.Width(80))) Save();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Popup End
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Save the data to the file and close the popup
    /// </summary>
    private void Save() {

        // Save the data to the file
        BuildData.Save(bd);

        // Close the popup
        Close();
    }

    /// <summary>
    /// Searches a path with a SaveFolderPanel
    /// </summary>
    /// <param name="currentPath">Folder to start to search</param>
    /// <returns>A string with the selected path</returns>
    private string SearchPath(string currentPath) {

        // Return variable
        string path = currentPath;

        // Controls empty TextBox
        if (currentPath == "")
            path = "C:\\";

        // Lunch SaveFolderPanel window
        path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", BuildManager.MakeAbsolute(path), "");

        if (path != "") {

            // If path isn't empty parse it
            path = BuildManager.MakeRelative(@path, @Application.dataPath);

        }else {

            // If path is empty put path back
            path = currentPath;
        }

        return path;
    }


    /// <summary>
    /// Searches a file path with a SaveFilePanel and sets the value to bd.zipPath
    /// </summary>
    /// <returns>A string with the selected path</returns>
    private void SearchZipExe() {

        // Return variable
        string path;

        // Lunch SaveFilePanel window
        path = EditorUtility.OpenFilePanel("Search Zip.exe", BuildManager.MakeAbsolute(bd.zipPath).Replace(@"\", @"\\").ToString(), "exe");

        // If path isn't empty parse it and store it
        if (path != "") {
            bd.zipPath = path;
        }
    }

    #endregion
}