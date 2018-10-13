using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFieldOfVision : MonoBehaviour {

    public Material material;
    [Range(0,1)]
    public int alpha;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Animator animator;
    private int chaseHash = Animator.StringToHash("chase");
    private int wanderHash = Animator.StringToHash("wander");
    private GameObject prey;

    // Use this for initialization
    void Start () {
        animator = transform.parent.GetComponent<Animator>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshRenderer>().material = material;
        // Set the alpha component of the material's color to 0 so the FoV is transparent
        Color materialColor = GetComponent<MeshRenderer>().material.GetColor("_Color");
        materialColor.a = alpha;
        GetComponent<MeshRenderer>().material.color = materialColor;

        // This way the Field of View triangle is a little bit above ground level and can collide with small objects
        float groundLevel = -transform.parent.lossyScale.y / 2 + 0.05f;

        vertices = new[] {
            new Vector3(0, groundLevel, 0),
            new Vector3(-4, groundLevel, 5),
            new Vector3(4, groundLevel, 5)
        };

        mesh.vertices = vertices;

        triangles = new[] { 0, 1, 2 };

        mesh.triangles = triangles;

        GetComponent<MeshCollider>().sharedMesh = mesh;
	}

    // THERE'S SOME BIG ISSUE HERE, DOESN'T ALWAYS WORK...
    // It only collides with the objects that are already inside the collider when the game starts...
    private void OnCollisionEnter(Collision collision) {
        // If we are already chasing a prey we focus on that :)
        if (prey == null && collision.gameObject.CompareTag("Prey")) {
            Debug.Log("Prey found");
            prey = collision.gameObject;
            Debug.Log("position: " + prey.transform.position);
            animator.ResetTrigger(wanderHash);
            animator.SetTrigger(chaseHash);
        }
    }

    public GameObject GetPrey() {
        return prey;
    }
}
