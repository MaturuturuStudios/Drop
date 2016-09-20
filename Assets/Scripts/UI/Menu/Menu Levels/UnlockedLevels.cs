using UnityEngine;
using System;

[Serializable]
public class ArrayBoolean{
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
public class ArrayScene {
	[SerializeField]
	public Scene[] anArray;

	public Scene this[int index] {
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
	private ArrayBoolean[] availableLevels;
	/// <summary>
	/// Set the unlocked levels of the game
	/// remember, the index start on 0
	/// </summary>
	[SerializeField]
	private ArrayBoolean[] unlockedLevels;
	/// <summary>
	/// The asociated scenes to the levels
	/// </summary>
	[SerializeField]
	private ArrayScene[] scenes;

	/// <summary>
	/// Return the number of worlds
	/// </summary>
	/// <returns></returns>
	public int GetNumberWorld() {
		return numberLevelsOnWorld.Length;
	}

	/// <summary>
	/// Return the number of levels of a world
	/// </summary>
	/// <param name="world">the world starting on zero</param>
	/// <returns>0 if no levels or not a world, the number otherwise</returns>
	public int GetNumberLevels(int world) {
		if (world < 0 || world > numberLevelsOnWorld.Length) return 0;
		return numberLevelsOnWorld[world];
	}

	/// <summary>
	/// Get the scene of a level
	/// </summary>
	/// <param name="level"></param>
	/// <returns></returns>
	public Scene GetScene(LevelInfo level) {
		if (level.world < 0 || level.world > numberLevelsOnWorld.Length) return null;
		if (level.level < 0 || level.level > numberLevelsOnWorld[level.world]) return null;
		return scenes[level.world][level.level];
	}

	/// <summary>
	/// Get the level of a scene's name
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public LevelInfo GetInfoLevel(string name) {
		LevelInfo result;
		result.level = -1;
		result.world = -1;

		//search on every level
		int worlds = GetNumberWorld();
		bool found = false;
		for(int i=0; i<worlds && !found; i++) {
			int levels = GetNumberLevels(i);
			for(int j=0; j< levels && !found; j++) {
				if (scenes[i][j] == null) continue;
				if (scenes[i][j].name.CompareTo(name) == 0) {
					result.level = j;
					result.world = i;
					found = true;
				}
			}
		}

		return result;
	}

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
		for(int i=numberLevelsOnWorld.Length-1; i>=0 && !done; i--) {
			//this world have!
			info.world = i;
			//start with the low level
			info.level = 0;
			//search the higher unlocked level
			for (int j = numberLevelsOnWorld[i]-1; j >= 0 && !done; j--) {
				if (!unlockedLevels[i][j]) continue;

				if (info.level <= j) {
					info.level = j;
					done = true;
				}
			}
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
	public LevelInfo UnlockNextLevel() {
		LevelInfo nextLevel = GetNextAvailableLevel();
		UnlockLevel(nextLevel);
		return nextLevel;
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
	/// Get the next available level
	/// </summary>
	/// <param name="actual"></param>
	/// <returns></returns>
	public LevelInfo GetNextAvailableLevel(LevelInfo actual) {
		LevelInfo nextAvailable = actual;

		int lastWorld = numberLevelsOnWorld.Length - 1;

		bool done = false;
		do {
			//plus the level
			actual.level++;

			//if no more level, plus the world and reset level
			if (actual.level > numberLevelsOnWorld[actual.world]) {
				actual.level = 0;
				actual.world++;
			}

			//if searched in all worlds, is over
			if (actual.world > lastWorld) done = true;

			//if not available, next
			if (!IsAvailableLevel(actual)) continue;
			else {
				nextAvailable = actual;
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

	public string ResumeStringUnlockedLevel() {
		string result="";
		int worlds = GetNumberWorld();
		bool found = false;
		for (int i = 0; i < worlds && !found; i++) {
			int levels = GetNumberLevels(i);
			for (int j = 0; j < levels && !found; j++) {
				if (unlockedLevels[i][j] == false) continue;
				result += ";" + i + "-" + j;
			}
		}
		if (result.Length > 0) result=result.Remove(0, 1);
		return result;
	}

	/// <summary>
	/// Split the string data about the unlocked levels and unlock them
	/// The rest of levels remains locked
	/// </summary>
	/// <param name="data">the string data from the user preferences</param>
	public void SetUnlockedLevels(string data) {
		//lock every level
		int numberWorld = GetNumberWorld();
		for(int i=0; i<numberWorld; i++) {
			int numberLevels = GetNumberLevels(i);
			for(int j=0; j<numberLevels; j++) {
				LevelInfo level;
				level.world = i;
				level.level = j;
				UnlockLevel(level, false);
			}
		}

		//now unlock the levels
		string[] levels = data.Split(';');
		for(int i=0;i <levels.Length; i++) {
			LevelInfo level;
			string[] aLevel = levels[i].Split('-');
			level.world = int.Parse(aLevel[0]);
			level.level = int.Parse(aLevel[1]);
			UnlockLevel(level);
		}
	}
}
