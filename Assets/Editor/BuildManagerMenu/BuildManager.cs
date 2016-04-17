using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.IO;

// Zip for windows
// http://gnuwin32.sourceforge.net/packages/zip.htm

/// <summary>
/// Script for manage the build process
/// </summary>
public class BuildManager {


    #region Private Attributes

    /// <summary>
    /// Data used in the build process
    /// </summary>
    private BuildData bd = new BuildData();

    #endregion

    #region Public Methods

    /// <summary>
    /// Build main process
    /// </summary>
    /// <param name="development">Build type</param>
    public static void BuildGame(bool development = true) {

        // Load data from file
        BuildManager bm = new BuildManager();
        bm.bd = BuildData.Load();

        //Verify that all data is ok
        if (bm.ValidateData()) {

            // Construct configuration confirmation string
            string succes = GetSuccesString(bm);

            Debug.Log(succes);

            // Show confirmation dialog
            if (EditorUtility.DisplayDialog("Build configuration", succes, "Proceed", "Stop")) {

                Debug.Log("Proceed to build");
                Debug.Log("Using \"" + bm.bd.workPath + "\" directory");

                /*
                // Copy a file from the project folder to the build folder, alongside the built game.
                FileUtil.CopyFileOrDirectory("Assets/WebPlayerTemplates/Readme.txt", path + "Readme.txt");*/

                // Build Windows
                if (bm.bd.win) {
                    BuildWindows(bm, development);
                }

                // Build Linux
                if (bm.bd.lin) {
                    BuildLinux(bm, development);
                }

                // Build Mac
                if (bm.bd.mac) {
                    BuildMac(bm, development);
                }

                // Wait for all work done
                System.Threading.Thread.Sleep(5000);

                // Packing all
                Debug.Log("Packing");

                // Compress to one file
                Compress(bm, "Package", "-9 -r " + bm.bd.buildName + ".zip " + (bm.bd.win ? bm.bd.buildName + "_win.zip " : "") + (bm.bd.lin ? bm.bd.buildName + "_lin.zip " : "") + (bm.bd.mac ? bm.bd.buildName + "_mac.zip " : ""));

                // Wait for all work done
                System.Threading.Thread.Sleep(5000);

                // Share file
                if (bm.bd.share)
                    ShareFile(bm, "Package");
            }
        }
    }


    /// <summary>
    /// Look if the scenes saved number is the same as the number of scenes in build settings
    /// </summary>
    /// <returns>If scenes are the right ones</returns>
    public static bool ValidateScenes() {

        // Process allowed only when game is not playing
        if (!EditorApplication.isPlaying) {

            // Load data from file
            BuildData bd = BuildData.Load();
            if (bd == null) {

                Debug.Log("Scenes not cathed");
                return false;

            } else {

                //Show number of scenes stored and in build settings
                Debug.Log("Number of scenes in build settings: " + SceneManager.sceneCountInBuildSettings);
                Debug.Log("Number of scenes in savedScenes: " + bd.scenes.Count);

                // Compare number of scenes
                if (bd.scenes.Count == SceneManager.sceneCountInBuildSettings) {

                    Debug.Log("Ok, All scenes are included");
                    return true;

                } else {

                    Debug.LogWarning("Not all scenes are included!");
                    return false;

                }
            }
        } else {

            Debug.LogError("Please, exit Play Mode");
            return false;
        }
    }


