using UnityEngine;
using System.Collections;

public class ButtonDoor : MonoBehaviour {
    public GameObject Door;

    void OnTriggerEnter(Collider other) {
        Door.transform.Translate(new Vector3(0,0,5));
    }

    void OnTriggerExit(Collider other) {
        Door.transform.Translate(new Vector3(0, 0, -5));
    }
}
