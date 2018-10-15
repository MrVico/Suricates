using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleBaseSM : StateMachineBehaviour {

    public float flyTime = 5f;
    public float rotationSpeed = 10f;
    // Full 360
    public float rotationAngle = 180f;
    public float moveDistance = 20f;
    public float moveSpeed = 2f;

    protected GameObject obj;
      
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        obj = animator.gameObject;
    }

    public void Move(string mode, Vector3 destination) {
        MovementController.Move(obj.transform, mode, destination, rotationSpeed, moveSpeed);
    }
}
