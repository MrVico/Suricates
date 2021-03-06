﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Follow : SuricateBaseSM {

    private GameObject tutor;
    private float timer;
    // Minimum time it stays a baby
    private float babyTime = 15f;
    // Minimum eating requirements to be able to grow up
    private float eatingGoal = 150f;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        FindTutor();
        timer = 0;
        moveSpeed = 1.5f;
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        timer += Time.deltaTime;
        // After babyTime and if we ate enough we are no more baby!
        if(timer >= babyTime && obj.GetComponent<Suricate>().GetBabyGrowth() > eatingGoal) {
            animator.SetBool("baby", false);
            animator.SetTrigger(Suricate.adultHash);
        }
        else if(tutor != null){
            // If the suricate is dead or no more hunter we need to find another one!
            if(tutor.GetComponent<Suricate>().IsDead() || !tutor.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter)) {
                FindTutor();
            }
            // We go join our tutor
            else if(Vector3.Distance(obj.transform.position, tutor.transform.position) > 3f) {
                moveSpeed = 4f;
                Move(tutor.transform.position);
            }
            // We follow our tutor
            else if(Vector3.Distance(obj.transform.position, tutor.transform.position) > 1.5f) {
                moveSpeed = 1.5f;
                Move(tutor.transform.position);
            }
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(timer >= babyTime && obj.GetComponent<Suricate>().GetBabyGrowth() > eatingGoal) {
            // We are big!
            obj.SendMessage("GrownAssBaby");
        }
    }

    // Finds the baby a tutor that he can follow and get food from
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
            obj.GetComponent<Suricate>().SetTutor(tutor);
        }
    }
}
