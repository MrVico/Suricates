using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : SuricateBaseSM {

    private GameObject prey;    
    private bool eating;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        moveSpeed = 3f;
        prey = animator.GetComponent<Suricate>().GetPrey();
        eating = false;
	}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        // The prey was eaten, we go back to wandering around
        if (prey == null || (prey != null && prey.GetComponent<Prey>().GetLife() <= 0)) {
            eating = false;
            animator.ResetTrigger(Suricate.chaseHash);
            animator.SetTrigger(Suricate.wanderHash);
        }
        else if (!eating) {
            float distance = Vector3.Distance(obj.transform.position, prey.transform.position);
            if (distance > 1.0f) {
                Move(prey.transform.position);
            }
            else {
                prey.SendMessage("Catched");
                eating = true;
            }
        }
        // We are currently eating a prey
        else if (eating && prey.GetComponent<Prey>().GetLife() > 0) {
            // We took a bite
            animator.SendMessage("TakeABite", prey);
            // If we are a tutor we need to give some to the youths
            if (obj.GetComponent<Suricate>().GetYouths().Count > 0) {
                foreach (GameObject youth in obj.GetComponent<Suricate>().GetYouths()) {
                    youth.SendMessage("TakeABite", prey);
                }
            }
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (prey != null)
            Destroy(prey);
    }
}
