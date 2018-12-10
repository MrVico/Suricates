using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaptorBaseSM : StateMachineBehaviour {

    protected float flyTime = 6f;
    protected float flyTimer = 0f;
    protected float rotationSpeed = 10f;
    protected float rotationAngle = 30f;
    protected float moveDistance = 30f;
    protected float moveSpeed = 3f;
    protected float flyHeight = 10f;

    protected GameObject obj;
      
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        obj = animator.gameObject;
    }

    public void Move(Vector3 destination) {
        MovementController.Move(obj.transform, destination, rotationSpeed, moveSpeed);
    }
}
