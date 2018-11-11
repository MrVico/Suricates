using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Follow : SuricateBaseSM {

    // Shouldn't always be the alpha female, but way too complicated to do otherwise
    private GameObject tutor;
    private float growthTime;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        // We want to follow the nearest male hunter
        foreach (GameObject candidat in GameObject.FindGameObjectsWithTag("Suricate")) {
            if (candidat.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter)
                 && candidat.GetComponent<Suricate>().GetGender().Equals(Suricate.Gender.Female)
                 && candidat.GetComponent<Suricate>().IsAlpha() ) {
                tutor = candidat;
                return;
            }
        }
        growthTime = 0;
        Debug.Log("Tutor: " + tutor.name);
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        growthTime += Time.deltaTime;
        // After 10s we are no more baby!
        if(growthTime > 10f) {
            animator.SetBool("baby", false);
            animator.SetBool("hunter", true);
        }
        if(Vector3.Distance(obj.transform.position, tutor.transform.position) > 1.5f) {
            Move(tutor.transform.position);
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // We are big!
        obj.transform.localScale *= 2;
    }
}
