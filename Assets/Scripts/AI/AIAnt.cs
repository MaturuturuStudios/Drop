using UnityEngine;
using System.Collections.Generic;

public class AIAnt : AIBase {
    /// <summary>
    /// A reference to the path this entity will follow.
    /// </summary>
    public PathDefinition path;
    /// <summary>
    /// Defines how will the entity move to the next point in the path.
    /// </summary>
    public FollowType followType = FollowType.MoveTowards;
    /// <summary>
	/// Speed of the entity.
	/// </summary>
	public float speed = 10;
    /// <summary>
    /// Distance tolerance for the entity to look for a new point in the path.
    /// </summary>
    public float maxDistanceToGoal = 0.1f;
    /// <summary>
    /// If enabled, the entity will also rotate to fit the point's rotation.
    /// </summary>
    public bool useOrientation = false;
    /// <summary>
    /// Defines how the entity looks for the next point in the path.
    /// </summary>
    public PathType pathType = PathType.Random;

    public new void Start() {
        //make base start
        base.Start();
        //get the behaviours and set their data
        Walking walking = _animator.GetBehaviour<Walking>();
        walking.path = path;
        walking.followType = followType;
        walking.speed = speed;
        walking.maxDistanceToGoal = maxDistanceToGoal;
        walking.useOrientation = useOrientation;
        walking.pathType = pathType;
    }
}
