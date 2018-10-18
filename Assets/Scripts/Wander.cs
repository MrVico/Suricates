﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : SuricateBaseSM {

    private Vector3 destination;
    private float timer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        moveSpeed = 1.5f;
        timer = 0;
        destination = MovementController.GetNewDestination(obj.transform.position, obj.transform.forward, rotationAngle, moveDistance);
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (raptor == null) {
            timer += Time.deltaTime;
            if (timer >= wanderingTime) {
                destination = MovementController.GetNewDestination(obj.transform.position, obj.transform.forward, rotationAngle, moveDistance);
                timer = 0;
            }
            Move(destination);
        }
        // An eagle caught us
        else {
            ThisIsTheEnd();
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
