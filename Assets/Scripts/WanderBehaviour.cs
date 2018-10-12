using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderBehaviour : MonoBehaviour {

    public float wanderingRadius;
    public float wanderingTime;
    public float moveSpeed;
    public float rotationSpeed;
    public float visionDepth;
    public float visionAngle;

    public GameObject enemy;

    private LineRenderer lineRenderer;

    private NavMeshAgent agent;
    private Vector3 destination;
    private Vector3 direction;
    private Quaternion rotation;

    private float timer;

	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>();
        GetNewDestination();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        //DrawVisionField();
        //CheckForObject();
        if (timer >= wanderingTime)
            GetNewDestination();
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        transform.Translate(0, 0, Time.deltaTime * moveSpeed);
    }

    // UNUSED
    private void CheckForObject() {
        float angle = Vector3.Angle(transform.forward, enemy.transform.position);
        Debug.Log(angle);
        if (angle < visionAngle/2) {
            Debug.Log("ENEMY FOUND");
        }
    }

    // UNUSED
    private void DrawVisionField() {
        if (direction != Vector3.zero) {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * visionDepth);
        }
    }

    private void GetNewDestination() {
        destination = RandomDestination(wanderingRadius, -1);
        direction = destination - transform.position;
        //agent.SetDestination(destination);
        rotation = Quaternion.LookRotation(direction);
        timer = 0;
    }

    private Vector3 RandomDestination(float distance, int layermask) {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }
}
