using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfVision : MonoBehaviour {

    public Material material;
    [Range(0,1)]
    public float alpha;
    
    private Animator animator;
    private int chaseHash = Animator.StringToHash("chase");
    private int wanderHash = Animator.StringToHash("wander");
    private GameObject prey;
    private Suricate.Type suricateType;

    // Use this for initialization
    void Start () {
        animator = transform.parent.GetComponent<Animator>();
        suricateType = transform.parent.GetComponent<Suricate>().GetSuricateType();
        CreateFieldOfVision();
	}

    private void CreateFieldOfVision() {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
        // Set the alpha component of the material's color to 0 so the FoV is transparent
        Color materialColor = GetComponent<MeshRenderer>().material.GetColor("_Color");
        materialColor.a = alpha;
        GetComponent<MeshRenderer>().material.color = materialColor;

        // This way the Field of View triangle is a little bit above ground level and can collide with small objects
        float groundLevel = -transform.parent.lossyScale.y / 2 + 0.05f;

        Vector3[] vertices = new[] {
            new Vector3(0, groundLevel, 0),
            new Vector3(-4, groundLevel, 5),
            new Vector3(4, groundLevel, 5)
        };

        mesh.vertices = vertices;
        int[] triangles = new[] { 0, 1, 2 };
        mesh.triangles = triangles;
        // THIS ISN'T perfect, since the collider is a rectangle and not a triangle...
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Not OnCollisionEnter 'cause this way we can directly switch to another target
    private void OnCollisionStay(Collision collision) {
        // If we are a hunter and already chasing a prey we focus on that :)
        if (suricateType == Suricate.Type.Hunter && prey == null && collision.gameObject.CompareTag("Prey")) {
            prey = collision.gameObject;
            animator.ResetTrigger(wanderHash);
            animator.SetTrigger(chaseHash);
        }
    }

    public GameObject GetPrey() {
        return prey;
    }
}
