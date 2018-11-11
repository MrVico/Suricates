using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : RaptorBaseSM {

    private float speed;
    
    private Vector3 destination;
    private float destinationTimer;
    private bool reset;
    private float flyTimer;
    private float leaveTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        moveSpeed = 3f;
        destinationTimer = 0;
        flyTimer = 0;
        // If this is true, it means we just dove and have to reset our height
        reset = (obj.transform.position.y < flyHeight);
        if(MovementController.IsPositionInsideGroundZone(obj.transform.position))
            destination = MovementController.GetNewDestination(obj.transform.position, obj.transform.forward, rotationAngle, moveDistance, reset);
        // This is when the raptor just spawned, it needs to get inside the zone first
        else
            destination = MovementController.GetNewDestination(obj.transform.position, obj.transform.forward, rotationAngle, 20f, reset);
        // After this time passed the raptor leaves
        leaveTime = Random.Range(10f, 20f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        destinationTimer += Time.deltaTime;
        flyTimer += Time.deltaTime;
        if(flyTimer >= leaveTime) {
            animator.ResetTrigger(Raptor.flyHash);
            animator.SetTrigger(Raptor.flyAwayHash);
        }
        // If we reset we don't need to wait for the timer
        else if ((destinationTimer >= flyTime || reset) && obj.transform.position.y >= flyHeight && MovementController.IsPositionInsideGroundZone(obj.transform.position)) {
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
            destinationTimer = 0;
        }
        Move(destination);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
