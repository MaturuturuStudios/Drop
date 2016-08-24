using UnityEngine;
using System.Collections.Generic;
using System;

public class AIAnt : AIBase, CollisionListener {
    /// <summary>
    /// Parameters for the attack state
    /// </summary>
    public AirAttackParameters airAttackParameters;
    /// <summary>
    /// The collider what is watching above ant
    /// </summary>
    public AIColliders collidersAntAir;

    public new void Start() {
        base.Start();

        Scared scared = _animator.GetBehaviour<Scared>();
        scared.commonParameters = commonParameters;

        AirAttack airAttack = _animator.GetBehaviour<AirAttack>();
        airAttack.commonParameters = commonParameters;
        airAttack.parameters = airAttackParameters;
        airAttack.parameters.launcher = attackParameters.launcher;

        collidersAntAir.AddListener(this);
    }

    public void Reset() {
        commonParameters.onFloor = true;
    }

    public void OnTriggerEnter(Collider other) {
        Trigger(other);
    }

    public void OnTriggerStay(Collider other) {
        Trigger(other);
    }

    private void Trigger(Collider other) {
        if (other.gameObject.tag != Tags.Player) return;
        _animator.SetBool("AirAttack", true);

        //This drop has priority, change it!!
        commonParameters.drop = other.gameObject;
        commonParameters.priorityDrop = true;
        int size=commonParameters.drop.GetComponent<CharacterSize>().GetSize();
        _animator.SetInteger("SizeDrop", size);
        TriggerListener(other);
    }
}
