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

    private Vector3 initialForward;

    private Vector3[] posts = { new Vector3(0, 0.5f, -24), new Vector3(34, 0.5f, 0), new Vector3(0, 0.5f, 24), new Vector3(-34, 0.5f, 0) };
    private int postIndex;
    private bool onPost = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        // TODO: Change that cause it's ugly
        // This means that the suricate just ran and came back to its post
        if (!onPost) {
            // TODO: Change that, only works for 1 or 2 sentinels!!!
            postIndex = nbOfSentinels * 2;
        }
        nbOfSentinels++;
        onPost = false;
        moveSpeed = 5f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        //Debug.DrawRay(obj.transform.position, obj.transform.forward*5, Color.red, 0.16f);
        // A raptor caught us
        if (raptor != null) {
            ThisIsTheEnd();
        }
        else if (!onPost) {
            Move(posts[postIndex]);
            if (Vector3.Distance(obj.transform.position, posts[postIndex]) < 0.1f) {
                Debug.Log("Arrived on post");
                // This way it's perfect
                obj.transform.position = posts[postIndex];
                onPost = true;
            }
        }
        else if (onPost) {
            // We always want to look at the center (0,0,0) of the zone before we look around
            //Debug.Log("Rotation towards the center, angle: " + Vector3.Angle(obj.transform.forward, (Vector3.zero - obj.transform.position)));
            if (postTimer == 0 && Vector3.Angle(obj.transform.forward, (Vector3.zero - obj.transform.position)) >= 0.2f) {
                obj.transform.rotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.LookRotation(Vector3.zero - obj.transform.position), 200f * Time.deltaTime);
            }
            else {
                //Debug.Log("Looking around...");
                // Look rotation setup once
                if (postTimer == 0) {
                    // This way it's perfect
                    obj.transform.rotation = Quaternion.LookRotation(Vector3.zero - obj.transform.position);
                    initialForward = obj.transform.forward;
                    initialRotation = obj.transform.rotation.eulerAngles.y;
                    // Starts looking left                    
                    if (Random.value < 0.5f)
                        lookDirection = -1;
                    // Starts looking right
                    else
                        lookDirection = 1;
                }
                postTimer += Time.deltaTime;
                // We were on this post long enough, off to the next one
                // Not useful I think, IRL the suricates don't rotate positions
                /*
                if (postTimer > postTime) {
                    postIndex = (postIndex + 1) % 4;
                    onPost = false;
                    postTimer = 0;
                }
                */
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
    }
}