    /// <summary>
    /// Catches the scenes with its path in build settings
    /// </summary>
    public static void CatchScenes() {

        // Process allowed only when game is playing
        if (EditorApplication.isPlaying) {

            // Load data from file
            BuildData bd = BuildData.Load();
            if (bd == null)
                bd = new BuildData();

            // Clear old scenes
            bd.scenes.Clear();

            // Get new scenes
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i) {

                //Load next Scene
                SceneManager.LoadScene(i);

                // Get scene path
                string path = SceneManager.GetSceneAt(i).path;
                if (i > 0) {
                    bd.scenes.Add(path);
                    Debug.Log("Scene Loaded: " + path);
                }


            }

            Debug.Log(bd.scenes.Count + " scenes catched");

            // Save data to file
            BuildData.Save(bd);

        } else {
            Debug.LogError("You must be in Play Mode");
        }
    }


    /// <summary>
    /// Shows in console the scenes catched
    /// </summary>
    public static void ShowScenesCatched() {

        // Load data from file
        BuildData bd = BuildData.Load();
        if (bd == null || bd.scenes == null) {
            Debug.Log("Scenes not cathed");
            return;
        }


        // Print scenes
        for (int i = 0; i < bd.scenes.Count; ++i)
            Debug.Log("Scene[" + i + "]: " + bd.scenes[i]);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Windows build process
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <param name="development">Build type</param>
    private static void BuildWindows(BuildManager bm, bool development) {

        Debug.Log("Building for Windows platform");

        // Get full build name 
        string fullPath = bm.bd.workPath + "/" + bm.bd.buildName + "_win.exe";

        // Build it
        BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneWindows, (development ? BuildOptions.Development : BuildOptions.None));

        Debug.Log("Build for Windows done");

        // Compress 
        Compress(bm, "Windows", "-9 -r " + bm.bd.buildName + "_win.zip " + bm.bd.buildName + "_win.exe " + bm.bd.buildName + "_win_Data ");
    }


    /// <summary>
    /// Linux build process
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <param name="development">Build type</param>
    private static void BuildLinux(BuildManager bm, bool development) {

        Debug.Log("Building for Linux platform");

        // Get full build name 
        string fullPath = bm.bd.workPath + "/" + bm.bd.buildName + "_lin.x86";

        // Build it
        BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneLinux, (development ? BuildOptions.Development : BuildOptions.None));

        Debug.Log("Build for Linux done");

        // Compress 
        Compress(bm, "Linux", "-9 -r " + bm.bd.buildName + "_lin.zip " + bm.bd.buildName + "_lin.x86 " + bm.bd.buildName + "_lin_Data ");
    }


    /// <summary>
    /// Mac build process
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <param name="development">Build type</param>
    private static void BuildMac(BuildManager bm, bool development) {

        Debug.Log("Building for Mac platform");

        // Get full build name 
        string fullPath = bm.bd.workPath + "/" + bm.bd.buildName + "_mac.app";

        // Build it
        BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneLinux, (development ? BuildOptions.Development : BuildOptions.None));

        Debug.Log("Build for Mac done");

        // Compress 
        Compress(bm, "Mac", "-9 -r " + bm.bd.buildName + "_mac.zip " + bm.bd.buildName + "_mac.app ");
    }


    /// <summary>
    /// Compression process
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <param name="label">Label to show in messages</param>
    /// <param name="arguments">Arguments needed to compression process</param>
    private static void Compress(BuildManager bm, string label, string arguments) {

        Debug.Log("Compressing " + label + " build");

        // Create a new parallel process
        System.Diagnostics.Process proc = new System.Diagnostics.Process();

        try {

            // Get zip location
            proc.StartInfo.FileName = MakeAbsolute(bm.bd.zipPath).Replace(@"\", @"\\").ToString();

            // Get directory where process will work
            proc.StartInfo.WorkingDirectory = MakeAbsolute(bm.bd.workPath);

            // Set arguments
            proc.StartInfo.Arguments = arguments;

            // Set custom options for not open a shell and no create a window
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            // Start compression
            proc.Start();

            // Compression done
            Debug.Log("Compression for " + label + " build done");

        } catch (Exception e) {

            // Compression failed
            Debug.LogError("Compression for " + label + " failed: " + e.ToString());
        }
    }


    /// <summary>
    /// Share file
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <param name="label">Label to show in messages</param>
    private static void ShareFile(BuildManager bm, string label) {

        Debug.Log("Sharing " + label + " build");

        try {

            // Get folders
            string from = bm.bd.workPath + "/" + bm.bd.buildName + ".zip ";
            string to = bm.bd.sharedFolderPath + "/" + bm.bd.buildName + ".zip ";

            // Make paths absolute
            from = MakeAbsolute(from);
            to = MakeAbsolute(to);

            // Copy package to shared folder
            FileUtil.CopyFileOrDirectory(from, to);

            // Move done
            Debug.Log("File shared in: " + to);

        } catch (Exception e) {

            // Move failed
            Debug.LogError("Share failed: " + e.ToString());
        }
    }


    /// <summary>
    /// Construct configuration confirmation string process
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <returns>Configuration confirmation string</returns>
    private static string GetSuccesString(BuildManager bm) {

        return "Build configurated succesfully"
        + "\nBuild name: "
        + bm.bd.buildName
        + "\nTarget platforms: "
        + (bm.bd.win == true ? "Windows" : "")
        + (bm.bd.win == true && (bm.bd.mac == true) ? ", " : "")
        + (bm.bd.mac == true ? "Mac" : "")
        + (((bm.bd.win == true || bm.bd.mac == true) && bm.bd.lin == true) ? ", " : "")
        + (bm.bd.lin == true ? "Linux" : "")
        + "\nWork folder: "
        + bm.bd.workPath
        + (bm.bd.share == true ?
            "\nShared folder: " + bm.bd.sharedFolderPath
            : "")
        + "\nZip location: "
        + bm.bd.zipPath;
    }


    /// <summary>
    /// Script who verify that all data is correct
    /// </summary>
    /// <returns>If data is right</returns>
    private bool ValidateData() {

        // Look for scenes
        if (!ValidateScenes()) {
            EditorUtility.DisplayDialog("Configuration error",
            "There are no scenes selected or catched scenes aren't the right ones, please re-catch scenes.", "Ok");
            return false;
        }

        // Look for build name
        if (bd.buildName == "") {
            EditorUtility.DisplayDialog("Configuration error",
            "Build name cannot be empty", "Ok");
            return false;
        }

        // Look for work folder
        if (bd.workPath == "") {
            EditorUtility.DisplayDialog("Configuration error",
            "Build path cannot be empty", "Ok");
            return false;
        }

        // Look for target platforms
        if (bd.win == false && bd.lin == false && bd.mac == false) {
            EditorUtility.DisplayDialog("Configuration error",
            "You must select at least one target platform", "Ok");
            return false;
        }

        // Look for shared folder
        if (bd.share && bd.sharedFolderPath == "") {
            EditorUtility.DisplayDialog("Configuration error",
            "If you want to share it, share folder path cannot be empty", "Ok");
            return false;
        }

        // Look for zip.exe
        if (bd.zipPath == "") {
            EditorUtility.DisplayDialog("Configuration error",
            "Zip file must be setted", "Ok");
            return false;
        }

        return true;
    }

    #endregion


    #region Public Path Utils

    /// <summary>
    /// Converts a path to relative
    /// </summary>
    /// <param name="filePath">Path to convert</param>
    /// <param name="referencePath">Path to fix as reference</param>
    /// <returns></returns>
    public static string MakeRelative(string filePath, string referencePath) {

        // Check if it is relative
        if (!IsAbsoluteUrl(filePath)) {
            return filePath;
        }

        // Convert to relative
        var fileUri = new Uri(filePath);
        var referenceUri = new Uri(referencePath);
        return referenceUri.MakeRelativeUri(fileUri).ToString();
    }

    /// <summary>
    /// Converts a path to absolute
    /// </summary>
    /// <param name="filePath">Path to convert</param>
    /// <returns></returns>
    public static string MakeAbsolute(string filePath) {

        // Check if it is absolute
        if (IsAbsoluteUrl(filePath))
            return filePath;

        // Convert to absolute
        return Path.GetFullPath(filePath);
    }


    /// <summary>
    /// Validates if a path is absilute
    /// </summary>
    /// <param name="filePath">Path to validate</param>
    private static bool IsAbsoluteUrl(string filePath) {

        Uri result;
        return Uri.TryCreate(filePath, UriKind.Absolute, out result);
    }

    #endregion

}
