using UnityEngine;

public class AIBase : MonoBehaviour {
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
    float timeDetectChase;
    /// <summary>
    /// End point when reached a running away state
    /// Can be a static point in a flower to let bee recolect
    /// or a point behind scene or lateral point to make an ant to disappear
    /// </summary>
    Vector3 endPoint;
    


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

        Vector3 topLeft = originWorld;
        Vector3 topRight = originWorld;
        topRight.x += triggerArea.size.x;
        Vector3 bottomLeft = originWorld;
        bottomLeft.y -= triggerArea.size.y;
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
}
