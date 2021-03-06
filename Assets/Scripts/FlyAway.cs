﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAway : RaptorBaseSM {

    // Position where the raptor goes once he is done, randomly chosen
    private Vector3 destination = new Vector3(0f, 25f, 25f);

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        moveSpeed = 3f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (Vector3.Distance(obj.transform.position, destination) > 1f)
            Move(destination);
        else
            Destroy(obj.gameObject);
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
