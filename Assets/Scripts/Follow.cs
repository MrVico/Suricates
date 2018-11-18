using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// For the babies!
public class Follow : SuricateBaseSM {

    // Shouldn't always be the alpha female, but way too complicated to do otherwise
    private GameObject tutor;
    private float timer;
    private float babyTime = 10f;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        List<GameObject> tutorCandidats = new List<GameObject>();
        foreach (GameObject candidat in GameObject.FindGameObjectsWithTag("Suricate")) {
            /*if(candidat.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter))
                Debug.Log("Name: "+candidat.name+" Distance: " + Vector3.Distance(candidat.transform.position, obj.transform.position));*/
            if (candidat.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter)
                 && !candidat.GetComponent<Suricate>().IsDead()
                 && Vector3.Distance(candidat.transform.position, obj.transform.position) < 20f) {
                tutorCandidats.Add(candidat);
            }
        }
        // We choose a hunter that's nearby
        tutor = tutorCandidats.ElementAt(Random.Range(0, tutorCandidats.Count));
        tutor.SendMessage("TutorMe", obj);
        timer = 0;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        timer += Time.deltaTime;
        // After babyTime we are no more baby!
        if(timer >= babyTime) {
            animator.SetBool("baby", false);
            animator.SetBool("hunter", true);
            animator.SetTrigger(Suricate.wanderHash);
        }
        if(Vector3.Distance(obj.transform.position, tutor.transform.position) > 1.5f) {
            Move(tutor.transform.position);
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(timer >= babyTime) {
            // We are big!
            obj.SendMessage("GrownAssBaby");
        }
    }
}
