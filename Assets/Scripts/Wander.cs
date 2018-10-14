using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wander : SuricateBaseSM {

    private Vector3 destination;
    private float timer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        timer = 0;
        destination = GetNewDestination();
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        timer += Time.deltaTime;
        if (timer >= wanderingTime) {
            destination = GetNewDestination();
        }
        Move(base.WANDER, destination);
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}

    private Vector3 GetNewDestination() {
        Vector3 destination = GetDirectionWithinAngle(obj.transform.forward, 90) * 20;
        //Debug.DrawRay(obj.transform.position, destination, Color.red, 3f);
        timer = 0;
        return destination;
    }

    private Vector3 GetDirectionWithinAngle(Vector3 targetDirection, float angle) {
        float angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        Vector2 pointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        Vector3 directionModifier = new Vector3(pointOnCircle.x, 0, Mathf.Cos(angleInRad));
        return Quaternion.LookRotation(targetDirection) * directionModifier;
    }
}
