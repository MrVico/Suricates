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
    private float radius = 10f;
    private float depth = 100f;
    private float angle = 20f;
    private float maxLookRotation = 30f;
    private float lookRotationSpeed = 0.1f;
    private int lookDirection;
    private Physics physics;

    private float timer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        // Starts looking left
        if (Random.value < 0.5f)
            lookDirection = -1;
        // Starts looking right
        else
            lookDirection = 1;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        rotateLook();
        detectEnemies();
        timer += Time.deltaTime;

        /*
        // Testing run away to holes
        if(timer > 5.0f) {
            timer = 0;
            foreach(GameObject suricate in GameObject.FindGameObjectsWithTag("Suricate")) {
                suricate.SendMessage("ToSafety");
            }
        }
        */
        // A raptor caught us
        if (raptor != null) {
            ThisIsTheEnd();
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}

    private void detectEnemies() {
        RaycastHit[] coneHits = MyPhysics.ConeCastAll(obj.transform.position, radius, obj.transform.forward, depth, angle);
        if (coneHits.Length > 0) {
            for (int i = 0; i < coneHits.Length; i++) {
                coneHits[i].collider.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1f);
            }
        }
    }

    private void rotateLook() {
        // Range = [0,360]
        float y = obj.transform.rotation.eulerAngles.y;

        // Looking right                                    we finished looking left at an angle of ~ 360-maxLookRotation
        if (lookDirection == 1 && (y < maxLookRotation || y > 360 - maxLookRotation - 1)) {
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, Quaternion.LookRotation(new Vector3(maxLookRotation, 0, 0)), lookRotationSpeed * Time.deltaTime);
        }
        // Looking left                     The angle is > 0        we finished looking right at an angle of ~ maxLookRotation
        else if (lookDirection == -1 && (y > 360 - maxLookRotation || y < maxLookRotation + 1)) {
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, Quaternion.LookRotation(new Vector3(-maxLookRotation, 0, 0)), lookRotationSpeed * Time.deltaTime);
        }
        else if ((lookDirection == 1 && y >= maxLookRotation) || (lookDirection == -1 && y >= 360 - maxLookRotation - 1)) {
            // We look in the other direction
            lookDirection *= -1;
        }
    }
}
