using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suricate : MonoBehaviour {

    // All the possible suricate state's
    public static int chaseHash = Animator.StringToHash("chase");
    public static int wanderHash = Animator.StringToHash("wander");

    public enum Type { Hunter, Sentinel };

    public Type suricateType;
    public Material material;
    [Range(0, 1)]
    public float alpha;

    private Animator animator;
    private GameObject prey;

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
        if (suricateType == Type.Hunter)
            GetComponent<Animator>().SetBool("hunter", true);
        else if (suricateType == Type.Sentinel)
            GetComponent<Animator>().SetBool("sentinel", true);
        CreateFieldOfVision();
    }

    private void CreateFieldOfVision() {
        GameObject FoV = new GameObject("FieldOfVision");
        //Add Components
        FoV.AddComponent<MeshRenderer>();
        FoV.AddComponent<MeshFilter>();
        FoV.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        FoV.GetComponent<MeshFilter>().mesh = mesh;
        FoV.GetComponent<MeshRenderer>().material = material;
        // Set the alpha component of the material's color to 0 so the FoV is transparent
        Color materialColor = GetComponent<MeshRenderer>().material.GetColor("_Color");
        materialColor.a = alpha;
        FoV.GetComponent<MeshRenderer>().material.color = materialColor;

        // This way the Field of Vision triangle is a little bit above ground level and can collide with small objects
        float groundLevel = -transform.lossyScale.y / 2 + 0.05f;

        Vector3[] vertices = new[] {
            new Vector3(0, groundLevel, 0),
            new Vector3(-4, groundLevel, 5),
            new Vector3(4, groundLevel, 5)
        };

        mesh.vertices = vertices;
        int[] triangles = new[] { 0, 1, 2 };
        mesh.triangles = triangles;
        // THIS ISN'T perfect, since the collider is a rectangle and not a triangle...
        FoV.GetComponent<MeshCollider>().sharedMesh = mesh;
        // Add the FoV as a child to the suricate
        FoV.transform.parent = gameObject.transform;
        // Set the local position to 0 so both the suricate's and the FoV's positions are the same
        FoV.transform.localPosition = new Vector3(0, 0, 0);
        // Add this script so that the collision are brought back here
        FoV.AddComponent<FoVCollision>();
    }

    // Not OnCollisionEnter 'cause this way we can directly switch to another target
    private void OnCollisionStay(Collision collision) {
        // If we are a hunter and already chasing a prey we focus on that :)
        if (suricateType == Type.Hunter && prey == null && collision.gameObject.CompareTag("Prey")) {
            prey = collision.gameObject;
            animator.ResetTrigger(wanderHash);
            animator.SetTrigger(chaseHash);
        }
    }

    public GameObject GetPrey() {
        return prey;
    }
}
