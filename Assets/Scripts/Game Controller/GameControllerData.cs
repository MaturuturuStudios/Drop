using UnityEngine;

public class PlayerDataStoreKeys {
    /// <summary>
    /// Store the list of unlocked levels
    /// </summary>
    public static readonly string PlayerUnlockedLevels = "PlayerUnlockedLevels";
    /// <summary>
    /// Store if is the first time for the player
    /// </summary>
    public static readonly string PlayerFirstTime = "PlayerFirstTime";
}

public class GameControllerData : MonoBehaviour, InterfaceUnlockedLevels {
    [HideInInspector]
    public static GameControllerData control;
    /// <summary>
    /// The default scene to use if no scene/level are available
    /// </summary>
    public Scene defaultScene;
    /// <summary>
    /// Data about the available and unlocked levels
    /// </summary>
    [SerializeField]
    private UnlockedLevels informationLevels;

    /// <summary>
    /// Get the data and initialize it
    /// </summary>
    public void Awake() {
        if (control == null) {
            DontDestroyOnLoad(gameObject);
            control = this;
        }else if (control != this) {
            Destroy(gameObject);
        }


        string unlocked = PlayerPrefs.GetString(PlayerDataStoreKeys.PlayerUnlockedLevels);
        //if have some data, use it over the inspector data
        if (unlocked.Trim().Length != 0) {
            SetUnlockedLevels(unlocked.Trim());
        } else {
            //store it
            string data=informationLevels.ResumeStringUnlockedLevel();
            PlayerPrefs.SetString(PlayerDataStoreKeys.PlayerUnlockedLevels, data);
            PlayerPrefs.Save();
        }

        if (defaultScene.name == "Not" || defaultScene.name == "") {
            Debug.LogWarning("Next Scene not setted, using StartScene by default, please, assign an scene");
            defaultScene.name = "StartScene";
        }
    }

    /// <summary>
    /// Return a default scene, used when there is no more levels
    /// </summary>
    /// <returns></returns>
    public Scene GetDefaultScene() {
        return defaultScene;
    }

    /// <summary>
    /// Get the next scene to this one (the next available and unlocked)
    /// </summary>
    /// <param name="actualLevel">the actual level to continue</param>
    /// <returns>the next available and unlocked level, if there is no next, return null</returns>
    public Scene GetNextScene(LevelInfo actualLevel, out LevelInfo nextLevel) {
        nextLevel = informationLevels.GetNextAvailableLevel(actualLevel);
        if (nextLevel == actualLevel) return null;
        return informationLevels.GetScene(nextLevel);
    }

    /// <summary>
    /// Get the next scene to this one (the next available and unlocked)
    /// </summary>
    /// <param name="actualLevel">the actual level to continue</param>
    /// <param name="nextLevel">The information of the next level</param>
    /// <returns>the next available and unlocked level, if there is no next, return null</returns>
    public Scene GetNextScene(string actualLevel, out LevelInfo nextLevel) {
        LevelInfo level=informationLevels.GetInfoLevel(actualLevel);
        Scene next=GetNextScene(level, out nextLevel);
        return next;
    }

    /// <summary>
    /// Get the last unlocked level
    /// </summary>
    /// <returns></returns>
    public Scene GetLastUnlockedScene() {
        return informationLevels.GetScene(informationLevels.GetLastUnlockedLevel());
    }

    /// <summary>
    /// Delete all data game and reset it
    /// </summary>
    /// <returns>true if successfull</returns>
    public bool ResetGame() {
        //delete levels unlocked
        //delete all options
        PlayerPrefs.DeleteAll();
        return true;
    }

    /// <summary>
    /// Unlock a level and store the game data
    /// </summary>
    /// <param name="level">the level to unlock</param>
    /// <returns>true if the operations was successfull, false otherwise</returns>
    public bool UnlockLevel(LevelInfo level) {
        informationLevels.UnlockLevel(level);
        string completed = PlayerPrefs.GetString(PlayerDataStoreKeys.PlayerUnlockedLevels);
        completed += ";" + level.world + "-" + level.level;
        PlayerPrefs.SetString(PlayerDataStoreKeys.PlayerUnlockedLevels, completed);
        PlayerPrefs.Save();
        return false;
    }

    /// <summary>
    /// Return if a level is available in the game
    /// Available does not means that the level is unlocked
    /// </summary>
    /// <param name="level">level to check</param>
    /// <returns>false if is not available, true otherwise</returns>
    public bool IsAvailableLevel(LevelInfo level) {
        return informationLevels.IsAvailableLevel(level);
    }

    /// <summary>
    /// Return if a level is unlocked
    /// If the level is not available, is also locked
    /// </summary>
    /// <param name="level"></param>
    /// <returns>false if the level is locked or unavailable</returns>
    public bool IsUnlockedlevel(LevelInfo level) {
        return informationLevels.IsUnlockedlevel(level);

    }

    /// <summary>
    /// Get the scene of a level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public Scene GetScene(LevelInfo level) {
        return informationLevels.GetScene(level);
    }

    /// <summary>
    /// Return the last level unlocked
    /// if not, 0,0 is always the last level
    /// </summary>
    /// <returns></returns>
    public LevelInfo GetLastUnlockedLevel() {
        return informationLevels.GetLastUnlockedLevel();
    }

    /// <summary>
    /// Split the string data about the unlocked levels and unlock them
    /// The rest of levels remains locked
    /// </summary>
    /// <param name="data">the string data from the user preferences</param>
    private void SetUnlockedLevels(string data) {
        //lock every level
        int numberWorld = informationLevels.GetNumberWorld();
        for(int i=0; i<numberWorld; i++) {
            int numberLevels = informationLevels.GetNumberLevels(i);
            for(int j=0; j<numberLevels; j++) {
                LevelInfo level;
                level.world = i;
                level.level = j;
                informationLevels.UnlockLevel(level, false);
            }
        }

        //now unlock the levels
        string[] levels = data.Split(';');
        for(int i=0;i <levels.Length; i++) {
            LevelInfo level;
            string[] aLevel = levels[i].Split('-');
            level.world = int.Parse(aLevel[0]);
            level.level = int.Parse(aLevel[1]);
            informationLevels.UnlockLevel(level);
        }
    }
}
