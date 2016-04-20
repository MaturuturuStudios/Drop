﻿using UnityEngine;

using System.Collections.Generic;
using System.Collections;

public class DetectPlayer : StateMachineBehaviour {
    /// <summary>
    /// Time to stay in detect state
    /// </summary>
    [HideInInspector]
    public float timeWarning=0;
    /// <summary>
    /// Timer
    /// </summary>
    private float _deltaTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _deltaTime = timeWarning;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //check if have condition to change state
        int size = animator.GetInteger("SizeDrop");
        int sizeLimit = animator.GetInteger("LimitSizeDrop");
        if (sizeLimit > 0 && size >= sizeLimit) {
            animator.SetBool("GoAway", true);
        }

        _deltaTime -= Time.deltaTime;
        if (_deltaTime <= 0) {
            animator.SetBool("Timer", true);
        }
    }
}
