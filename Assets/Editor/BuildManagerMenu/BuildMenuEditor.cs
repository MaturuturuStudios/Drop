using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class BuildMenuEditor: MonoBehaviour {

    [MenuItem("Build/Scenes/Catch scenes")]
    private static void CatchScenes(MenuCommand command) {
        BuildManager.CatchScenes();
    }

    [MenuItem("Build/Scenes/Verify scenes catched")]
    private static void VerifyScenesCatched(MenuCommand command) {
        BuildManager.ValidateScenes();
    }

    [MenuItem("Build/Scenes/Show Scene catched")]
    private static void ShowScenesCatched(MenuCommand command) {
        BuildManager.ShowScenesCatched();
    }

    [MenuItem("Build/Configuration")]
    public static void Configuration(MenuCommand command) {
        BuildConfigPopup.Init();
    }

    [MenuItem("Build/Development build")]
    public static void DevelopmentBuild(MenuCommand command) {
        BuildManager.BuildGame();
    }

    [MenuItem("Build/Build With Postprocess")]
    public static void BuildGame(MenuCommand command) {
        BuildManager.BuildGame(false);
    }
}