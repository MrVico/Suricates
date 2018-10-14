using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour {

    public float speed;
    public float viewRange = 10; 
    public float viewRadius = 10;
    public Material capsuleMaterial;

    private Transform shape;

    private RaycastHit hit;
    private GameObject prey;

    // Use this for initialization
    void Start () {

    }

    void Update() {
        // We have a target, we go get it!
        if(prey == null && Physics.CapsuleCast(transform.position, transform.position + Vector3.down * transform.position.y, viewRadius, transform.up, out hit)) {
            Debug.Log("Eagle found prey!");
            prey = hit.transform.gameObject;
        }
        // We look for a target
        else if (false) {

        }        
        // When s is pressed we can see the collider with which the eagle sees
        else if (Input.GetKeyDown("s")) {
            DrawCapsule();
        }
        else if (Input.GetKeyDown("h")) {
            HideCapsule();
        }
    }

    private void DrawCapsule() {
        RaycastHit hit;
        // find centers of the top/bottom hemispheres
        Vector3 p1 = transform.position;
        Vector3 p2 = p1 + Vector3.down * transform.position.y;
        // draw CapsuleCast volume:
        RenderVolume(p1, p2, viewRadius, transform.up, viewRange);
        // cast character controller shape range meters forward:
        if (Physics.CapsuleCast(p1, p2, viewRadius, transform.up, out hit, viewRange)) {
            // if some obstacle inside range, save its distance 
            Debug.Log("Obstacle: true ");
        }
        else {
            // otherwise shows that the way is clear up to range distance
            Debug.Log("Obstacle: false");
        }

    }

    void HideCapsule() {
        if (shape)
            shape.GetComponent<Renderer>().enabled = false;
    }

    void RenderVolume(Vector3 p1, Vector3 p2, float radius, Vector3 dir, float distance) {
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
