using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raptor : MonoBehaviour {

    // All the possible raptor state's
    public static int flyHash = Animator.StringToHash("fly");
    public static int diveHash = Animator.StringToHash("dive");
    public static int flyAwayHash = Animator.StringToHash("fly away");

    public Material wingMaterial;

    private Animator animator;
    private GameObject prey;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    private void OnCollisionEnter(Collision collision) {
        // If we aren't busy and see a vulnerable suricate
        if (prey == null && collision.gameObject.CompareTag("Suricate") && !collision.gameObject.GetComponent<Suricate>().IsSafe()) {
            prey = collision.gameObject;
            animator.ResetTrigger(flyHash);
            animator.SetTrigger(diveHash);
        }
    }

    public GameObject GetPrey() {
        return prey;
    }
}
