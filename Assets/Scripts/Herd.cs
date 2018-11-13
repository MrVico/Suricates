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
    private static int nbOfSentinels = 0;
    
    private float radius = 10f;
    private float depth = 100f;
    private float angle = 20f;
    private float maxLookRotation = 30f;
    // Degrees per second
    private float lookRotationSpeed = 10f;
    private float postTime = 10f;
    private int lookDirection;
    private Physics physics;

    private float postTimer;
    private float initialRotation;

    private Vector3[] posts = { new Vector3(0, 0.5f, -24), new Vector3(34, 0.5f, 0), new Vector3(0, 0.5f, 24), new Vector3(-34, 0.5f, 0) };
    private int postIndex;
    private bool onPost = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        // TODO: Change that cause it's ugly
        // Only called once per suricate
        if (!onPost) {
            // TODO: Change that, only works for 1 or 2 sentinels!!!
            postIndex = nbOfSentinels * 2;
        }
        nbOfSentinels++;
        onPost = false;
        moveSpeed = 5f;
        postTimer = 0;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        //Debug.DrawRay(obj.transform.position, obj.transform.forward*5, Color.red, 0.16f);
        if (!onPost) {
            Move(posts[postIndex]);
            if (Vector3.Distance(obj.transform.position, posts[postIndex]) < 0.2f) {
                // This way it's perfect
                obj.transform.position = posts[postIndex];
                onPost = true;
            }
        }
        else if (onPost) {
            //Debug.Log("Rotation towards the center, angle: " + Vector3.Angle(obj.transform.forward, (Vector3.zero - obj.transform.position)));
            // We always want to look at the center (0,0,0) of the zone before we look around
            if (postTimer == 0 && Vector3.Angle(obj.transform.forward, (Vector3.zero - obj.transform.position)) >= 0.2f) {
                obj.transform.rotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.LookRotation(Vector3.zero - obj.transform.position), 200f * Time.deltaTime);
            }
            else {
                // Look rotation setup once
                if (postTimer == 0) {
                    // This way it's perfect
                    obj.transform.rotation = Quaternion.LookRotation(Vector3.zero - obj.transform.position);                    
                    initialRotation = obj.transform.rotation.eulerAngles.y;
                    // Starts looking left                    
                    if (Random.value < 0.5f)
                        lookDirection = -1;
                    // Starts looking right
                    else
                        lookDirection = 1;
                    // Now we stand!
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x, 2.5f, obj.transform.localScale.z);
                }
                postTimer += Time.deltaTime;
                // We look around
                rotateLook();
                detectEnemies();
            }
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
                    // Notify everyone of the danger!
                    foreach (GameObject suricate in GameObject.FindGameObjectsWithTag("Suricate")) {
                        suricate.SendMessage("ToSafety");
                    }
                }
                if (coneHits[i].collider.CompareTag("Wall"))
                    coneHits[i].collider.gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 1f);
            }
        }
    }
        
    private void rotateLook() {
        // Range = [0,360]
        int y = (int) obj.transform.rotation.eulerAngles.y;
        //float y = obj.transform.rotation.eulerAngles.y;

        // Quick fix
        if (postIndex == 0 && y > 270)
            y = y - 360;

        //Debug.Log(obj.transform.name+" initialRotation: " + initialRotation);
        //Debug.Log(obj.transform.name + " Direction: " + lookDirection + " y: " + y);
        
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
        Debug.DrawRay(obj.transform.position, obj.transform.forward * 2f, Color.red, 0.15f);
    }
}
