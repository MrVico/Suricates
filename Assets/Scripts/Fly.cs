using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : EagleBaseSM {

    private float speed;
    // For capsule cast, viewRange = length & viewRadius = width
    private float viewRange = 5;
    private float viewRadius = 1;
    public Material capsuleMaterial;

    private Transform shape;
    private GameObject sphere;

    private RaycastHit[] hits;
    //private int layerMask;

    private Transform transform;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        transform = animator.transform;
        // Only collide with the suricate layer, which is in position 9
        // The ~ operator inverts a bitmask.
        //layerMask = (1 << 9);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        /*
        if(prey == null) {
            // layer mask on CapsuleCast doesn't work for shit, at least I can't seem to make it work...
            // Doesn't take the distance into account, even though I put viewRange = 1 it still looks up to a distance of 30...
            hits = Physics.CapsuleCastAll(transform.position, transform.position + Vector3.down * transform.position.y, viewRadius, transform.up, viewRange);
            foreach (RaycastHit hit in hits) {
                if (hit.transform.gameObject.tag.Equals("Suricate")) {
                    Debug.Log("Position of suricate: " + hit.transform.position);
                    Debug.Log("Found suricate at a distance of "+Vector3.Distance(transform.position, hit.transform.position));
                    Debug.Log("FULL RANDOM!!! DOESN'T TAKE THE MAXDISTANCE INTO ACCOUNT AT ALL!!!");
                    SetPrey(hit.transform.gameObject);
                    //animator.ResetTrigger(flyHash);
                    //animator.SetTrigger(diveHash);
                    break;
                }
            }
        }
        */
        // When s is pressed we can see the collider with which the eagle sees
        if (Input.GetKeyDown("s")) {
            DrawCapsule();
        }
        else if (Input.GetKeyDown("h")) {
            HideCapsule();
        }
        else if (Input.GetKeyDown("d")) {
            if(sphere == null) {
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = transform.position;
                sphere.transform.localScale = new Vector3(viewRange, viewRange, viewRange);
            }
        }
        else if (Input.GetKeyDown("f")) {
            Destroy(sphere);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}

    private void DrawCapsule() {
        RaycastHit hit;
        // find centers of the top/bottom hemispheres
        Vector3 p1 = transform.position;
        Vector3 p2 = p1 + Vector3.down * transform.position.y;
        float height = p1.y/2;
        p2.y += height;
        p1.y -= height;
        // draw CapsuleCast volume:
        RenderVolume(p1, p2, viewRadius, transform.up, viewRange);
        Debug.Log("P1: " + p1 + " P2: " + p2);
        // cast character controller shape range meters forward:
        if (Physics.CapsuleCast(p1, p2, viewRadius, transform.up, out hit, viewRange)) {
            Debug.Log("Obstacle: true ");
        }
        else {
            Debug.Log("Obstacle: false");
        }

    }

    private void HideCapsule() {
        if (shape)
            shape.GetComponent<Renderer>().enabled = false;
    }

    private void RenderVolume(Vector3 p1, Vector3 p2, float radius, Vector3 dir, float distance) {
        if (!shape) { // if shape doesn't exist yet, create it
            shape = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            Destroy(shape.GetComponent<Collider>()); // no collider, please!
            shape.GetComponent<Renderer>().material = capsuleMaterial; // assign the selected material to it
            // Trying to make it a bit transparent, not working x)
            Color materialColor = shape.GetComponent<MeshRenderer>().material.GetColor("_Color");
            materialColor.a = 0.05f;
        }
        Vector3 scale; // calculate desired scale
        float diam = 2 * radius; // calculate capsule diameter
        scale.x = diam; // width = capsule diameter
        scale.y = Vector3.Distance(p2, p1) + diam; // capsule height
        scale.z = distance + diam; // volume length
        shape.localScale = scale; // set the rectangular volume size
                                  // set volume position and rotation
        shape.position = (p1 + p2 + dir.normalized * distance) / 2;
        shape.rotation = Quaternion.LookRotation(dir, p2 - p1);
        shape.GetComponent<Renderer>().enabled = true; // show it
    }
}
