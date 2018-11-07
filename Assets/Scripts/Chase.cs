using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : SuricateBaseSM {

    private GameObject prey;    
    private float eatingTimer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        moveSpeed = 3f;
        prey = animator.GetComponent<Suricate>().GetPrey();
        eatingTimer = 0;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        // A raptor caught us
        if (raptor != null) {
            ThisIsTheEnd();
        }
        // It is possible that the prey already got dealt with by another suricate
        else if (prey == null) {
            animator.ResetTrigger(Suricate.chaseHash);
            animator.SetTrigger(Suricate.wanderHash);
        }
        else if (eatingTimer == 0) {
            float distance = Vector3.Distance(obj.transform.position, prey.transform.position);
            if (distance > 1.0f) {
                Move(prey.transform.position);
            }
            else {
                prey.SendMessage("Catched");
                //Debug.Log(obj.name+" is eating " + prey.gameObject.name+"...");
                // For the next "eatingTime" we are eating
                eatingTimer += Time.deltaTime;
            }
        }
        // We are currently eating a prey
        else if (eatingTimer > 0) {
            eatingTimer += Time.deltaTime;
            if (eatingTimer >= eatingTime) {
                // The prey is dead
                prey.SendMessage("Dead");
                // The suricate ate
                animator.SendMessage("Ate");
                eatingTimer = 0;
                animator.ResetTrigger(Suricate.chaseHash);
                animator.SetTrigger(Suricate.wanderHash);
            }
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // It is possible that the prey already got dealt with by another suricate
        if (prey != null)
            Destroy(prey);
    }
}
