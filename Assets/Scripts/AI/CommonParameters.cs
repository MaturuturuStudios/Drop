using UnityEngine;
using System.Collections;

[System.Serializable]
public class CommonParameters {
    /// <summary>
    /// The enemy to control
    /// </summary>
    public GameObject enemy;
    [HideInInspector]
    /// <summary>
    /// The entity to be chased
    /// </summary>
    public GameObject drop;
    /// <summary>
    /// Is it a grounded entity? or maybe fly?
    /// </summary>
    public bool onFloor;
    /// <summary>
    /// Speed of rotation when moving
    /// </summary>
    public float RotationSpeed;
    /// <summary>
    /// Under this limit, enemy attack, equal or over this size, the enemy escape
    /// if zero or less, the size is ignored (example of bee)
    /// </summary>
    public int sizeLimitDrop;
    /// <summary>
    /// Distance tolerance of the enemy to take in account when calculate if reached the drop
    /// The size of the drop is alreaded taked in account
    /// </summary>
    public float toleranteDistanceAttack = 1.0f;
    /// <summary>
    /// Distance considered too much near to the enemy
    /// </summary>
    public float near = 2f;
    [HideInInspector]
    public Transform rootEntityPosition;
}
