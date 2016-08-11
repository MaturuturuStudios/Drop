using UnityEngine;

public class PlayerDataStoreKeys {
    public static readonly string PlayerUnlockedLevels = "PlayerUnlockedLevels";
}

public class GameControllerData : MonoBehaviour {
    /// <summary>
    /// Data about the available and unlocked levels
    /// </summary>
    [SerializeField]
    private UnlockedLevels levelsUnlocked;

    public UnlockedLevels Getlevels() {
        return levelsUnlocked;
    }

    public void Awake() {
        string unlocked = PlayerPrefs.GetString(PlayerDataStoreKeys.PlayerUnlockedLevels);
        //if have some data, use it over the inspector data
        if (unlocked.Trim().Length != 0) {

        }
    }

    public Scene GetNextScene(LevelInfo actualLevel) {
        return null; //for test!
        LevelInfo next = levelsUnlocked.GetNextAvailableLevel(actualLevel);
        if (next == actualLevel) return null;
        return levelsUnlocked.GetScene(next);
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
    /// Complete the level, which means the next level available will be
    /// unlocked and the data game will be stored
    /// </summary>
    /// <returns>true if the operations was successfull, false otherwise</returns>
    public bool CompleteLevel() {
        LevelInfo levelUnlocked = levelsUnlocked.UnlockNextLevel();
        string completed = PlayerPrefs.GetString(PlayerDataStoreKeys.PlayerUnlockedLevels);
        completed += ";" + levelUnlocked.world + "-" + levelUnlocked.level;
        PlayerPrefs.SetString(PlayerDataStoreKeys.PlayerUnlockedLevels, completed);
        return false;
    }

    /// <summary>
    /// Unlock a level and store the game data
    /// </summary>
    /// <param name="level">the level to unlock</param>
    /// <returns>true if the operations was successfull, false otherwise</returns>
    public bool UnlockLevel(LevelInfo level) {
        string completed = PlayerPrefs.GetString(PlayerDataStoreKeys.PlayerUnlockedLevels);
        completed += ";" + level.world + "-" + level.level;
        PlayerPrefs.SetString(PlayerDataStoreKeys.PlayerUnlockedLevels, completed);
        return false;
    }

    /// <summary>
    /// Split the string data about the unlocked levels and unlock them
    /// The rest of levels remains locked
    /// </summary>
    /// <param name="data">the string data from the user preferences</param>
    private void SetUnlockedLevels(string data) {
        //lock every level
        int numberWorld = levelsUnlocked.GetNumberWorld();
        for(int i=0; i<numberWorld; i++) {
            int numberLevels = levelsUnlocked.GetNumberLevels(i);
            for(int j=0; j<numberLevels; j++) {
                LevelInfo level;
                level.world = i;
                level.level = j;
                levelsUnlocked.UnlockLevel(level, false);
            }
        }

        //now unlock the levels
        string[] levels = data.Split(';');
        for(int i=0;i <levels.Length; i++) {
            LevelInfo level;
            string[] aLevel = levels[i].Split('-');
            level.world = int.Parse(aLevel[0]);
            level.level = int.Parse(aLevel[1]);
            levelsUnlocked.UnlockLevel(level);
        }
    }
}
