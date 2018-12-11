using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : SuricateBaseSM {

    private Vector3 destination;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        destination = animator.GetComponent<Suricate>().GetNearestHole().transform.position;
        moveSpeed = 4f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        // If we are safe, near enough of the destination, we wait 'till the danger is gone
        if(Vector3.Distance(obj.transform.position, destination) > 1.4f)
            Move(destination);
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
	}
}
