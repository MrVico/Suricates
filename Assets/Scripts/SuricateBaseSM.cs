using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuricateBaseSM : StateMachineBehaviour {

    public float wanderingRadius = 10f;
    public float wanderingTime = 3f;
    public float moveSpeed = 1.5f;
    public float moveDistance = 20f;
    public float rotationSpeed = 5f;
    public float rotationAngle = 70f;
    public float eatingTime = 1.5f;

    protected GameObject obj;

    private Quaternion rotation;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        obj = animator.gameObject;
    }

    public void Move(string mode, Vector3 destination) {
        MovementController.Move(obj.transform, mode, destination, rotationSpeed, moveSpeed);
    }
}
