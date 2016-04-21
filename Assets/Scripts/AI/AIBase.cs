﻿using UnityEngine;
using System.Collections.Generic;

public class AIBase : MonoBehaviour {
    #region Public Attribute
    /// <summary>
    /// The enemy to control
    /// </summary>
    public GameObject enemy;
    /// <summary>
    /// Is it a grounded entity? or maybe fly?
    /// </summary>
    public bool onFloor;
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
    /// Area in which enemy react
    /// </summary>
    public Region triggerArea;
    /// <summary>
    /// Parameters for the go Away state
    /// </summary>
    public GoAwayParameters goAwayParameters;
    /// <summary>
    /// Parameters for the walking state
    /// </summary>
    public WalkingParameters walkingParameters;
    /// <summary>
    /// Parameters for the iddle state
    /// </summary>
    public IddleParameters iddleParameters;
    /// <summary>
    /// Parameters for the chase state
    /// </summary>
    public ChaseParameters chaseParameters;
    /// <summary>
    /// Parameters for the detection state
    /// </summary>
    public DetectParameters detectParameters;
    #endregion

    #region Private attributes
    /// <summary>
    /// Independent control to create or remove drops
    /// </summary>
    protected GameControllerIndependentControl _independentControl;
    /// <summary>
    /// The animation which has the state machine
    /// </summary>
    protected Animator _animator;
    /// <summary>
    /// The detected drop
    /// </summary>
    protected GameObject _detectedDrop;
    /// <summary>
    /// The size of the detected drop
    /// </summary>
    protected CharacterSize _sizeDetected;
    /// <summary>
    /// TODO: needed?
    /// </summary>
    protected Attack attackAI;
    /// <summary>
    /// The chase AI reference to send him the detected and chased drop
    /// </summary>
    protected Chase _chaseAI;
    #endregion

    #region Methods
    /// <summary>
    /// Initialization method.
    /// The character start with size one
    /// </summary>
    public void Awake() {
        //get the independ control to know if the detected drop is under control
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController)
                                .GetComponent<GameControllerIndependentControl>();
        //get the animator of the enemy
        _animator = enemy.GetComponent<Animator>();
        _animator.SetInteger("LimitSizeDrop", sizeLimitDrop);
    }

    public void Start() {
        //get the behaviours and set their data
        Walking walkingAI = _animator.GetBehaviour<Walking>();
        walkingAI.parameters.enemy = enemy;
        walkingAI.parameters.timeUntilIddle = walkingParameters.timeUntilIddle;
        walkingAI.parameters.onFloor = onFloor;
        walkingAI.parameters.path = walkingParameters.path;
        walkingAI.parameters.followType = walkingParameters.followType;
        walkingAI.parameters.speed = walkingParameters.speed;
        walkingAI.parameters.maxDistanceToGoal= walkingParameters.maxDistanceToGoal;
        walkingAI.parameters.useOrientation= walkingParameters.useOrientation;
        walkingAI.parameters.pathType = walkingParameters.pathType;

        GoAway runningAway = _animator.GetBehaviour<GoAway>();
        runningAway.parameters.enemy = enemy;
        runningAway.parameters.endPoint = goAwayParameters.endPoint;
        runningAway.parameters.onFloor = onFloor;

        DetectPlayer detectedAI = _animator.GetBehaviour<DetectPlayer>();
        detectedAI.parameters.timeWarningDetect = detectParameters.timeWarningDetect;

        Iddle iddle= _animator.GetBehaviour<Iddle>();
        iddle.parameters.timeInIddle = iddleParameters.timeInIddle;

        _chaseAI = _animator.GetBehaviour<Chase>();
        _chaseAI.parameters.enemy = enemy;
        _chaseAI.parameters.speed = chaseParameters.speed;

        attackAI = _animator.GetBehaviour<Attack>();
    }

    public void Update() {
        //Get the collisions/trigger area
        LayerMask layerCast = (1 << LayerMask.NameToLayer("Character")); ;
        Vector3 center = triggerArea.origin + transform.position;
        Vector3 halfSize = triggerArea.size / 2;
        center.x += halfSize.x;
        center.y += halfSize.y;
        center.z += halfSize.z;
        Collider[] drops = Physics.OverlapBox(center, halfSize, Quaternion.identity, layerCast, QueryTriggerInteraction.Ignore);

        //if we have a drop detected, check only him if is outside the trigger or not
        if (_detectedDrop == null) {
            int sizeDrop = 0;
            foreach (Collider dropCollider in drops) {
                if (_independentControl.IsUnderControl(dropCollider.gameObject)) {
                    _detectedDrop = dropCollider.gameObject;
                    _sizeDetected = _detectedDrop.GetComponent<CharacterSize>();
                    _chaseAI.parameters.target = _detectedDrop;
                    sizeDrop = _sizeDetected.GetSize();
                }
            }
            _animator.SetInteger("SizeDrop", sizeDrop);

        } else {
            //check if is outside of trigger
            bool outside = true;
            foreach (Collider dropCollider in drops) {
                if (dropCollider.gameObject==_detectedDrop) {
                    outside = false;
                }
            }
            //it's outside!
            if (outside) {
                _animator.SetInteger("SizeDrop", 0);
                _detectedDrop = null;
            }
            //check if reached the drop
            DropReached();
        }
    }
    

    private void DropReached() {
        if (_detectedDrop == null) {
            return;
        }

        //check distance TODO adjust 
        // Checks if the entity is close enough to the target point
        float squaredDistance = (_detectedDrop.transform.position - enemy.transform.position).sqrMagnitude;
        // The squared distance is used becouse a multiplication is cheaper than a square root
        float distanceTolerance= _sizeDetected.GetSize();
        distanceTolerance += toleranteDistanceAttack;
        distanceTolerance *= distanceTolerance;
        if (squaredDistance < distanceTolerance)
            _animator.SetBool("Reached", true);
    }
    #endregion

    #region Personal methods and class
    /// <summary>
    /// Region definition
    /// </summary>
    [System.Serializable]
    public class Region {
        public Vector3 origin = Vector3.one;
        public Vector3 size = Vector3.one;
    }

    /// <summary>
    /// Draws the trigger area and paths
    /// </summary>
    public void OnDrawGizmos() {
        //draw paths
        walkingParameters.path.OnDrawGizmos();
        goAwayParameters.endPoint.OnDrawGizmos();

        //draw area
        Gizmos.color = Color.red;

        Vector3 originWorld = triggerArea.origin + transform.position;

        Vector3 bottomLeft = originWorld;
        Vector3 topLeft = originWorld;
        topLeft.y += triggerArea.size.y;
        Vector3 topRight = topLeft;
        topRight.x += triggerArea.size.x;
        Vector3 bottomRight = bottomLeft;
        bottomRight.x += triggerArea.size.x;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);

        Vector3 deep = Vector3.zero;
        deep.z = triggerArea.size.z;
        Gizmos.DrawLine(topLeft, topLeft + deep);
        Gizmos.DrawLine(topRight, topRight + deep);
        Gizmos.DrawLine(bottomRight, bottomRight + deep);
        Gizmos.DrawLine(bottomLeft, bottomLeft + deep);

        topLeft += deep;
        topRight += deep;
        bottomLeft += deep;
        bottomRight += deep;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);
    }
    #endregion
}
