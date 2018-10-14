using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuricateBaseSM : StateMachineBehaviour {

    public float wanderingRadius = 10f;
    public float wanderingTime = 3f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float rotationAngle = 70f;
    public float eatingTime = 1.5f;

    protected GameObject obj;

    protected readonly string CHASE = "chase";
    protected readonly string WANDER = "wander";

    private Quaternion rotation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        obj = animator.gameObject;
    }

    public void Move(string mode, Vector3 destination) {
        // If we are too close to the target it messes up the LookRotation function
        if(Vector3.Distance(obj.transform.position, destination) > 0.01f)
            rotation = RotateTowards(mode, destination);
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        obj.transform.Translate(0, 0, Time.deltaTime * moveSpeed);
    }

    // Rotates and moves forward
    public void Move(Quaternion rotation) {
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        obj.transform.Translate(0, 0, Time.deltaTime * moveSpeed);
    }
    
    // Computes the rotation towards a destination
    public Quaternion RotateTowards(string mode, Vector3 destination) {
        if(mode.Equals(CHASE))
            destination = destination - obj.transform.position;
        Quaternion rotation = Quaternion.LookRotation(destination);
        // We only want to rotate left/right, so around the Y axis
        rotation.x = 0;
        rotation.z = 0;
        return rotation;
    }
}
