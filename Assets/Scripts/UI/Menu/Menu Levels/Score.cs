using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
    /// <summary>
    /// Effect to show when the level has played and achieved the max score
    /// </summary>
    public GameObject levelCompleted;
    /// <summary>
    /// Effect to show when the level has played
    /// </summary>
    public GameObject levelPlayed;

    /// <summary>
    /// 
    /// </summary>
    public void Awake() {
        levelPlayed.SetActive(false);
        levelCompleted.SetActive(false);
    }

    /// <summary>
    /// Set the score
    /// </summary>
    /// <param name="max">max score available</param>
    /// <param name="achieved">score achieved</param>
	public void SetScore(int max, int achieved=0){
		TextMesh text = GetComponent<TextMesh> ();
		text.text = achieved + "/" + max;

        SetParticleScore(achieved, max);
	}

    /// <summary>
    /// Set the particles
    /// </summary>
    /// <param name="achieved"></param>
    private void SetParticleScore(int achieved, int max) {
        if (achieved <= 0) {
            levelPlayed.SetActive(false);
            levelCompleted.SetActive(false);
        }else if (achieved == max) {
            levelCompleted.SetActive(true);
        } else {
            levelPlayed.SetActive(true);
        }
    }
}
