using UnityEngine;
using System.Collections.Generic;

public class AIAnt : AIBase {
    public new void Start() {
        base.Start();

        Scared scared = _animator.GetBehaviour<Scared>();
        scared.commonParameters = commonParameters;
    }

    public void Reset() {
        commonParameters.onFloor = true;
    }
}
