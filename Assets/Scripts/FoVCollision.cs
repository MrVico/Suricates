using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UNUSED was for my attempt at the field of vision cone
public class FOVCollision : MonoBehaviour {

    Suricate parent;

    void Start() {
        parent = transform.parent.GetComponent<Suricate>();
    }

    // Calls the collision method on the parent
    private void OnCollisionStay(Collision collision) {
        Debug.Log("New FOV collision");
        parent.SendMessage("OnCollisionStay", collision);
    }
}
