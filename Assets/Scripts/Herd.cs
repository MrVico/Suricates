using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The behaviour that the sentinel should have when he guards the others
public class Herd : SuricateBaseSM {

    /**
     * The sentinel makes rounds.
     * If it's hungry another takes its place.
     * We could make the sentinel watch the sky for 3s, than the ground for 3s, repeat... 
     * */

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        // A raptor caught us
        if (raptor != null) {
            ThisIsTheEnd();
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
