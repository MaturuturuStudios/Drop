using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	public void SetScore(float max, float achieved=0){
		TextMesh text = GetComponent<TextMesh> ();
		text.text = achieved + "/" + max;
	}
}
