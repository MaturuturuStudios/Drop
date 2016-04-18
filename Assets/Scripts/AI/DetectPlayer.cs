using UnityEngine;

using System.Collections.Generic;
using System.Collections;

public class DetectPlayer : StateMachineBehaviour {

    /// <summary>
    /// Independent control to create or remove drops
    /// </summary>
    private GameControllerIndependentControl _independentControl;

    

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _independentControl = GameObject.FindGameObjectWithTag(Tags.GameController)
                                .GetComponent<GameControllerIndependentControl>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        List<GameObject> list = _independentControl.getCharacters();
        foreach(GameObject aDrop in list ){
            Transform positionDrop = aDrop.transform;
            if(Vector3.Distance(positionDrop.position, animator.rootPosition) < 5) {
                Debug.Log("Ajaaa!!");
            }
        }
        
        
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }
}
