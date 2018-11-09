using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dive : RaptorBaseSM {

    private GameObject prey;
    // Did we catch a suricate?
    private bool caught;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        prey = animator.GetComponent<Raptor>().GetPrey();
        caught = false;
        moveSpeed = 7f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        // The suricate got to safety
        if(prey.GetComponent<Suricate>().IsSafe()) {
            animator.GetComponent<Raptor>().SendMessage("LostPrey");
            // We didn't catch anything we go back to flying 'cause we are hungry
            animator.ResetTrigger(Raptor.diveHash);
            animator.SetTrigger(Raptor.flyHash);
        }
        else if (!caught) {
            float distance = Vector3.Distance(obj.transform.position, prey.transform.position);
            if (distance > 1.0f) {
                Move(prey.transform.position);
            }
            else {
                prey.SendMessage("CaughtBy", obj);
                caught = true;
            }
        }
        // We caught a suricate, we fly away with it
        else {
            animator.ResetTrigger(Raptor.diveHash);
            animator.SetTrigger(Raptor.flyAwayHash);
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
