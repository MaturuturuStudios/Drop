using UnityEngine;

public class TimeScaler : MonoBehaviour {

	public float timeScale = 1;

	void Update() {
		Time.timeScale = timeScale;
	}
}
