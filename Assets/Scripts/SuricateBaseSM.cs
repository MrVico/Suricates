﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuricateBaseSM : StateMachineBehaviour {

    protected float wanderingRadius = 10f;
    protected float wanderingTime = 3f;
    protected float moveSpeed = 1.5f;
    protected float moveDistance = 10f;
    protected float rotationSpeed = 5f;
    protected float rotationAngle = 70f;
    protected float eatingTime = 1.5f;

    protected GameObject obj;
    protected GameObject eagle;

    private Quaternion rotation;
    private Animator animator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        obj = animator.gameObject;
        this.animator = animator;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Each frame we check if an eagle didn't catch us
        eagle = animator.GetComponent<Suricate>().GetEagle();
    }

    protected void Move(string mode, Vector3 destination) {
        MovementController.Move(obj.transform, mode, destination, rotationSpeed, moveSpeed);
    }

    protected void ThisIsTheEnd() {
        // The eagle is taking it with him
        obj.transform.parent = eagle.transform;
        animator.ResetTrigger(Suricate.wanderHash);
        animator.ResetTrigger(Suricate.chaseHash);
        animator.SetTrigger(Suricate.deadHash);
    }
}
