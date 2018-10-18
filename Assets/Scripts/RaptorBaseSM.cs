using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaptorBaseSM : StateMachineBehaviour {

    protected float flyTime = 5f;
    protected float rotationSpeed = 10f;
    // Full 360
    protected float rotationAngle = 180f;
    protected float moveDistance = 20f;
    protected float moveSpeed = 3f;

    protected GameObject obj;
      
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        obj = animator.gameObject;
    }

    public void Move(Vector3 destination) {
        MovementController.Move(obj.transform, destination, rotationSpeed, moveSpeed);
    }
}
