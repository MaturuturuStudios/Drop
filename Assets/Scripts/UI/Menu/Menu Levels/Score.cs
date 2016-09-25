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
    /// Material for the text score when max is achieved
    /// </summary>
    public Color levelCompletedColorText;
    /// <summary>
    /// Original color for the text score
    /// </summary>
    private Color originalColor;
    /// <summary>
    /// reference to the material
    /// </summary>
    private MeshRenderer text;

    /// <summary>
    /// TODO cambiar por material
    /// </summary>
    public void Awake() {
        text = GetComponent<MeshRenderer>();
        originalColor = text.material.color;
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
            text.material.color = originalColor;

        } else if (achieved == max) {
            text.material.color = levelCompletedColorText;
            levelCompleted.SetActive(true);

        } else {
            levelPlayed.SetActive(true);
        }
    }
}
