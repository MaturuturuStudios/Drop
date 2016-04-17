using UnityEngine;
using UnityEditor;

public class BuildMenuEditor: MonoBehaviour {

    [MenuItem("Build/Scenes/Catch Scenes")]
    private static void CatchScenes(MenuCommand command) {
        BuildManager.CatchScenes();
    }

    [MenuItem("Build/Scenes/Verify Scenes Catched")]
    private static void VerifyScenesCatched(MenuCommand command) {
        BuildManager.ValidateScenes();
    }

    [MenuItem("Build/Scenes/Show Scene Catched")]
    private static void ShowScenesCatched(MenuCommand command) {
        BuildManager.ShowScenesCatched();
    }

    [MenuItem("Build/Configuration")]
    public static void Configuration(MenuCommand command) {
        BuildConfigPopup.Init();
    }

    [MenuItem("Build/Development Build With Postprocess")]
    public static void DevelopmentBuild(MenuCommand command) {
        BuildManager.BuildGame();
    }

    [MenuItem("Build/Build With Postprocess")]
    public static void BuildGame(MenuCommand command) {
        BuildManager.BuildGame(false);
    }
}