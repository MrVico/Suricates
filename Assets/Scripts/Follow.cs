using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// For the babies!
public class Follow : SuricateBaseSM {

    /*
     * Change the growth to eating amount not time!
     * */
    private GameObject tutor;
    private float timer;
    private float babyTime = 15f;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        FindTutor();
        timer = 0;
        moveSpeed = 4f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        timer += Time.deltaTime;
        // After babyTime and if we ate enough we are no more baby!
        if(timer >= babyTime && obj.GetComponent<Suricate>().GetBabyGrowth() > 200f) {
            animator.SetBool("baby", false);
            animator.SetTrigger(Suricate.adultHash);
        }
        else if(tutor != null){
            // If the suricate is dead or no more hunter we need to find another one!
            if(tutor.GetComponent<Suricate>().IsDead() || !tutor.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter)) {
                FindTutor();
            }
            // We follow our tutor
            else if(Vector3.Distance(obj.transform.position, tutor.transform.position) > 1.5f) {
                Move(tutor.transform.position);
            }
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(timer >= babyTime && obj.GetComponent<Suricate>().GetBabyGrowth() > 200f) {
            // We are big!
            obj.SendMessage("GrownAssBaby");
        }
    }

    private void FindTutor() {
        List<GameObject> tutorCandidats = new List<GameObject>();
        foreach (GameObject candidat in GameObject.FindGameObjectsWithTag("Suricate")) {
            /*if(candidat.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter))
                Debug.Log("Name: "+candidat.name+" Distance: " + Vector3.Distance(candidat.transform.position, obj.transform.position));*/
            if (candidat.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter)
                 && !candidat.GetComponent<Suricate>().IsDead()
                 && Vector3.Distance(candidat.transform.position, obj.transform.position) < 30f) {
                tutorCandidats.Add(candidat);
            }
        }
        // We choose a hunter that's nearby
        if (tutorCandidats.Count > 0) {
            tutor = tutorCandidats.ElementAt(Random.Range(0, tutorCandidats.Count));
            tutor.SendMessage("TutorMe", obj);
        }
    }
}
