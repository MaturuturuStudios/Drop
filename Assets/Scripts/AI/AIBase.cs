using UnityEngine;
using System.Collections.Generic;

public class AIBase : MonoBehaviour {
    #region Public Attribute
    /// <summary>
    /// Area in which enemy react
    /// </summary>
    public Region triggerArea;
    /// <summary>
    /// Under this limit, enemy attack, equal or over this size, the enemy escape
    /// </summary>
    public int sizeLimitDrop;
    /// <summary>
    /// Time between detecting a drop and chasing him or come back to iddle/walking if not
    /// in trigger area anymore
    /// </summary>
    public float timeDetectChase;
    /// <summary>
    /// End point when reached a running away state
    /// Can be a static point in a flower to let bee recolect
    /// or a point behind scene or lateral point to make an ant to disappear
    /// </summary>
    public Vector3 endPoint;
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
    #endregion

    #region Methods
    /// <summary>
    /// Initialization method.
    /// The character start with size one
    /// </summary>
    public void Awake() {
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController)
                                .GetComponent<GameControllerIndependentControl>();
        _animator = gameObject.GetComponent<Animator>();
        _animator.SetInteger("LimitSizeDrop", sizeLimitDrop);
    }

    public void Start() {
        //get the behaviours and set their data
        Walking walkingAI = _animator.GetBehaviour<Walking>();
        walkingAI.enemy = gameObject;

        DetectPlayer detectedAI = _animator.GetBehaviour<DetectPlayer>();
        detectedAI.timeWarning = timeDetectChase;
    }

    public void Update() {
        //if we have a drop detected, continue with it and check if is outside the trigger or not
        //if (true) {
        //    //send to the SM that drop dissapear (size=0)
        //} else {
            
        //}

        LayerMask layerCast = (1 << LayerMask.NameToLayer("Character")); ;
        Vector3 center = triggerArea.origin + transform.position;
        Vector3 halfSize = triggerArea.size / 2;
        center.x += halfSize.x;
        center.y += halfSize.y;
        center.z += halfSize.z;
        Collider[] drops=Physics.OverlapBox(center, halfSize,
            Quaternion.identity, layerCast, QueryTriggerInteraction.Ignore);

        int sizeDrop = 0;
        foreach(Collider dropCollider in drops) {
            Debug.Log("Enter to detection");
            if (_independentControl.IsUnderControl(dropCollider.gameObject)) {
                _detectedDrop = dropCollider.gameObject;
                sizeDrop=(_detectedDrop.GetComponent<CharacterSize>()).GetSize();
            }
        }

        _animator.SetInteger("SizeDrop", sizeDrop);
    }
    #endregion


    #region Private class
    /// <summary>
    /// Region definition
    /// </summary>
    [System.Serializable]
    public class Region {
        public Vector3 origin = Vector3.one;
        public Vector3 size = Vector3.one;
    }

    /// <summary>
    /// Draws the trigger area.
    /// </summary>
    public void OnDrawGizmos() {
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
