using UnityEngine;
using System.Collections;

public interface MapLevelListener {

    void OnChangeWorld(int previousWorld, int newWorld);

    void OnChangeLevel(LevelInfo previous, LevelInfo actual);

    void OnSelectedLevel(LevelInfo selected);
}
