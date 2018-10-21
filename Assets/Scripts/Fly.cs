using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : RaptorBaseSM {

    private float speed;
    
    private Vector3 destination;
    private float timer;
    private bool reset;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        moveSpeed = 3f;
        timer = 0;
        // If this is true, it means we just dove and have to reset our height
        reset = (obj.transform.position.y < flyHeight);
        destination = MovementController.GetNewDestination(obj.transform.position, obj.transform.forward, rotationAngle, moveDistance, reset);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        timer += Time.deltaTime;
        // If we reset we don't need to wait for the timer
        if ((timer >= flyTime || reset) && obj.transform.position.y >= flyHeight) {
            // Reset done
            reset = false;
            // We want the raptor to fly horizontally over the ground
            if (obj.transform.forward.y != 0) {
                Vector3 tmp = obj.transform.forward;
                tmp.y = 0;
                obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, Quaternion.LookRotation(tmp), rotationSpeed * Time.deltaTime);
            }
            destination = MovementController.GetNewDestination(obj.transform.position, obj.transform.forward, rotationAngle, moveDistance);
            // We always want to fly at this height
            destination.y = flyHeight;
            timer = 0;
        }
        Move(destination);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
