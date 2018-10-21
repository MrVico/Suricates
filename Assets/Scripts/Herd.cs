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
    // Degrees per second
    private float lookRotationSpeed = 10f;
    private int lookDirection;
    private Physics physics;

    private float timer;
    private float initialRotation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        initialRotation = obj.transform.rotation.eulerAngles.y;
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
                if (coneHits[i].collider.CompareTag("Predator")) {
                    coneHits[i].collider.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1f);
                }
                coneHits[i].collider.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1f);
            }
        }
    }

    private void rotateLook() {
        // Range = [0,360]
        int y = (int) obj.transform.rotation.eulerAngles.y;
        
        // We look all the way, we now need to look in the other direction
        if ((lookDirection == 1 && y >= initialRotation + maxLookRotation) || (lookDirection == -1 && y <= initialRotation - maxLookRotation)) {
            lookDirection *= -1;
        }
        else if (y >= initialRotation - maxLookRotation && y <= initialRotation + maxLookRotation) {
            //obj.transform.eulerAngles = Vector3.Lerp(obj.transform.eulerAngles, new Vector3(0, (initialRotation + maxLookRotation * lookDirection), 0), lookRotationSpeed * Time.deltaTime);
            Quaternion rotation = new Quaternion(0F, 0F, 0F, 0F);
            // We only rotate around the Y axis
            rotation.eulerAngles = new Vector3(0, (initialRotation + maxLookRotation * lookDirection), 0);
            // This way the rotation is smooth by always having the same speed
            obj.transform.rotation = Quaternion.RotateTowards(obj.transform.rotation, rotation, lookRotationSpeed * Time.deltaTime);
        }
    }
}
