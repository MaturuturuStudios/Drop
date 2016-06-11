using UnityEngine;
using UnityEditor;

public class BuildMenuEditor: MonoBehaviour {

    [MenuItem("Build/Scenes/Catch Scenes", false, 1)]
    private static void CatchScenes(MenuCommand command) {
        BuildManager.CatchScenes();
    }

    [MenuItem("Build/Scenes/Verify Scenes Catched", false, 2)]
    private static void VerifyScenesCatched(MenuCommand command) {
        BuildManager.ValidateScenes();
    }

    [MenuItem("Build/Scenes/Show Scene Catched", false, 3)]
    private static void ShowScenesCatched(MenuCommand command) {
        BuildManager.ShowScenesCatched();
    }

    [MenuItem("Build/Configuration", false, 15)]
    public static void Configuration(MenuCommand command) {
        BuildConfigPopup.Init();
    }

    [MenuItem("Build/Development Build", false, 27)]
    public static void DevelopmentBuild(MenuCommand command) {
        BuildManager.BuildGame();
    }

    [MenuItem("Build/Release With Postprocess", false, 28)]
    public static void BuildGame(MenuCommand command) {
        BuildManager.BuildGame(false);
    }
}