using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAway : RaptorBaseSM {

    // Position where the raptor goes once he is done
    private Vector3 destination = new Vector3(0, 25, 25);

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        moveSpeed = 3f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (obj.transform.position != destination)
            Move(MovementController.CHASE, destination);
        else
            Debug.Log("Eagle arrived!");
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
