using UnityEngine;
using System.Collections;

public class SceneLevel : MonoBehaviour {

    public Scene level = null;
    public float waitBeforeStartLevel = 3f;


    public void StartLevel() {
        if (level == null || level.name == "Not" || level.name == "") return;
        // Get the navigator
        MenuNavigator _menuNavigator = GameObject.FindGameObjectWithTag(Tags.Menus)
                                 .GetComponent<MenuNavigator>();
        // Wait before starting the change
        _menuNavigator.ChangeScene(level.name, waitBeforeStartLevel);
    }
}
