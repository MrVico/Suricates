using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController {

    public static readonly string CHASE = "chase";
    public static readonly string WANDER = "wander";

    public static void Move(Transform objTransform, string mode, Vector3 destination, float rotationSpeed, float moveSpeed) {
        Quaternion rotation = RotateTowards(objTransform, mode, destination);
        objTransform.rotation = Quaternion.Slerp(objTransform.rotation, rotation, rotationSpeed * Time.deltaTime);
        objTransform.Translate(0, 0, Time.deltaTime * moveSpeed);
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

    public static Vector3 GetNewDestination(Vector3 direction, float rotationAngle, float distance) {
        Vector3 destination = GetDirectionWithinAngle(direction, rotationAngle) * distance;
        //Debug.DrawRay(obj.transform.position, destination, Color.red, 3f);
        return destination;
    }

    private static Vector3 GetDirectionWithinAngle(Vector3 targetDirection, float angle) {
        float angleInRad = Random.Range(0.0f, angle) * Mathf.Deg2Rad;
        Vector2 pointOnCircle = (Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
        Vector3 directionModifier = new Vector3(pointOnCircle.x, 0, Mathf.Cos(angleInRad));
        return Quaternion.LookRotation(targetDirection) * directionModifier;
    }
}
