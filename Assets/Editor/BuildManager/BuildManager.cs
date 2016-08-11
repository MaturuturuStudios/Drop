using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Globalization;
using System;
using System.IO;
using System.Collections.Generic;

// Zip for windows
// http://gnuwin32.sourceforge.net/packages/zip.htm

/// <summary>
/// Script for manage the build process
/// </summary>
public class BuildManager {

    #region

    public enum BuildType {
        Fast,
        Development,
        Release
    }

    #endregion

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
    /// <param name="buildTyoe">Build type</param>
    public static void BuildGame(BuildType buildType) {

        // Load data from file
        BuildManager bm = new BuildManager();
        bm.bd = BuildData.Load();

        //Verify that all data is ok
        if (bm.ValidateData()) {

            BuildOptions bo;
            string popUpTitle;
            // Set the desired build options
            if (buildType == BuildType.Release) {
                bo = BuildOptions.None;
                popUpTitle = "Release Configuration";
            } else {
                bo = BuildOptions.Development | BuildOptions.AllowDebugging;
                if (buildType == BuildType.Development)
                    popUpTitle = "Development Build Configuration";
                else {
                    popUpTitle = "Fast Build Configuration";

                    string name = DateTime.Now.ToString(new CultureInfo("en-GB"));
                    name = name.Replace(" ", "_");
                    name = name.Replace(":", "-");
                    name = name.Replace("/", "-");
                    bm.bd.buildName = "drop_" + name;
                    bm.bd.workPath = "Build/" + bm.bd.buildName;
                    bm.bd.win32 = true;
                    bm.bd.win64 = false;
                    bm.bd.lin = false;
                    bm.bd.mac = false;
                    bm.bd.scenes = new List<string>();
                    bm.bd.countScenes = false;
                }
            }

            // Construct configuration confirmation string
            string popupMessage = GetSuccesString(bm, buildType);

            Debug.Log(popupMessage);

            // Show confirmation dialog
            if (EditorUtility.DisplayDialog(popUpTitle, popupMessage, "Proceed", "Abort")) {

                Debug.Log("Proceed to build");
                Debug.Log("Using \"" + bm.bd.workPath + "\" directory");

                /*
                // Copy a file from the project folder to the build folder, alongside the built game.
                FileUtil.CopyFileOrDirectory("Assets/WebPlayerTemplates/Readme.txt", path + "Readme.txt");*/

                bool goOn = true;

                // Build Windows
                if (goOn && bm.bd.win32) {
                    goOn = BuildWindows32(bm, bo, buildType);
                }

                // Build Windows
                if (goOn && bm.bd.win64) {
                    goOn = BuildWindows64(bm, bo, buildType);
                }

                // Build Linux
                if (goOn && bm.bd.lin) {
                    goOn = BuildLinux(bm, bo, buildType);
                }

                // Build Mac
                if (goOn && bm.bd.mac) {
                    goOn = BuildMac(bm, bo, buildType);
                }

                if (buildType == BuildType.Release) {

                    if (goOn)
                        // Wait for all work done
                        System.Threading.Thread.Sleep(5000);

                    // Packing all
                    Debug.Log("Packing");

                    if (goOn)
                        // Compress to one file
                        Compress(bm, "Package", "-9 -r " + bm.bd.buildName + ".zip " + (bm.bd.win32 ? bm.bd.buildName + "_win32.zip " : "") + (bm.bd.win64 ? bm.bd.buildName + "_win64.zip " : "") + (bm.bd.lin ? bm.bd.buildName + "_lin.zip " : "") + (bm.bd.mac ? bm.bd.buildName + "_mac.zip " : ""));

                    if (goOn)
                        // Wait for all work done
                        System.Threading.Thread.Sleep(15000);

                    // Share file
                    if (goOn && bm.bd.share)
                        ShareFile(bm, "Package");
                }

                if (goOn)
                    ShowExplorer(bm.bd.workPath);
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

            // Get where to start
            int i = 0;

            // If scene is in build settings we have to count one more scene
            int handler = 0;


            // Get new scenes
            for (; i < SceneManager.sceneCountInBuildSettings + handler; ++i) {

                //Load next Scene
                SceneManager.LoadScene(i);

                // Get scene path
                string path = SceneManager.GetSceneAt(i).path;
                if (i > 0) {

                    // Add to scene list
                    bd.scenes.Add(path);
                    Debug.Log("Scene Loaded: " + path);

                    // Check if scene is build settings
                    if (SceneManager.GetSceneAt(0).name == SceneManager.GetSceneAt(i).name)
                        handler = 1;
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
    private static bool BuildWindows32(BuildManager bm, BuildOptions bo, BuildType bt) {

        Debug.Log("Building for Windows32 platform");

        // Get full build name 
        string fullPath = bm.bd.workPath + "/" + bm.bd.buildName + "_win32.exe";

        // Build it
        string error = BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneWindows, bo);

        if (String.IsNullOrEmpty(error)) {
            Debug.Log("Build for Windows32 done");

            // Compress 
            if (bt == BuildType.Release)
                Compress(bm, "Windows32", "-9 -r " + bm.bd.buildName + "_win32.zip " + bm.bd.buildName + "_win32.exe " + bm.bd.buildName + "_win32_Data ");

            return true;

        } else {
            Debug.Log("Error While building to Windows32: " + error);

            return false;
        }

    }

    /// <summary>
    /// Windows build process
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <param name="development">Build type</param>
    private static bool BuildWindows64(BuildManager bm, BuildOptions bo, BuildType bt) {

        Debug.Log("Building for Windows64 platform");

        // Get full build name 
        string fullPath = bm.bd.workPath + "/" + bm.bd.buildName + "_win64.exe";

        // Build it
        string error = BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneWindows64, bo);

        if (String.IsNullOrEmpty(error)) {
            Debug.Log("Build for Windows64 done");

            // Compress 
            if (bt == BuildType.Release)
                Compress(bm, "Windows64", "-9 -r " + bm.bd.buildName + "_win64.zip " + bm.bd.buildName + "_win64.exe " + bm.bd.buildName + "_win64_Data ");

            return true;

        } else {
            Debug.Log("Error While building to Windows: " + error);

            return false;
        }

    }


    /// <summary>
    /// Linux build process
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <param name="development">Build type</param>
    private static bool BuildLinux(BuildManager bm, BuildOptions bo, BuildType bt) {

        Debug.Log("Building for Linux platform");

        // Get full build name 
        string fullPath = bm.bd.workPath + "/" + bm.bd.buildName + "_lin.x86";

        // Build it
        string error = BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneLinuxUniversal, bo);

        if (String.IsNullOrEmpty(error) && bt == BuildType.Release) {
            Debug.Log("Build for Linux done");

            // Compress 
            if (bt == BuildType.Release)
                Compress(bm, "Linux", "-9 -r " + bm.bd.buildName + "_lin.zip " + bm.bd.buildName + "_lin.x86 " + bm.bd.buildName + "_lin_Data ");

            return true;

        } else {
            Debug.Log("Error While building to Mac: " + error);

            return false;
        }
    }


    /// <summary>
    /// Mac build process
    /// </summary>
    /// <param name="bm">Buld manager object working on</param>
    /// <param name="development">Build type</param>
    private static bool BuildMac(BuildManager bm, BuildOptions bo, BuildType bt) {

        Debug.Log("Building for Mac platform");

        // Get full build name 
        string fullPath = bm.bd.workPath + "/" + bm.bd.buildName + "_mac.app";

        // Build it
        string error = BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneOSXUniversal, bo);

        if (String.IsNullOrEmpty(error) && bt == BuildType.Release) {
            Debug.Log("Build for Mac done");

            // Compress 
            if (bt == BuildType.Release)
                Compress(bm, "Mac", "-9 -r " + bm.bd.buildName + "_mac.zip " + bm.bd.buildName + "_mac.app ");

            return true;

        } else {
            Debug.Log("Error While building to Linux: " + error);

            return false;
        }
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
            proc.StartInfo.FileName = MakeAbsolute(bm.bd.zipPath);

            // Get directory where process will work
            proc.StartInfo.WorkingDirectory = MakeAbsolute(bm.bd.workPath);

            // Set arguments
            proc.StartInfo.Arguments = arguments;

            // Set custom options for not open a shell and no create a window
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            // Start compression
            proc.Start();

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

        string path = MakeAbsolute (bm.bd.sharedFolderPath);
		System.IO.Directory.CreateDirectory (path);
        
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
    private static string GetSuccesString(BuildManager bm, BuildType bt) {

        string res;

        if (bt == BuildType.Fast) {
            res = " Fast Build configurated succesfully"

            + "\n\nBuild name:\n"
            + bm.bd.buildName

            + "\n\nBuild for windows platforms on \""
            + bm.bd.workPath +"\" folder";

        }else if(bt == BuildType.Development) {

            res = " Development Build configurated succesfully"

            + "\n\nBuild name:\n"
            + bm.bd.buildName

            + "\n\nTarget platforms: "
            + (bm.bd.win32 == true ? "\n - Windows32" : "")
            + (bm.bd.win64 == true ? "\n - Windows64" : "")
            + (bm.bd.mac == true ? "\n - Mac" : "")
            + (bm.bd.lin == true ? "\n - Linux" : "")

            + "\n\nWork folder:\n"
            + bm.bd.workPath;

        } else {

            res = " Release configurated succesfully"

            + "\n\nBuild name:\n"
            + bm.bd.buildName

            + "\n\nTarget platforms: "
            + (bm.bd.win32 == true ? "\n - Windows" : "")
            + (bm.bd.win64 == true ? "\n - Windows" : "")
            + (bm.bd.mac == true ? "\n - Mac" : "")
            + (bm.bd.lin == true ? "\n - Linux" : "")

            + "\n\nWork folder:\n"
            + bm.bd.workPath

            + (bm.bd.share == true ?
                "\n\nShared folder:\n" + bm.bd.sharedFolderPath
                : "")

            + "\n\nZip location:\n"
            + bm.bd.zipPath;

        }

        return res;
    }


    /// <summary>
    /// Script who verify that all data is correct
    /// </summary>
    /// <returns>If data is right</returns>
    private bool ValidateData() {

        // Look for scenes
        if (bd.countScenes && !ValidateScenes()) {
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
        if (bd.win32 == false && bd.win64 == false && bd.lin == false && bd.mac == false) {
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
    public static bool IsAbsoluteUrl(string filePath) {

        Uri result;
        return Uri.TryCreate(filePath, UriKind.Absolute, out result);
    }


    /// <summary>
    /// opens a folder with desired path
    /// </summary>
    /// <param name="itemPath">Path to open</param>
    public static void ShowExplorer(string itemPath) {
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", itemPath);
    }

    #endregion

}
