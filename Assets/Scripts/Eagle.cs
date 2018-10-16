using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour {

    // All the possible eagle state's
    public static int flyHash = Animator.StringToHash("fly");
    public static int diveHash = Animator.StringToHash("dive");
    public static int flyAwayHash = Animator.StringToHash("fly away");

    public Material wingMaterial;

    private Animator animator;
    private GameObject prey;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        //AddWings();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // UNUSED
    private void AddWings() {
        GameObject wings = new GameObject("Wings");
        //Add Components
        wings.AddComponent<MeshRenderer>();
        wings.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        wings.GetComponent<MeshFilter>().mesh = mesh;
        wings.GetComponent<MeshRenderer>().material = wingMaterial;

        // This way the Field of Vision triangle is a little bit above ground level and can collide with small objects
        float groundLevel = -transform.lossyScale.y / 2 + 0.05f;

        Vector3[] vertices = new[] {
            new Vector3(-1.5f, 0, 0.5f),
            new Vector3(1.5f, 0, 0.5f),
            new Vector3(0, 0, -0.5f)
        };

        mesh.vertices = vertices;
        int[] triangles = new[] { 0, 1, 2 };
        mesh.triangles = triangles;
        // Add the wings as a child to the eagle
        wings.transform.parent = transform;
        // Set the local position to 0 so both the suricate's and the FoV's positions are the same
        wings.transform.localPosition = new Vector3(0, 0, 0);
    }
    
    private void OnCollisionStay(Collision collision) { 
        //Debug.Log("Eagle collision with "+collision.gameObject.name+" of type "+collision.gameObject.tag);
        // If we are already chasing a prey we focus on that :)
        if (prey == null && collision.gameObject.CompareTag("Suricate")) {
            prey = collision.gameObject.transform.parent.gameObject;
            animator.ResetTrigger(flyHash);
            animator.SetTrigger(diveHash);
        }
    }

    public GameObject GetPrey() {
        return prey;
    }
}
