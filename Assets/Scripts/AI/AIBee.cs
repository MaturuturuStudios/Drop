using UnityEngine;
using System.Collections;

public class AIBee : MonoBehaviour {
    public Collider AreaTrigger;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider other) {
        //fuerte dependencia con DetectPlayer
        //encima no sería a este collider!!!
        //DetectPlayer.OnTriggerEnter(AreaTrigger, other);
    }
}
