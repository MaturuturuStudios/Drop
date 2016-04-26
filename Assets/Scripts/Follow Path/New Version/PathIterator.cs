using UnityEngine;


/// <summary>
/// Moves an entity according to a PathDefinition script.
/// </summary>
public class PathIterator  : MonoBehaviour {

    #region Custom Enumerations

    /// <summary>
    /// Defines how will the entity move to the next point in the path.
    /// </summary>
    public enum FollowType {

        /// <summary>
        /// Moves towards the points at constant speed.
        /// </summary>
        MoveTowards,

        /// <summary>
        /// Moves towards the points at decreasing speed.
        /// </summary>
        Lerp,

        /// <summary>
        /// Moves towards the points at constant time
        /// </summary>
        Time
    }
    #endregion

    #region Public Attributes


    /// <summary>
    /// Set this variable to true for run
    /// </summary>
    public bool isActive = false;

    /// <summary>
    /// Defines how will the entity move to the next point in the path.
    /// </summary>
    public FollowType followType = FollowType.MoveTowards;

	/// <summary>
	/// A reference to the path this entity will follow.
	/// </summary>
	public SimplePathDefinition path;

    /// <summary>
    /// Speed of the entity.
    /// </summary>
    public float speed = 10;

    /// <summary>
    /// Distance tolerance for the entity to look for a new point in the path.
    /// </summary>
    public float maxDistanceToGoal = 0.1f;

    /// <summary>
    /// Duration of the transitions between points
    /// </summary>
    [Range(0,10)]
    public float transitionDuration = 1F;

    /// <summary>
    /// If enabled, the entity will also rotate to fit the point's rotation.
    /// </summary>
    public bool useOrientation = false;

    /// <summary>
    /// Set the step that you want to start
    /// </summary>
    public int startStep = 0;

    /// <summary>
    /// Step by step mode, enable it and it will stop every step
    /// </summary>
    [HideInInspector]
    public bool stepByStep = false;

    /// <summary>
    /// Set the number of steps that you want to move
    /// </summary>
    [HideInInspector]
    public int numberOfSteps = 1;

    /// <summary>
    /// Set it true if you want to jump "numberOfSteps" positions
    /// </summary>
    [HideInInspector]
    public bool directly = false;

    #endregion

    #region Private Attributes

    /// <summary>
    /// A reference to the entity's transform.
    /// </summary>
    private Transform _transform;

    /// <summary>
    /// reference to know when we have to stop moving
    /// </summary>
    private int _stepsToStop = 1;

    /// <summary>
    /// Elapsed time of the current transitions between points
    /// </summary>
    private float elapsedTransition = 0;

    /// <summary>
    /// Elapsed time of the current transitions between points
    /// </summary>
    private Vector3 _previousPosition;

    #endregion

    #region Methods

    /// <summary>
    /// Unity's method called at the first frame this entity is enabled.
    /// Retrieves all the desired components and initialices the platform.
    /// </summary>
    public void Start() {
		// A path is required
		if (path == null || path.points.Count < 1) {
			Debug.LogError("Path cannot be null or empty!", gameObject);
			return;
		}

        // Recovers the transform component
        _transform = transform;

        //Clamp input values
        startStep = Mathf.Clamp(startStep, 0, path.points.Count - 1);
        numberOfSteps = Mathf.Clamp(numberOfSteps, 1, path.points.Count - 1);

        //calculate steps to stop
        _stepsToStop = 1;

        // Get the desire path
        _transform.position = path.MoveAt(startStep).position;
    }

	/// <summary>
	/// Unity's method called every frame.
	/// Moves the platform close to the next point in the path and checks
	/// if it's close enough to it.
	/// </summary>
	public void FixedUpdate() {

        if (!isActive)
            return;

        // Saves the original position
        Vector3 originalPosition = _transform.position;
        float percent;
        // Moves the entity using the right function
        switch (followType) {
			case FollowType.MoveTowards:
				_transform.position = Vector3.MoveTowards(_transform.position, path.GetCurrent().position, speed * Time.deltaTime);
				break;
            case FollowType.Lerp:
                _transform.position = Vector3.Lerp(_transform.position, path.GetCurrent().position, speed * Time.deltaTime);
                break;
            case FollowType.Time:
                elapsedTransition += Time.deltaTime;
                percent = (elapsedTransition / transitionDuration);
                _transform.position = Vector3.Lerp( _previousPosition, path.GetCurrent().position, percent);
                break;
            default:
				return;
		}

		// Rotates the entity
		if (useOrientation) {
			float traveledDistance = (_transform.position - originalPosition).magnitude;
			float remainingDistance = (path.GetCurrent().position - originalPosition).magnitude;
			if (remainingDistance > 0.01f)
				_transform.rotation = Quaternion.Lerp(_transform.rotation, path.GetCurrent().rotation, traveledDistance / remainingDistance);
		}

		// Checks if the entity is close enough to the target point
		float squaredDistance = (_transform.position - path.GetCurrent().position).sqrMagnitude;
        // The squared distance is used becouse a multiplication is cheaper than a square root
        if (transitionDuration < elapsedTransition || squaredDistance < maxDistanceToGoal * maxDistanceToGoal) {
            // Move next
            isActive = false;
            //NextStep();

            elapsedTransition = 0;

        }

    }

    /// <summary>
    /// Method for change comportament depending of the path type.
    /// </summary>
    public void NextStep() {

        isActive = true;

        elapsedTransition = 0;

        _previousPosition = _transform.position;

        int nextStep = numberOfSteps;

        if (!directly)
            nextStep = 1;
        
        path.MoveNext(numberOfSteps);

        /*
        // Ask for stop
        if (stepByStep && (directly || (!directly && _stepsToStop == 0))) {
            if (!directly)
                _stepsToStop = numberOfSteps;
            isActive = false;
        }*/
    }


    /// <summary>
    /// Method for change comportament depending of the path type.
    /// </summary>
    public void PrevStep() {

        isActive = true;

        elapsedTransition = 0;

        _previousPosition = _transform.position;

        int nextStep = numberOfSteps;

        if (!directly)
            nextStep = 1;

        path.MovePrevious(numberOfSteps);
        /*
        // Ask for stop
        if (stepByStep && (directly || (!directly && _stepsToStop == 0))) {
            if (!directly)
                _stepsToStop = numberOfSteps;
            isActive = false;
        }*/
    }

    #endregion
}
