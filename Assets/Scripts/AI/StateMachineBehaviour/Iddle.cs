﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class IddleParameters {
    /// <summary>
    /// Time the entity will be in iddle state
    /// will be ignored if the enemy does not walk
    /// </summary>
    public float timeInIddle=0;
}

public class Iddle : StateMachineBehaviour {
    #region Attributes
    /// <summary>
    /// Parameters
    /// </summary>
    [HideInInspector]
    public IddleParameters parameters;
    [HideInInspector]
    public CommonParameters commonParameters;

    /// <summary>
	/// A reference to the entity's controller.
	/// </summary>
	private CharacterController _controller;
    /// <summary>
    /// Timer
    /// </summary>
    private float _deltaTime;
    #endregion

    #region Methods
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //start timer
        _deltaTime = parameters.timeInIddle;
        _controller = commonParameters.enemy.GetComponent<CharacterController>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //reset states (using bool because trigger does not work correctly)
        animator.SetBool("Detect", false);
        animator.SetBool("Timer", false);
        animator.SetBool("GoAway", false);
        animator.SetBool("Reached", false);
        animator.SetBool("Near", false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //check if have condition to change state
        AIMethods.CheckDrop(animator, commonParameters.sizeLimitDrop);
        CheckTimeIddle(animator);
        
        //always use the gravity
        //set gravity and move
        if (commonParameters.onFloor) {
            Vector3 move = (commonParameters.enemy.transform.up * -1) * 25 * Time.deltaTime;
            _controller.Move(move);
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //OnStateMove is called before OnAnimatorMove would be called in MonoBehaviours for every frame the state is playing.
        //When OnStateMove is called, it will stop OnAnimatorMove from being called in MonoBehaviours.
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //OnStateIK is called after OnAnimatorIK on MonoBehaviours for every frame the while the state is being played.
        //It is important to note that OnStateIK will only be called if the state is on a layer that has an IK pass. 
        //By default, layers do not have an IK pass and so this function will not be called. 
        //For more information on IK see the information linked below.
    }

    private void CheckTimeIddle(Animator animator) {
        if (commonParameters.walking) {
            //if enemy walk, check the timer
            _deltaTime -= Time.deltaTime;
            if (_deltaTime <= 0) {
                animator.SetBool("Timer", true);
            }
        }else {
            //if the enemy is not walking but not in its position, go to walking
            Vector3 target = commonParameters.initialPositionEnemy;
            Quaternion rotationTarget = commonParameters.initialRotationEnemy;
            
            if (!AIMethods.CheckTargetPoint(commonParameters.enemy, target, commonParameters.onFloor, commonParameters.minimumWalkingDistance)) {
                animator.SetBool("Timer", true);
            }

            if(!AIMethods.CheckTargetRotation(commonParameters.enemy, rotationTarget, commonParameters.toleranceDegreeToGoal)) {
                animator.SetBool("Timer", true);
            }

        }
    }
    #endregion
}
