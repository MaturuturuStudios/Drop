using UnityEngine;

/// <summary>
/// Region definition
/// </summary>
[System.Serializable]
public class Region {
    public Vector3 origin = Vector3.one;
    public Vector3 size = Vector3.one;

    public Vector3 GetRandomPoint() {
        Vector3 result = new Vector3(Random.Range(origin.x, origin.x + size.x),
                        Random.Range(origin.y, origin.y + size.y),
                        Random.Range(origin.z, origin.z + size.z));
        return result;
    }

    public void OndrawGizmos(Color color, Vector3 parentPosition) {
        Gizmos.color = color;
        //draw area
        Vector3 originWorld = origin + parentPosition;

        Vector3 bottomLeft = originWorld;
        Vector3 topLeft = originWorld;
        topLeft.y += size.y;
        Vector3 topRight = topLeft;
        topRight.x += size.x;
        Vector3 bottomRight = bottomLeft;
        bottomRight.x += size.x;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);

        Vector3 deep = Vector3.zero;
        deep.z = size.z;
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
