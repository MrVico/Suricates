using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The behaviour that the sentinel should have when he guards the others
public class Herd : SuricateBaseSM {

    private static int nbOfSentinels = 0;
    
    private float radius = 10f;
    private float depth = 100f;
    private float angle = 20f;
    private float maxLookRotation = 30f;
    // Degrees per second
    private float lookRotationSpeed = 10f;
    private int lookDirection;
    private Physics physics;

    private float postTimer;
    private float initialRotation;

    private Vector3 post;
    private bool onPost = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        post = obj.GetComponent<Suricate>().GetPost();
        nbOfSentinels++;
        onPost = false;
        moveSpeed = 5f;
        postTimer = 0;
        // Something went wrong, we have to find the unoccupied post
        if(post == Vector3.zero){
            post = FindObjectOfType<Spawner>().GetUnoccupiedSentinelPost();
        }
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (!onPost) {
            Move(post);
            if (Vector3.Distance(obj.transform.position, post) < 0.7f) {
                // This way it's perfect
                obj.transform.position = post;
                onPost = true;
            }
        }
        else if (onPost) {
            //Debug.Log("Rotation towards the center, angle: " + Vector3.Angle(obj.transform.forward, (Vector3.zero - obj.transform.position)));
            // We always want to look at the center (0,0,0) of the zone before we look around
            if (postTimer == 0 && Vector3.Angle(obj.transform.forward, (Vector3.zero - obj.transform.position)) >= 0.3f) {
                obj.transform.rotation = Quaternion.RotateTowards(obj.transform.rotation, Quaternion.LookRotation(Vector3.zero - obj.transform.position), 200f * Time.deltaTime);
            }
            else {
                // Look rotation setup once
                if (postTimer == 0) {
                    // Notify the predecessor, if there's one
                    if (obj.transform.GetComponent<Suricate>().GetPredecessor() != null)
                        obj.transform.GetComponent<Suricate>().GetPredecessor().SendMessage("RelievedFromPost");
                    // This way it's perfect
                    obj.transform.rotation = Quaternion.LookRotation(Vector3.zero - obj.transform.position);                    
                    initialRotation = obj.transform.rotation.eulerAngles.y;
                    // We always look the same direction first since the two sentinels are facing each other
                    lookDirection = 1;
                    // Now we stand!
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x, 2.5f, obj.transform.localScale.z);
                    // So we actually see our vision
                    animator.gameObject.transform.Find("HerdRay").gameObject.SetActive(true);
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
        // We don't stand anymore!
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, 1f, obj.transform.localScale.z);
        animator.gameObject.transform.Find("HerdRay").gameObject.SetActive(false);
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
        if (post.z == -24 && y > 270)
            y = y - 360;

        //Debug.Log(obj.transform.name+" initialRotation: " + initialRotation);
        //.Log(obj.transform.name + " Direction: " + lookDirection + " y: " + y);
        
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
        //Debug.DrawRay(obj.transform.position, obj.transform.forward * 2f, Color.red, 0.15f);
    }
}
