using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour{

    // To get the ground zone boundaries
    private static Renderer render;
   
    void Start() {
        render = GetComponent<Renderer>();
    }

    // Moves the given transform/gameobject
    public static void Move(Transform objTransform, Vector3 destination, float rotationSpeed, float moveSpeed) {
        Quaternion rotation = RotateTowards(objTransform, destination);
        objTransform.rotation = Quaternion.Slerp(objTransform.rotation, rotation, rotationSpeed * Time.deltaTime);
        objTransform.Translate(0, 0, Time.deltaTime * moveSpeed);
    }

    // Computes the rotation towards a destination
    public static Quaternion RotateTowards(Transform objTransform, Vector3 destination) {
        Vector3 direction = destination;
        direction = destination - objTransform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        // We only want to rotate left/right, so around the Y axis, if we aren't flying
        if(objTransform.tag != "Predator") {
            rotation.x = 0;
            rotation.z = 0;
        }
        return rotation;
    }

    // Gets a new destination given a current position, forward vector and max distance
    public static Vector3 GetNewDestination(Vector3 position, Vector3 forward, float rotationAngle, float distance, bool raptorReset=false) {
        Vector3 direction = GetDirectionWithinAngle(forward, rotationAngle);
        if (raptorReset)
            direction.y = 0.5f;
        float angle = 0f;
        int angleDirection;
        // This way we don't always turn in the same direction
        if(Random.value < 0.5f) {
            angleDirection = 1;
        }
        else {
            angleDirection = -1;
        }
        Vector3 destination = position + direction * distance;
        // We need to get a destination inside the ground zone
        while (!IsPositionInsideGroundZone(destination)) {
            angle += 5f*angleDirection;
            direction = GetDirectionWithinAngle(forward, angle, false);
            if (raptorReset)
                direction.y = 0.5f;
            destination = position + direction * distance;
        }
        return destination;
    }

    // returns a random destination, for when the gameobject is at the edge of the ground zone
    private static Vector3 GetDirectionWithinAngle(Vector3 targetForward, float angle, bool random = true) {
        // We want to get a random angle direction
        if (random)
            angle = Random.Range(-angle, angle);
        return Quaternion.AngleAxis(angle, Vector3.up) * targetForward;
    }

    // Returns true if the given destination is inside the ground zone
    public static bool IsPositionInsideGroundZone(Vector3 destination) {
        // -1 to let a little bit of space between the destination and the border
        if (destination.x >= render.bounds.min.x + 1 && destination.x <= render.bounds.max.x - 1 && destination.z >= render.bounds.min.z + 1 && destination.z <= render.bounds.max.z - 1)
            return true;
        return false;
    }
}
