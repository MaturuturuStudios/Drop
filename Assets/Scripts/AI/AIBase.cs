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

    private AnimationState actualState;

    [HideInInspector]
    public List<EnemyBehaviourListener> listeners = new List<EnemyBehaviourListener>();
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

        Iddle iddle = _animator.GetBehaviour<Iddle>();
        iddle.parameters = iddleParameters;
        iddle.commonParameters = commonParameters;

        _chaseAI = _animator.GetBehaviour<Chase>();
        _chaseAI.commonParameters = commonParameters;
        _chaseAI.parameters = chaseParameters;

        Attack attackAI = _animator.GetBehaviour<Attack>();
        attackAI.commonParameters = commonParameters;
        attackAI.parameters = attackParameters;

        actualState = GetAnimationState(_animator.GetCurrentAnimatorStateInfo(0));
    }

    public void Update() {
        //check if any drop inside the area
        CheckDropInside();

        //check changes on animation state
        StateListener();
    }

    /// <summary>
    /// Subscribes a listener to the animation's events.
    /// Returns false if the listener was already subscribed.
    /// </summary>
    /// <param name="listener">The listener to subscribe</param>
    /// <returns>If the listener was successfully subscribed</returns>
    public bool AddListener(EnemyBehaviourListener listener) {
        if (listeners.Contains(listener))
            return false;
        listeners.Add(listener);
        return true;
    }

    /// <summary>
    /// Unsubscribes a listener to the animation's events.
    /// Returns false if the listener wasn't subscribed yet.
    /// </summary>
    /// <param name="listener">The listener to unsubscribe</param>
    /// <returns>If the listener was successfully unsubscribed</returns>
    public bool RemoveListener(EnemyBehaviourListener listener) {
        if (!listeners.Contains(listener))
            return false;
        listeners.Remove(listener);
        return true;
    }

    public void Scare() {
        _animator.SetBool("GoAway", true);
    }

    public void TriggerListener(Collider other) {
        if (other.gameObject.tag == Tags.Player) {
            if (_animator == null) return;
            _animator.SetBool("Reached", true);
        }
    }

    public AnimationState GetActualState() {
        return actualState;
    }

    public void OnDrawGizmosSelected() {
        //draw paths
        if(!goAwayParameters.endMoving)
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

        attackParameters.launcher.OnDrawGizmos();
    }

    #region Private methods


    /// <summary>
    /// Check the drops inside the area and update their state
    /// </summary>
    protected void CheckDropInside() {
        //Get the collisions/trigger area
        LayerMask layerCast = (1 << LayerMask.NameToLayer("Character"));
        Collider[] drops = AIMethods.DropInTriggerArea(triggerArea, transform.position, layerCast);

        int sizeDrop = 0;

        //if no drops in trigger area or not in the list, no priority drops
        if (drops.Length == 0) commonParameters.priorityDrop = false;

        //si la gota prioritaria está fuera del trigger y hay otra dentro, al momento
        //del ataque, se expulsa a la otra gota
        //ir a dentro del for, y comprobar que la prioritaria está dentro del trigger o ya está expulsada

        //priority drop
        // 1. the drop in air trigger (usually is controlled)
        // 2. drop inside, preference by distance
        // 3. if selected drop is near, attack it 

        //clear drop if no priority was setted
        if (!commonParameters.priorityDrop) commonParameters.drop = null;
        //delete priority
        commonParameters.priorityDrop = false;

        //keep track of drop index
        int dropIndex = -1;
        float distance = float.MaxValue;
        bool hasPriority = false;

        //check the list of drop inside the trigger area
        for (int i = 0; i < drops.Length && !hasPriority; i++) {
            Collider dropCollider = drops[i];

            //if has a drop (priority drop) and found it, we have a drop priority
            //end loop
            if (commonParameters.drop != null && drops[i].gameObject == commonParameters.drop) {
                hasPriority = true;
            }

            //if drop is under controll, check the distance and get the closest drop
            if (_independentControl.IsUnderControl(dropCollider.gameObject)) {
                float newDistance = Vector2.Distance(dropCollider.transform.position,
                                                commonParameters.enemy.transform.position);
                if (newDistance > distance) continue;
                distance = newDistance;
                dropIndex = i;
            }
        }


        //if we have a drop and no found a priority drop
        //because there is no one, or the priority drop is outside
        if (!hasPriority && dropIndex >= 0) {
            //store the drop
            commonParameters.drop = drops[dropIndex].gameObject;
            _sizeDetected = commonParameters.drop.GetComponent<CharacterSize>();
            sizeDrop = _sizeDetected.GetSize();

            //if founded a priority, the drop was setted, restore the priority and
            //update the size
        } else if (hasPriority) {
            commonParameters.priorityDrop = true;
            _sizeDetected = commonParameters.drop.GetComponent<CharacterSize>();
            sizeDrop = _sizeDetected.GetSize();
        }




        //update size
        _animator.SetInteger("SizeDrop", sizeDrop);
        //check if the selected drop is near
        DropNear();
    }

    private void DropNear() {
        if (commonParameters.drop == null) return;

        LayerMask layerCast = (1 << LayerMask.NameToLayer("Character"));
        //check if the drop is near
        //near data
        Vector3 size = Vector3.one;
        size *= commonParameters.near;
        size.y = triggerArea.size.y; //height is the same as trigger area

        size /= 2.0f;
        Vector3 origin=Vector3.zero;// = triggerArea.origin + transform.position;
        origin.x = commonParameters.enemy.transform.position.x;
        origin.y += size.y;
        origin.z = 0;
        //get all drops in the trigger area (near the enemy)
        Collider[] dropsNear = Physics.OverlapBox(origin, size,
            Quaternion.identity, layerCast, QueryTriggerInteraction.Ignore);

        foreach (Collider drop in dropsNear) {
            if (drop.gameObject == commonParameters.drop) {
                _animator.SetBool("Near", true);
            }
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

    /// <summary>
    /// Check the state of the animation to see if we have to call the listeners
    /// </summary>
    private void StateListener() {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        AnimationState state = GetAnimationState(info);
        if (actualState != state) {
            // Notifies the listeners
            if(commonParameters.drop!=null)
                foreach (EnemyBehaviourListener listener in commonParameters.drop.GetComponents<EnemyBehaviourListener>())
                    listener.OnStateAnimationChange(actualState, state);

            foreach (EnemyBehaviourListener listener in listeners)
                listener.OnStateAnimationChange(actualState, state);
            actualState = state;
        }
    }

    private AnimationState GetAnimationState(AnimatorStateInfo info) {
        AnimationState state = AnimationState.IDDLE;

        if (info.IsName("Iddle")) {
            state = AnimationState.IDDLE;
        } else if (info.IsName("Walk")) {
            state = AnimationState.WALK;
        } else if (info.IsName("Chase")) {
            state = AnimationState.CHASE;
        } else if (info.IsName("Detect")) {
            state = AnimationState.DETECT;
		} else if (info.IsName("Attack")) {
			state = AnimationState.ATTACK;
        } else if (info.IsName("Scared")) {
            state = AnimationState.SCARED;
		} else if (info.IsName("Run Away")) {
            state = AnimationState.RUN_AWAY;
        } else if (info.IsName("Hidde")) {
            state = AnimationState.HIDDE_RECOLECT;
        } else if (info.IsName("Recolect")) {
            state = AnimationState.HIDDE_RECOLECT;
        } else if(info.IsName("Air Attack")) {
            state = AnimationState.AIR_ATTACK;
        }

        return state;
    }
    #endregion

    #endregion
}
