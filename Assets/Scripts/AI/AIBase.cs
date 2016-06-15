using UnityEngine;
using System.Collections.Generic;

public class AIBase : MonoBehaviour {
    #region Public Attribute
    public CommonParameters commonParameters;
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
    /// <summary>
    /// Parameters for the attack state
    /// </summary>
    public AttackParameters attackParameters;
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
    /// The size of the detected drop
    /// </summary>
    protected CharacterSize _sizeDetected;
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
        _animator = commonParameters.enemy.GetComponent<Animator>();
        _animator.SetInteger("LimitSizeDrop", commonParameters.sizeLimitDrop);
        commonParameters.rootEntityPosition = transform;
        commonParameters.initialPositionEnemy = commonParameters.enemy.transform.position;
        commonParameters.initialRotationEnemy = commonParameters.enemy.transform.rotation;
        commonParameters.AI = this;
    }

    public void Start() {
        //get the behaviours and set their data
        Walking walkingAI = _animator.GetBehaviour<Walking>();
        walkingAI.commonParameters = commonParameters;
        walkingAI.parameters = walkingParameters;

        GoAway runningAway = _animator.GetBehaviour<GoAway>();
        runningAway.commonParameters = commonParameters;
        runningAway.parameters = goAwayParameters;

        DetectPlayer detectedAI = _animator.GetBehaviour<DetectPlayer>();
        detectedAI.parameters = detectParameters;
        detectedAI.commonParameters = commonParameters;

        Iddle iddle= _animator.GetBehaviour<Iddle>();
        iddle.parameters = iddleParameters;
        iddle.commonParameters = commonParameters;

        _chaseAI = _animator.GetBehaviour<Chase>();
        _chaseAI.commonParameters = commonParameters;
        _chaseAI.parameters = chaseParameters;

        Attack attackAI = _animator.GetBehaviour<Attack>();
        attackAI.commonParameters = commonParameters;
        attackAI.parameters = attackParameters;
    }

    public void Update() {
        //check if any drop inside the area
        CheckDropInside();
        //check if too near
        DropNear();
    }

    public void Scare() {
        _animator.SetBool("GoAway", true);
    }

    public void OnDrawGizmosSelected() {
        //draw paths
        goAwayParameters.endPoint.OnDrawGizmos();
        if (commonParameters.walking) {
            if (!walkingParameters.usePath) {
                walkingParameters.walkArea.OndrawGizmos(Color.blue, transform.position);
            } else {
                walkingParameters.path.OnDrawGizmos();
            }
        }

        Vector3 size = Vector3.one;
        size *= commonParameters.near;
        size.y = triggerArea.size.y;
        Vector3 origin = triggerArea.origin+transform.position;
        origin.x = commonParameters.enemy.transform.position.x;
        origin.y += size.y / 2.0f;
        paint(Color.green, origin, size);
    }

    /// <summary>
    /// Draws the trigger area and paths
    /// </summary>
    public void OnDrawGizmos() {
        triggerArea.OndrawGizmos(Color.red, transform.position);

        attackParameters.launchDestination.OnDrawGizmos();
    }

    #region Private methods
    private void CheckDropInside() {
        //Get the collisions/trigger area
        LayerMask layerCast = (1 << LayerMask.NameToLayer("Character"));
        Collider[] drops = AIMethods.DropInTriggerArea(triggerArea, transform.position, layerCast);

        //if we have a drop detected, check only him if is outside the trigger or not
        if (commonParameters.drop == null) {
            int sizeDrop = 0;
            foreach (Collider dropCollider in drops) {
                if (_independentControl.IsUnderControl(dropCollider.gameObject)) {
                    commonParameters.drop = dropCollider.gameObject;
                    _sizeDetected = commonParameters.drop.GetComponent<CharacterSize>();
                    sizeDrop = _sizeDetected.GetSize();
                }
            }
            _animator.SetInteger("SizeDrop", sizeDrop);

        } else {
            //update his size
            int sizeDrop = 0;
            sizeDrop = _sizeDetected.GetSize();


            //check if is outside of trigger
            bool outside = true;
            foreach (Collider dropCollider in drops) {
                if (dropCollider.gameObject == commonParameters.drop) {
                    outside = false;
                }
            }


            //it's outside!
            if (outside) {
                _animator.SetInteger("SizeDrop", 0);
                commonParameters.drop = null;
            } else {
                _animator.SetInteger("SizeDrop", sizeDrop);
            }
        }
    }

    /// <summary>
    /// Check if a drop is too near of the enemy
    /// </summary>
    private void DropNear() {
        LayerMask layerCast = (1 << LayerMask.NameToLayer("Character"));
        Vector3 size = Vector3.one;
        size *= commonParameters.near;
        size.y = triggerArea.size.y;

        size /= 2.0f;
        Vector3 origin = triggerArea.origin + transform.position;
        origin.x = commonParameters.enemy.transform.position.x;
        origin.y += size.y;
        origin.z = 0;

        Collider[] drops = Physics.OverlapBox(origin, size, 
            Quaternion.identity, layerCast, QueryTriggerInteraction.Ignore);

        if (drops.Length==0){
            return;
        }

        List<Collider> dropsInTriggerArea = new List<Collider>(AIMethods.DropInTriggerArea(triggerArea, transform.position, layerCast));
        commonParameters.drop = drops[0].gameObject;
        bool near = false;
        foreach(Collider aCollider in drops){
            if (dropsInTriggerArea.Contains(aCollider)) {
                commonParameters.drop = aCollider.gameObject;
                near = true;
                break;
            }
        }

        if (near) {
            _animator.SetBool("Near", true);
            _sizeDetected = commonParameters.drop.GetComponent<CharacterSize>();
            _animator.SetInteger("SizeDrop", _sizeDetected.GetSize());
        }
    }

    private void paint(Color color,Vector3 origin, Vector3 size) {
        Gizmos.color = color;
        //draw area
        Vector3 originWorld = origin;

        Vector3 bottomLeft = originWorld - (size/2.0f);
        Vector3 topLeft = bottomLeft;
        topLeft.y += size.y;
        Vector3 topRight = topLeft;
        topRight.x += size.x;
        Vector3 bottomRight = bottomLeft;
        bottomRight.x += size.x;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        
}
    #endregion

    #endregion
    
   

    
}
