using UnityEngine;
using System.Collections.Generic;

public class AIColliders : MonoBehaviour {
    private List<CollisionListener> _listeners = new List<CollisionListener>();

    /// <summary>
    /// Subscribes a listener to the collisions's events.
    /// Returns false if the listener was already subscribed.
    /// </summary>
    /// <param name="listener">The listener to subscribe</param>
    /// <returns>If the listener was successfully subscribed</returns>
    public bool AddListener(CollisionListener listener) {
        if (_listeners.Contains(listener))
            return false;
        _listeners.Add(listener);
        return true;
    }

    /// <summary>
    /// Unsubscribes a listener to the collisions's events.
    /// Returns false if the listener wasn't subscribed yet.
    /// </summary>
    /// <param name="listener">The listener to unsubscribe</param>
    /// <returns>If the listener was successfully unsubscribed</returns>
    public bool RemoveListener(CollisionListener listener) {
        if (!_listeners.Contains(listener))
            return false;
        _listeners.Remove(listener);
        return true;
    }

    public void OnTriggerEnter(Collider other) {
        // Notifies the listeners
        foreach (CollisionListener listener in _listeners)
            listener.OnTriggerEnter(other);
    }

    public void OnTriggerStay(Collider other) {
        // Notifies the listeners
        foreach (CollisionListener listener in _listeners)
            listener.OnTriggerStay(other);
    }
}
