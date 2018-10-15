using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleBaseSM : StateMachineBehaviour {

    protected static GameObject prey;

    protected int diveHash = Animator.StringToHash("dive");
    protected int flyHash = Animator.StringToHash("fly");

    protected GameObject eagle;
    
    public void SetPrey(GameObject p) {
        EagleBaseSM.prey = p;
        Debug.Log("Setting prey " + prey.name);
    }

    public GameObject GetPrey() {
        return EagleBaseSM.prey;
    }    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        eagle = animator.gameObject;
    }
}
