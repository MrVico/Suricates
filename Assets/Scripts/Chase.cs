using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : SuricateBaseSM {

    public GameObject prey;
    
    private int chaseHash = Animator.StringToHash("chase");
    private int wanderHash = Animator.StringToHash("wander");

    private float eatingTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        prey = animator.GetComponentInChildren<CreateFieldOfVision>().GetPrey();
        eatingTimer = 0;
        Debug.Log("Chasing prey " + prey.gameObject.name);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(eatingTimer == 0) {
            float distance = Vector3.Distance(obj.transform.position, prey.transform.position);
            //Debug.Log("distance: " + distance);
            if (distance > 1.0f) {
                Move(base.CHASE, prey.transform.position);
            }
            else {
                Debug.Log("eating..." + prey.gameObject.name);
                // For the next "eatingTime" we are eating
                eatingTimer += Time.deltaTime;
            }
        }
        // We are currently eating a prey
        else {
            eatingTimer += Time.deltaTime;
            if (eatingTimer >= eatingTime) {
                animator.ResetTrigger(chaseHash);
                animator.SetTrigger(wanderHash);
            }
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("exiting");
        Destroy(prey);
    }
}
