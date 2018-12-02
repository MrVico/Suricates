using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : SuricateBaseSM {

    private List<GameObject> youths;
    private bool collected;
    private Vector3 position;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        moveSpeed = 4f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        // We can have new young ones assigned to us
        youths = obj.GetComponent<Suricate>().GetYouths();
        collected = true;
        foreach (GameObject youth in youths) {
            // If we still didn't collect everyone we go do it
            if (Vector3.Distance(obj.transform.position, youth.transform.position) > 1.5f) {
                collected = false;
                Move(youth.transform.position);
                return;
            }
        }
        // we collected everyone
        if (collected) {
            obj.SendMessage("CollectFinished");
            animator.ResetTrigger(Suricate.collectHash);
            animator.SetTrigger(Suricate.wanderHash);
        }

    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
