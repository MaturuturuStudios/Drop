using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class BuildManager {

    public BuildData bd = new BuildData();


    public static void BuildGame(bool development = true) {

        //Verify that all data
        BuildManager bm = new BuildManager();
        bm.bd = BuildData.Load();
        if (bm.ValidateData()) {
            string succes =
                "Build configurated succesfully\nNow we proced to build ("
                + (bm.bd.win == true ? "Windows" : "")
                + (bm.bd.win == true && (bm.bd.mac == true || bm.bd.lin == true) ? ", " : "")
                + (bm.bd.mac == true ? "Mac" : "")
                + ((bm.bd.mac == true && bm.bd.lin == true) ? ", " : "")
                + (bm.bd.lin == true ? "Linux" : "")
                + "). \nThe target folder will be: \""
                + bm.bd.path
                + "\" \nThe build name is: "
                + bm.bd.buildName;

            Debug.Log(succes);
        }

        // Build Windows
        if (bm.bd.win) {
            Debug.Log("Building for Windows platform");
            string fullPath = bm.bd.path + "/" + bm.bd.buildName + "_win.exe";
            BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneWindows, (development ? BuildOptions.Development : BuildOptions.None));


            Debug.Log("Compressing and uploading to: " + bm.bd.path);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            try {
                proc.StartInfo.FileName = MakeRelative("D:/Program Files (x86)/GnuWin32/bin/zip.exe", Application.dataPath);
                proc.StartInfo.WorkingDirectory = bm.bd.path;
                proc.StartInfo.Arguments = "-9 -r " + bm.bd.buildName + "_win.zip " + bm.bd.buildName + "_win.exe " + bm.bd.buildName + "_win_Data ";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
            } catch (Exception e) {
                Debug.LogError("Compression failed: " + e.ToString());
            }
        }

        // Build Linux
        if (bm.bd.lin) {
            Debug.Log("Building for Linux platform");
            string fullPath = bm.bd.path + "/" + bm.bd.buildName + "_lin.x86";
            BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneLinux, (development ? BuildOptions.Development : BuildOptions.None));


            Debug.Log("Compressing and uploading to: " + bm.bd.path);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            try {
                proc.StartInfo.FileName = MakeRelative("D:/Program Files (x86)/GnuWin32/bin/zip.exe", Application.dataPath);
                proc.StartInfo.WorkingDirectory = bm.bd.path;
                proc.StartInfo.Arguments = "-9 -r " + bm.bd.buildName + "_lin.zip " + bm.bd.buildName + "_lin.x86 " + bm.bd.buildName + "_lin_Data ";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
            } catch (Exception e) {
                Debug.LogError("Compression failed: " + e.ToString());
            }
        }

        // Build Mac
        if (bm.bd.mac) {
            Debug.Log("Building for Mac platform");
            string fullPath = bm.bd.path + "/" + bm.bd.buildName + "_mac.app";
            BuildPipeline.BuildPlayer(bm.bd.scenes.ToArray(), fullPath, BuildTarget.StandaloneLinux, (development ? BuildOptions.Development : BuildOptions.None));


            Debug.Log("Compressing and uploading to: " + bm.bd.path);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            try {
                proc.StartInfo.FileName = MakeRelative("D:/Program Files (x86)/GnuWin32/bin/zip.exe", Application.dataPath);
                proc.StartInfo.WorkingDirectory = bm.bd.path;
                proc.StartInfo.Arguments = "-9 -r " + bm.bd.buildName + "_mac.zip " + bm.bd.buildName + "_mac.app ";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
            } catch (Exception e) {
                Debug.LogError("Compression failed: " + e.ToString());
            }
        }


        // Build Mac
        if (bm.bd.lin) {
            Debug.Log("Packing");
            System.Threading.Thread.Sleep(5000);
            Debug.Log("Compressing and uploading to: " + bm.bd.path);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            try {
                proc.StartInfo.FileName = MakeRelative("D:/Program Files (x86)/GnuWin32/bin/zip.exe", Application.dataPath);
                proc.StartInfo.WorkingDirectory = bm.bd.path;
                proc.StartInfo.Arguments = "-9 -r " + bm.bd.buildName + ".zip " + (bm.bd.win ? bm.bd.buildName + "_win.zip " : "") + (bm.bd.lin ? bm.bd.buildName + "_lin.zip " : "") + (bm.bd.mac ? bm.bd.buildName + "_mac.zip " : "");
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
            } catch (Exception e) {
                Debug.LogError("Compression failed: " + e.ToString());
            }
        }


        /*
        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/WebPlayerTemplates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = path + "BuiltGame.exe";
        proc.Start();*/
    }


    public bool ValidateData() {

        if (!ValidateScenes())
            return false;

        if (bd.buildName == "") {
            EditorUtility.DisplayDialog("Configuration error",
            "Build name cannot be empty", "Ok");
            return false;
        }

        if (bd.path == "") {
            EditorUtility.DisplayDialog("Configuration error",
            "Build path cannot be empty", "Ok");
            return false;
        }

        if (bd.win == false && bd.lin == false && bd.mac == false) {
            EditorUtility.DisplayDialog("Configuration error",
            "You must select at least one target platform", "Ok");
            return false;
        }

        return true;
    }

    public static bool ValidateScenes() {
        if (!EditorApplication.isPlaying) {
            BuildData bd = BuildData.Load();
            if (bd == null) {
                Debug.Log("Scenes not cathed");
                return false;
            } else {
                Debug.Log("Number of scenes in build settings: " + SceneManager.sceneCountInBuildSettings);
                Debug.Log("Number of scenes in savedScenes: " + bd.scenes.Count);

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


    public static void CatchScenes() {
        if (EditorApplication.isPlaying) {
            BuildData bd = BuildData.Load();
            if (bd == null) 
                bd = new BuildData();

            bd.scenes.Clear();
            SceneManager.LoadScene(0);
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; ++i) {
                bd.scenes.Add(SceneManager.GetSceneAt(i).path);
                SceneManager.LoadScene(i);
                Debug.Log("Scene Loaded: " + SceneManager.GetSceneAt(i).path);
            }

            Debug.Log(bd.scenes.Count + " scenes catched");

            BuildData.Save(bd);
            
        } else {
            Debug.LogError("You must be in Play Mode");
        }
    }


    public static void ShowScenesCatched() {

        BuildData bd = BuildData.Load();
        if (bd == null || bd.scenes == null) {
            Debug.Log("Scenes not cathed");
            return;
        }

        for (int i = 0; i < bd.scenes.Count; ++i)
            Debug.Log("Scene[" + i + "]: " + bd.scenes[i]);
    }

    public static string MakeRelative(string filePath, string referencePath) {
        var fileUri = new Uri(filePath);
        var referenceUri = new Uri(referencePath);
        return referenceUri.MakeRelativeUri(fileUri).ToString();
    }
}
