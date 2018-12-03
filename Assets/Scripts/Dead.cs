using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead : StateMachineBehaviour {

	[SerializeField] Material deadMat;

	private MeshRenderer bodyMesh;
	private bool caught = false;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bodyMesh = animator.GetComponent<MeshRenderer>();
		animator.GetComponent<Renderer>().GetComponent<Renderer>().material = deadMat;
		if(animator.GetComponent<Suricate>().GetRaptor() != null)
			caught = true;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// Only if we died of hunger
		if(!caught){
			Color materialColor = bodyMesh.material.GetColor("_Color");
			materialColor.a = materialColor.a - Time.deltaTime * 0.25f;
			if(materialColor.a < 0){
				Destroy(animator.gameObject);
			}
			else{
				bodyMesh.material.color = materialColor;
			}
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	
	}
}
