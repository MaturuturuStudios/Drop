
public interface InterfaceUnlockedLevels {

    /// <summary>
    /// Unlock a level and store the game data
    /// </summary>
    /// <param name="level">the level to unlock</param>
    /// <returns>true if the operations was successfull, false otherwise</returns>
    bool UnlockLevel(LevelInfo level);

    /// <summary>
    /// Return if a level is available in the game
    /// Available does not means that the level is unlocked
    /// </summary>
    /// <param name="level">level to check</param>
    /// <returns>false if is not available, true otherwise</returns>
    bool IsAvailableLevel(LevelInfo level);

    /// <summary>
    /// Return if a level is unlocked
    /// If the level is not available, is also locked
    /// </summary>
    /// <param name="level"></param>
    /// <returns>false if the level is locked or unavailable</returns>
    bool IsUnlockedlevel(LevelInfo level);

    /// <summary>
    /// Get the scene of a level
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    Scene GetScene(LevelInfo level);

    /// <summary>
    /// Return the last level unlocked
    /// if not, 0,0 is always the last level
    /// </summary>
    /// <returns></returns>
    LevelInfo GetLastUnlockedLevel();
}
