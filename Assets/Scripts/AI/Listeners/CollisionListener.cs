using UnityEngine;

public interface CollisionListener {
    void OnTriggerEnter(Collider other);
    void OnTriggerStay(Collider other);
}
