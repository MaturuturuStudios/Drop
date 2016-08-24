using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
    public GameObject follow;

	// Use this for initialization
	void Awake() {
        transform.position = follow.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = follow.transform.position;
	}
}
