using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVCollision : MonoBehaviour {

    Suricate parent;

    void Start() {
        parent = transform.parent.GetComponent<Suricate>();
    }

    // Calls the collision method on the parent
    private void OnCollisionStay(Collision collision) {
        parent.SendMessage("OnCollisionStay", collision);
    }
}
