using UnityEngine;
using System.Collections;

[System.Serializable]
public class CommonParameters {
    /// <summary>
    /// The enemy to control
    /// </summary>
    public GameObject enemy;
    
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
    /// Distance tolerance for the entity to target (except in attack)
    /// </summary>
    public float toleranceDistanceToGoal = 0.1f;
    /// <summary>
    /// Distance tolerance of the enemy to take in account when calculate if reached the drop
    /// The size of the drop is alreaded taked in account
    /// </summary>
    public float toleranceDistanceAttack = 1.0f;
    /// <summary>
    /// Tolerance degree to target rotation
    /// </summary>
    public float toleranceDegreeToGoal = 5f;
    /// <summary>
    /// Distance considered too much near to the enemy
    /// </summary>
    public float near = 2f;
    /// <summary>
    /// True if the enemy will patrol/walk
    /// false if will be fixed at its point
    /// The first point of the path walking will be used
    /// if the enemy is not in that point, will move to it
    /// after iddle
    /// </summary>
    public bool walking = true;


    [HideInInspector]
    /// <summary>
    /// The entity to be chased
    /// </summary>
    public GameObject drop;
    /// <summary>
    /// Position of the game object, needed in random points area
    /// </summary>
    [HideInInspector]
    public Transform rootEntityPosition;
    /// <summary>
    /// Initial position of enemy
    /// </summary>
    [HideInInspector]
    public Vector3 initialPositionEnemy;
    /// <summary>
    /// Initial rotation of enemy
    /// </summary>
    [HideInInspector]
    public Quaternion initialRotationEnemy;
    /// <summary>
    /// Reference to the main script
    /// </summary>
    [HideInInspector]
    public AIBase AI;

    [HideInInspector]
    public AIColliders colliders;

    [HideInInspector]
    public float minimumWalkingDistance;
}
