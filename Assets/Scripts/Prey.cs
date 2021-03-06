﻿using System.Collections;
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
            Vector3 destination = transform.position + (moveDirection.normalized * runSpeed * Time.deltaTime);
            if (MovementController.IsPositionInsideGroundZone(destination))
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
        life -= Time.timeScale;
        if (life <= 0)
            Dead();
    }

    // We just died
    private void Dead() {
        FindObjectOfType<Spawner>().SendMessage("PreyDied", gameObject);
    }
}
