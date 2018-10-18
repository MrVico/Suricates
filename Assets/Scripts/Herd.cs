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

    public Material capsuleMaterial;
    private Transform shape;
    // For capsule cast, viewRange = length & viewRadius = width
    private float viewRange = 15;
    private float viewRadius = 3;

    private float timer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        timer += Time.deltaTime;

        // Testing run away to holes
        if(timer > 5.0f) {
            foreach(GameObject suricate in GameObject.FindGameObjectsWithTag("Suricate")) {
                suricate.SendMessage("ToSafety");
            }
        }

        // A raptor caught us
        if (raptor != null) {
            ThisIsTheEnd();
        }
        // When s is pressed we can see the collider with which the eagle sees
        if (Input.GetKeyDown("s")) {
            DrawCapsule();
        }
        else if (Input.GetKeyDown("d")) {
            HideCapsule();
        }
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}



    /**
     *  TESTING VISION
     * */

    private void DrawCapsule() {
        RaycastHit hit;
        // find centers of the top/bottom hemispheres
        Vector3 p1 = obj.transform.position;
        Vector3 p2 = p1 + Vector3.down * obj.transform.position.y;
        float height = p1.y / 2;
        p2.y += height;
        p1.y -= height;
        p2 = p1;
        Vector3 direction = new Vector3(-1, 1, 0);
        // draw CapsuleCast volume:
        RenderVolume(p1, p2, viewRadius, direction, viewRange);
        Debug.Log("P1: " + p1 + " P2: " + p2);
        // cast character controller shape range meters forward:
        if (Physics.CapsuleCast(p1, p2, viewRadius, direction, out hit, viewRange)) {
            Debug.Log("Obstacle: true --> "+hit.transform.gameObject.name);
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
