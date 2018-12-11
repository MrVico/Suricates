using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuricateBaseSM : StateMachineBehaviour {

    // Suricate state machine parameters
    protected float wanderingRadius = 10f;
    protected float wanderingTime = 3f;
    protected float moveSpeed = 1.5f;
    protected float moveDistance = 7f;
    protected float rotationSpeed = 5f;
    protected float rotationAngle = 70f;
    protected float eatingTime = 1.5f;

    protected GameObject obj;
    protected GameObject raptor;

    private Quaternion rotation;

    // If we are a tutor we have to wait for the babies
    private bool wait;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        obj = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // Each frame we check if an eagle didn't catch us
        raptor = animator.GetComponent<Suricate>().GetRaptor();
    }

    protected void Move(Vector3 destination) {
        if(!wait)
            MovementController.Move(obj.transform, destination, rotationSpeed, moveSpeed);
    }
    
    // Babies notify to wait
    public void WaitForUs() {
        wait = true;
    }

    // Babies are here, let's go!
    public void AllGood() {
        wait = false;
    }
}
