using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dive : EagleBaseSM {

    private GameObject prey;
    // Did we catch a suricate?
    private bool caught;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        prey = animator.GetComponent<Eagle>().GetPrey();
        Debug.Log("Diving towards " + prey.name);
        caught = false;
        // We dive fast
        moveSpeed = 5f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!caught) {
            float distance = Vector3.Distance(obj.transform.position, prey.transform.position);
            if (distance > 1.0f) {
                Move(MovementController.CHASE, prey.transform.position);
            }
            else {
                prey.SendMessage("CatchedBy", obj);
                Debug.Log("Caught " + prey.gameObject.name);
                caught = true;
            }
        }
        // We caught a suricate, we fly away with it
        else {
            animator.ResetTrigger(Eagle.diveHash);
            animator.SetTrigger(Eagle.flyAwayHash);
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
