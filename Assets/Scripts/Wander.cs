using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : SuricateBaseSM {

    private Vector3 destination;
    private Quaternion rotation;

    private float timer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        GetNewDestination();
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        timer += Time.deltaTime;
        if (timer >= wanderingTime)
            GetNewDestination();
        Move(rotation);
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}

    private void GetNewDestination() {
        destination = RandomDestination(wanderingRadius, -1);
        rotation = RotateTowards(destination);
        timer = 0;
    }

    private Vector3 RandomDestination(float distance, int layermask) {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += obj.transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }
}
