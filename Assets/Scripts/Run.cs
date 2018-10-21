﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : SuricateBaseSM {

    /**
     * State of a suricate one's a raptor was spotted
     **/

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
        // A raptor caught us
        if (raptor != null) {
            ThisIsTheEnd();
        }
        Move(destination);
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}