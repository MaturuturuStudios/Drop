using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
    /// <summary>
    /// The particle effect for max score achieved
    /// </summary>
    public GameObject particleMaxScoreAchieved;
    /// <summary>
    /// The instantiated particles
    /// </summary>
    private GameObject _instantiatedParticles;

    /// <summary>
    /// Set the score
    /// </summary>
    /// <param name="max">max score available</param>
    /// <param name="achieved">score achieved</param>
	public void SetScore(float max, float achieved=0){
		TextMesh text = GetComponent<TextMesh> ();
		text.text = achieved + "/" + max;

        SetParticleScore(achieved >= max);
	}

    /// <summary>
    /// Set the particles
    /// </summary>
    /// <param name="achieved"></param>
    private void SetParticleScore(bool achieved) {
        if (!achieved) {
            if (_instantiatedParticles == null) return;
            else Destroy(_instantiatedParticles);

        } else {
            _instantiatedParticles = Instantiate(particleMaxScoreAchieved);
            Vector3 localPosition = _instantiatedParticles.transform.localPosition;
            Quaternion localRotation = _instantiatedParticles.transform.localRotation;
            Vector3 localScale = _instantiatedParticles.transform.localScale;
            _instantiatedParticles.transform.parent = this.transform;
            _instantiatedParticles.transform.localPosition = localPosition;
            _instantiatedParticles.transform.localScale = localScale;
            _instantiatedParticles.transform.localRotation = localRotation;
        }
    }
}
