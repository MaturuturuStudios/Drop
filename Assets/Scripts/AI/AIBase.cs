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
        //Get the collisions/trigger area
        LayerMask layerCast = (1 << LayerMask.NameToLayer("Character"));
        Vector3 center = triggerArea.origin + transform.position;
        Vector3 halfSize = triggerArea.size / 2;
        center.x += halfSize.x;
        center.y += halfSize.y;
        center.z += halfSize.z;
        Collider[] drops = Physics.OverlapBox(center, halfSize, Quaternion.identity, layerCast, QueryTriggerInteraction.Ignore);

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
        //check if too near
        DropNear();
    }

    public void Scare() {
        _animator.SetBool("GoAway", true);
    }

    public void OnDrawGizmosSelected() {
        //draw paths
        goAwayParameters.endPoint.OnDrawGizmos();

        if (!walkingParameters.usePath) {
            walkingParameters.walkArea.OndrawGizmos(Color.blue, transform.position);
        } else {
            walkingParameters.path.OnDrawGizmos();
        }
    }

    /// <summary>
    /// Draws the trigger area and paths
    /// </summary>
    public void OnDrawGizmos() {
        triggerArea.OndrawGizmos(Color.red, transform.position);

        attackParameters.launchDestination.OnDrawGizmos();
    }

    #region Private methods
    private void DropNear() {
        LayerMask layerCast = (1 << LayerMask.NameToLayer("Character"));
        Vector3 size = Vector3.one;
        size *= commonParameters.near;
        Collider[] drops = Physics.OverlapBox(commonParameters.enemy.transform.position, size, 
            Quaternion.identity, layerCast, QueryTriggerInteraction.Ignore);

        if (drops.Length==0){
            return;
        }

        commonParameters.drop = drops[0].gameObject;
        _animator.SetBool("Near", true);
        _sizeDetected = commonParameters.drop.GetComponent<CharacterSize>();
        _animator.SetInteger("SizeDrop", _sizeDetected.GetSize());
    }
    #endregion

    #endregion
}
