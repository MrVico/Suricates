using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour {

    public float runSpeed = 1f;
    public float safeDistance = 10f;

    private bool catched;
    private GameObject enemy;
    private float life;

	// Use this for initialization
	void Start () {
        catched = false;
        life = Random.Range(50f, 200f);
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

    // Since the suricate FoV doesn't have a collider it's a quick fix
    private void SetEnemy(GameObject suricate) {
        enemy = suricate;
    }

    private void Catched() {
        catched = true;
    }

    public float GetLife() {
        return life;
    }

    // Someone bit us
    private void Aww() {
        life--;
        if (life <= 0)
            Dead();
    }

    private void Dead() {
        Debug.Log("The prey died!");
        // call method on Spawner...
        FindObjectOfType<Spawner>().SendMessage("PreyDied", gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Collider: " + collision.gameObject.name+" Tag: "+collision.gameObject.tag);
        if (collision.gameObject.tag == "SuricateFoV") {
            Debug.Log("run");
            enemy = collision.gameObject;
        }
    }
}
