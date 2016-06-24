using UnityEngine;

[RequireComponent(typeof(Animation))]
public class LocalAnimation : MonoBehaviour {
    Vector3 localPos;
    bool wasPlaying;

    void Awake() {
        localPos = transform.position;
        wasPlaying = false;
    }

    void LateUpdate() {
        if (!GetComponent<Animation>().isPlaying || !wasPlaying)
            return;

        transform.localPosition += localPos;

        wasPlaying = GetComponent<Animation>().isPlaying;
    }
}