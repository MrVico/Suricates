using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuricateBaseSM : StateMachineBehaviour {

    public float wanderingRadius = 10f;
    public float wanderingTime = 3f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    public GameObject obj;

    private Quaternion rotation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        obj = animator.gameObject;
    }

    public void Move(Vector3 destination) {
        // If we are too close to the target it messes up the LookRotation function
        if(Vector3.Distance(obj.transform.position, destination) > 0.01f)
            rotation = RotateTowards(destination);
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        obj.transform.Translate(0, 0, Time.deltaTime * moveSpeed);
    }

    // Rotates and moves forward
    public void Move(Quaternion rotation) {
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        obj.transform.Translate(0, 0, Time.deltaTime * moveSpeed);
    }
    
    // Computes the rotation towards a destination
    public Quaternion RotateTowards(Vector3 destination) {
        Vector3 direction = destination - obj.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        // We only want to rotate left/right, so around the Y axis
        rotation.x = 0;
        rotation.z = 0;
        return rotation;
    }
}
