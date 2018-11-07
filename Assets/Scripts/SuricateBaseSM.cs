using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuricateBaseSM : StateMachineBehaviour {

    protected float wanderingRadius = 10f;
    // If we change this we need to change the moveDistance
    protected float wanderingTime = 3f;
    // Same thing here
    protected float moveSpeed = 1.5f;
    protected float moveDistance = 7f;
    protected float rotationSpeed = 5f;
    protected float rotationAngle = 70f;
    protected float eatingTime = 1.5f;

    protected GameObject obj;
    protected GameObject raptor;

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
        raptor = animator.GetComponent<Suricate>().GetRaptor();
        if(raptor != null)
            Debug.Log("POSITION: "+obj.transform.position);
    }

    protected void Move(Vector3 destination) {
        if(raptor != null)
            Debug.Log("MOVING");
        MovementController.Move(obj.transform, destination, rotationSpeed, moveSpeed);
    }

    // The suricate is dead
    protected void ThisIsTheEnd() {
        Debug.Log("RIP " + animator.name);
        // The eagle is taking it with him
        obj.transform.parent = raptor.transform;
        //obj.transform.position = new Vector3(0, -0.6f, 0.8f);
        animator.gameObject.SendMessage("Die");
    }
}
