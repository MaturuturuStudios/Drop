using UnityEngine;
using System.Collections;

public class DrawerPathDefinition : MonoBehaviour {
    public PathDefinition path;

    public void OnDrawGizmos() {
        //draw paths
        path.OnDrawGizmos();
    }
}
