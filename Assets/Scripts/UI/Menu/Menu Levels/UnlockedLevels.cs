using UnityEngine;
using System;

[Serializable]
public class Array2D{
    [SerializeField]
    public bool[] anArray;

    public bool this [int index] {
        get { return anArray[index]; }
        set { anArray[index] = value; }
    }

    public int Length {
        get { return anArray.Length; }
    }
}

[Serializable]
public class UnlockedLevels {
    /// <summary>
    /// Store how much levels have a world of that index
    /// </summary>
    [SerializeField]
    private int[] numberLevelsOnWorld;
    /// <summary>
    /// Set the available levels of the game
    /// remember, the index start on 0
    /// </summary>
    [SerializeField]
    private Array2D[] availableLevels;
    /// <summary>
    /// Set the unlocked levels of the game
    /// remember, the index start on 0
    /// </summary>
    [SerializeField]
    private Array2D[] unlockedLevels;
    
    /// <summary>
    /// Return the last level unlocked
    /// if not, 0,0 is always the last level
    /// </summary>
    /// <returns></returns>
    public LevelInfo GetLastUnlockedLevel() {
        LevelInfo info;
        info.world = 0;
        info.level = 0;

        bool done = false;
        for(int i=numberLevelsOnWorld.Length-1; i>0 && !done; i--) {
            //this world have no levels unlocked...
            if (unlockedLevels[i].Length == 0) continue;

            //this world have!
            info.world = i;
            //start with the low level
            info.level = 0;
            //search the higher unlocked level
            bool levelDone = false;
            for (int j = numberLevelsOnWorld[i]-1; j > 0 && !levelDone; j--) {
                if (!unlockedLevels[i][j]) continue;

                if (info.level < i) {
                    info.level = i;
                    levelDone = true;
                }
            }
            //done!
            done = true;
        }

        return info;
    }

    /// <summary>
    /// Unlock all the levels of a world if parameter is true
    /// otherwise lock the levels
    /// </summary>
    /// <param name="unlock">true if want to unlock the levels, by default</param>
    /// <param name="world">from 0 to number worlds-1</param>
	public void UnlockWorld(int world, bool unlock=true) {
        //get the number of levels of the world
        int numberlevel = numberLevelsOnWorld[world];

        //clear the array and fill
        //for all the array...
        for (int i = 0; i < unlockedLevels[world].Length; i++) {
            //if the level is available, set the desired lock
            //if not, set as false always
            unlockedLevels[world][i] = (i>numberlevel)? false : unlock;
        }
    }

    /// <summary>
    /// Unlock a level
    /// </summary>
    /// <param name="level">level to unlock</param>
    /// <param name="unlock">true to unlock the level, by default, false if want to lock the level</param>
    public void UnlockLevel(LevelInfo level, bool unlock=true) {
        if (level.level > (numberLevelsOnWorld[level.world] - 1)) return;
        unlockedLevels[level.world][level.level] = unlock;
    }

    /// <summary>
    /// Unlock the next level available
    /// </summary>
    public void UnlockNextLevel() {
        LevelInfo nextLevel = GetNextAvailableLevel();
        UnlockLevel(nextLevel);

    }

    /// <summary>
    /// Return the next available level
    /// if there is no next, return the last unlocked level (same as the last available level)
    /// </summary>
    /// <returns></returns>
    public LevelInfo GetNextAvailableLevel() {
        LevelInfo lastUnlocked = GetLastUnlockedLevel();
        LevelInfo nextAvailable = lastUnlocked;

        int lastWorld = numberLevelsOnWorld.Length - 1;
        int lastLevel = numberLevelsOnWorld[lastWorld] - 1;

        bool done = false;
        do {
            //plus the level
            lastUnlocked.level++;

            //if no more level, plus the world and reset level
            if(lastUnlocked.level > numberLevelsOnWorld[lastUnlocked.world]) {
                lastUnlocked.level = 0;
                lastUnlocked.world++;
            }

            //if searched in all worlds, is over
            if (lastUnlocked.world > lastWorld) done = true;

            //if not available, next
            if (!IsAvailableLevel(lastUnlocked)) continue;
            else {
                nextAvailable = lastUnlocked;
                done = true;
            }

        } while (!done);

        return nextAvailable;

    }

    /// <summary>
    /// Return if a level is available in the game
    /// Available does not means that the level is unlocked
    /// </summary>
    /// <param name="level">level to check</param>
    /// <returns>false if is not available, true otherwise</returns>
    public bool IsAvailableLevel(LevelInfo level) {
        if (level.world > numberLevelsOnWorld.Length - 1 || level.level > numberLevelsOnWorld[level.world] - 1) return false;
        return availableLevels[level.world][level.level];
    }

    /// <summary>
    /// Return if a level is unlocked
    /// If the level is not available, is also locked
    /// </summary>
    /// <param name="level"></param>
    /// <returns>false if the level is locked or unavailable</returns>
    public bool IsUnlockedlevel(LevelInfo level) {
        if (IsAvailableLevel(level)) return unlockedLevels[level.world][level.level];
        else return false;
        
    }


}
