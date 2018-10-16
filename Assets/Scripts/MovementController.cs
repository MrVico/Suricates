using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour{

    public static readonly string CHASE = "chase";
    public static readonly string WANDER = "wander";

    // To get the ground zone boundaries
    private static Renderer render;
   
    void Start() {
        render = GetComponent<Renderer>();
    }

    public static void Move(Transform objTransform, string mode, Vector3 destination, float rotationSpeed, float moveSpeed) {
        if(objTransform.tag != "Predator" || mode == CHASE) {
            Quaternion rotation = RotateTowards(objTransform, mode, destination);
            objTransform.rotation = Quaternion.Slerp(objTransform.rotation, rotation, rotationSpeed * Time.deltaTime);
            objTransform.Translate(0, 0, Time.deltaTime * moveSpeed);
        }
        else
            objTransform.Translate(0, Time.deltaTime * moveSpeed, 0);
    }

    // Computes the rotation towards a destination
    public static Quaternion RotateTowards(Transform objTransform, string mode, Vector3 destination) {
        Vector3 direction = destination;
        // We only want to recompute the direction if we are chasing something
        if (mode == CHASE)
            direction = destination - objTransform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        // We only want to rotate left/right, so around the Y axis, if we aren't flying
        if(objTransform.tag != "Predator") {
            rotation.x = 0;
            rotation.z = 0;
        }
        return rotation;
    }

    public static Vector3 GetNewDestination(Vector3 position, Vector3 direction, float rotationAngle, float distance) {
        Vector3 destination = GetDirectionWithinAngle(direction, rotationAngle) * distance;
        float angle = 0f;
        // We need to get a destination inside the ground zone
        // We sum the two vectors to get the destination relative to the origin (0,0,0) 
        while (!IsDestinationInsideGroundZone(position + destination)) {
            angle += 10f;
            destination = GetDirectionWithinAngle(direction, rotationAngle, true, angle) * distance;
            Debug.Log("Recomputing destination to be inside the zone...");
        }
        //Debug.DrawRay(position, destination, Color.red, 3f);
        return destination;
    }

    // The lost parameter is true if we are going outside the ground zone
    // In which case we rotate degree by degree so we get a destination inside the ground zone
    private static Vector3 GetDirectionWithinAngle(Vector3 targetDirection, float maxAngle, bool lost=false, float angle=0f) {
        float angleInRad;
        if (!lost)
            angleInRad = Random.Range(0.0f, maxAngle) * Mathf.Deg2Rad;
        else
            angleInRad = angle * Mathf.Deg2Rad;
        Vector2 pointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        Vector3 directionModifier = new Vector3(pointOnCircle.x, 0, Mathf.Cos(angleInRad));
        return Quaternion.LookRotation(targetDirection) * directionModifier;
    }

    private static bool IsDestinationInsideGroundZone(Vector3 destination) {
        // -1 to let a little bit of space between the destination and the border
        if (destination.x >= render.bounds.min.x - 1 && destination.x <= render.bounds.max.x - 1 && destination.z >= render.bounds.min.z - 1 && destination.z <= render.bounds.max.z - 1)
            return true;
        return false;
    }
}
