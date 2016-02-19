using UnityEngine;
using System.Collections;

public class CameraControler : MonoBehaviour {

    private GameObject currentCharacter;

    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(0.0f, 2.0f, -10.0f);
    }

	void LateUpdate () {
        transform.position = currentCharacter.transform.position + offset;	
	}

    public void setObjective(GameObject objective)
    {
        currentCharacter = objective;
    }

}
