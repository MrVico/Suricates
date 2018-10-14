using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour {

    private float runSpeed = 1f;
    private float safeDistance = 15f;

    private bool catched;
    private GameObject enemy;

	// Use this for initialization
	void Start () {
        catched = false;
    }
	
	// Update is called once per frame
	void Update () {
        // If we didn't get catched and an enemy is chasing us and is too close
        if(!catched && enemy != null && Vector3.Distance(transform.position, enemy.transform.position) < safeDistance) {
            Vector3 moveDirection = transform.position - enemy.transform.position;
            moveDirection.y = 0;
            transform.Translate(moveDirection.normalized * runSpeed * Time.deltaTime);
        }
        // If an enemy was close but is now at a safe distance
        else if(enemy != null && Vector3.Distance(transform.position, enemy.transform.position) >= safeDistance) {
            enemy = null;
        }
    }

    private void Catched() {
        catched = true;
    }

    private void Dead() {
        // call method on PreySpawner...
        FindObjectOfType<PreySpawner>().SendMessage("PreyDied", gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag != "Ground") {
            enemy = collision.gameObject;
        }
    }
}
