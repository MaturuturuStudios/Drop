using UnityEngine;
using System.Collections;

public class AIBee : AIBase {
    /// <summary>
    /// Parameters for the flying state (bee)
    /// </summary>
    public FlyingParameters flyingParameters;

    public new void Start() {
        //get the behaviours and set their data
        Flying flyingAI = _animator.GetBehaviour<Flying>();
        if (flyingAI != null) {
            flyingAI.commonParameters = commonParameters;
            flyingAI.parameters = flyingParameters;
        }

        base.Start();
    }

    /// <summary>
    /// Set another defaults values
    /// </summary>
    public void Reset() {
        commonParameters.onFloor = false;
    }

    public new void OnDrawGizmosSelected() {
        base.OnDrawGizmosSelected();

        if(goAwayParameters.endMoving)
            if(!flyingParameters.usePath)
                flyingParameters.walkArea.OndrawGizmos(Color.cyan, transform.position);
            else
                flyingParameters.path.OnDrawGizmos();
    }
}
