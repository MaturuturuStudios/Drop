using UnityEngine;
using System.Collections;
using System;
using System.Text;

[Serializable]
public class ArrayInt{
	[SerializeField]
	public int[] anArray;

	public int this [int index] {
		get { return anArray[index]; }
		set { anArray[index] = value; }
	}

	public int Length {
		get { return anArray.Length; }
	}
}

public struct ScoreLevel{
	public int achieved;
	public int max;
}

[Serializable]
public class ScoreLevels {
	/// <summary>
	/// The number of worlds.
	/// </summary>
	//[SerializeField]
	//private int numberWorlds;
	/// <summary>
	/// Store how much levels have a world of that index
	/// </summary>
	[SerializeField]
	private int[] numberLevelsOnWorld;
	/// <summary>
	/// The max score of levels
	/// </summary>
	[SerializeField]
	private ArrayInt[] maxScoreLevels;
	/// <summary>
	/// The achieved score levels.
	/// </summary>
	private int [][] _achievedScoreLevels;

	/// <summary>
	/// Gets the score.
	/// </summary>
	/// <returns>The score.</returns>
	/// <param name="level">Level.</param>
	public ScoreLevel GetScore(LevelInfo level){
		ScoreLevel result;
		result.max = maxScoreLevels[level.world][level.level];
		result.achieved = _achievedScoreLevels [level.world] [level.level];
		return result;
	}

	/// <summary>
	/// Gets the max score.
	/// </summary>
	/// <returns>The max score.</returns>
	/// <param name="level">Level.</param>
	public int GetMaxScore(LevelInfo level){
		return maxScoreLevels[level.world][level.level];
	}

	/// <summary>
	/// Gets the score achieved.
	/// </summary>
	/// <returns>The score achieved.</returns>
	/// <param name="level">Level.</param>
	public int GetScoreAchieved(LevelInfo level){
		return _achievedScoreLevels[level.world][level.level];
	}

	/// <summary>
	/// Sets the achieved score.
	/// </summary>
	/// <param name="score">Score.</param>
	/// <param name="level">Level.</param>
	public void SetAchievedScore(int score, LevelInfo level){
		if (score < 0) return;
		_achievedScoreLevels[level.world][level.level]=score;
		StoreScoreLevels();
	}

	/// <summary>
	/// Stores the score levels.
	/// </summary>
	private void StoreScoreLevels(){
		StringBuilder builder = new StringBuilder();
		int numberWorld = numberLevelsOnWorld.Length;
		for(int i=0; i<numberWorld; i++) {
			int numberLevels = numberLevelsOnWorld[i];
			for(int j=0; j<numberLevels; j++) {
				int score = _achievedScoreLevels[i][j];
				if (score!= 0) {
					builder.Append(";" + i + "-" + j + "-" + score);
				}
			}
		}

		if (builder.Length > 0) {
			builder.Remove (0, 1);
		}

		PlayerPrefs.SetString (PlayerDataStoreKeys.PlayerScoreLevels, builder.ToString());
		PlayerPrefs.Save ();
	}

	/// <summary>
	/// Sets the score levels.
	/// </summary>
	public void SetScoreLevels(){
		string scores = PlayerPrefs.GetString(PlayerDataStoreKeys.PlayerScoreLevels);
		_achievedScoreLevels = new int[numberLevelsOnWorld.Length][];
		//put every at 0 drop achieved
		int numberWorld = numberLevelsOnWorld.Length;
		for(int i=0; i<numberWorld; i++) {
			int numberLevels = numberLevelsOnWorld[i];
			_achievedScoreLevels[i]= new int[numberLevels];
			for(int j=0; j<numberLevels; j++) {
				_achievedScoreLevels[i][j]=0;
			}
		}

		//get the levels and fill
		if(scores.Length==0) return;
		string[] levels = scores.Split(';');

		for(int i=0;i <levels.Length; i++) {
			string[] aLevel = levels[i].Split('-');
			int world = int.Parse(aLevel[0]);
			int level = int.Parse(aLevel[1]);
			int score = int.Parse(aLevel[2]);
			_achievedScoreLevels[world][level]=score;
		}
	}
}
