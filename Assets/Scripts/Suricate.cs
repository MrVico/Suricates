using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suricate : MonoBehaviour {

    // All the possible suricate state's
    public static int chaseHash = Animator.StringToHash("chase");
    public static int wanderHash = Animator.StringToHash("wander");
    public static int herdHash = Animator.StringToHash("herd");
    public static int runHash = Animator.StringToHash("run");
    public static int deadHash = Animator.StringToHash("dead");

    public enum Type { Hunter, Sentinel };

    public Type suricateType;
    public Material material;
    [Range(0, 1)]
    public float alpha;
    public int visionAngle = 90;
    public int visionRange = 5;

    private Animator animator;
    private GameObject prey;
    private GameObject raptor;
    private GameObject[] holes;
    private bool safe;

    private GameObject FoV;  

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
        if (suricateType == Type.Hunter) {
            GetComponent<Animator>().SetBool("hunter", true);
            // Only on the hunter 'cause the sentinel has better sight
            FoV = CreateVisionField();
        }
        else if (suricateType == Type.Sentinel) {
            GetComponent<Animator>().SetBool("sentinel", true);
        }
        safe = false;
        MemoriseHoles();
    }

    // Update is called once per frame
    void Update() {
        // We only want to scan for a prey if we haven't already got one
        if(prey == null && suricateType == Type.Hunter /*For now*/)
            detectCollision();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hole")) {
            Debug.Log(name + " is safe!");
            safe = true;
        }
    }

    private void CaughtBy(GameObject raptor) {
        this.raptor = raptor;
    }

    public GameObject GetRaptor() {
        return raptor;
    }

    public GameObject GetPrey() {
        return prey;
    }

    // Every suricate knows all the holes made as homes
    private void MemoriseHoles() {
        holes = GameObject.FindGameObjectsWithTag("Hole");
    }

    // The suricate want to hide quickly
    public GameObject GetNearestHole() {
        GameObject nearest = holes[0];
        foreach (GameObject hole in holes) {
            // TODO: Skip the first one since we already got it
            if (Vector3.Distance(animator.transform.position, hole.transform.position) < Vector3.Distance(animator.transform.position, nearest.transform.position))
                nearest = hole;
        }
        return nearest;
    }

    // A raptor was spotted, run away!
    private void ToSafety() {
        Debug.Log("RUUUUUUUN");
        animator.ResetTrigger(wanderHash);
        animator.ResetTrigger(chaseHash);
        animator.ResetTrigger(herdHash);
        animator.SetTrigger(runHash);
    } 

    public bool IsSafe() {
        return safe;
    }

    // Source : https://www.youtube.com/watch?v=FShHFmEFFkg
    private GameObject CreateVisionField() {
        GameObject visionCone = new GameObject("Vision Cone");
        //Add Components
        visionCone.AddComponent<MeshRenderer>();
        visionCone.AddComponent<MeshFilter>();
        visionCone.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        visionCone.GetComponent<MeshFilter>().mesh = mesh;
        visionCone.GetComponent<MeshRenderer>().material = material;
        // Set the alpha component of the material's color to 0 so the FoV is transparent
        Color materialColor = GetComponent<MeshRenderer>().material.GetColor("_Color");
        materialColor.a = alpha;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();

        Vector3 temp = Vector3.zero;

        vertices.Add(Vector3.zero);
        normals.Add(Vector3.up);
        uv.Add(Vector2.one * 0.5f);

        int w, s;
        for (w = 0; w < visionAngle; w++) {
            for (s = 0; s < visionRange; s++) {
                temp.x = Mathf.Cos(Mathf.Deg2Rad * w + Mathf.Deg2Rad * (s / visionRange)) * visionRange;
                temp.z = Mathf.Sin(Mathf.Deg2Rad * w + Mathf.Deg2Rad * (s / visionRange)) * visionRange;                
                // For some reason this doesn't always look ahead...
                //vertices.Add(new Vector3(temp.x, temp.y, temp.z));
                vertices.Add(Quaternion.Euler(0, -45, 0) * new Vector3(temp.x, temp.y, temp.z));
                normals.Add(Vector3.up);
                uv.Add(new Vector2((visionRange + temp.x) / (visionRange * 2), (visionRange + temp.z) / (visionRange * 2)));
            }
        }

        int[] triangles = new int[(vertices.Count - 2) * 3];
        s = 0;
        for (w = 1; w < (vertices.Count - 2); w++) {
            triangles[s++] = w + 1;
            triangles[s++] = w;
            triangles[s++] = 0;
        }
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles;
        // Add the FoV as a child to the suricate
        visionCone.transform.parent = transform;
        // Set the local position to 0 so both the suricate's and the FoV's positions are the same
        visionCone.transform.localPosition = new Vector3(0, -(transform.localScale.y/2)+0.2f, 0);
        return visionCone;
    }
    

    // Detects the collision by scanning the vision field with Raycasts
    private void detectCollision() {
        Vector3 direction = transform.forward;
        Vector3 scanVector = Quaternion.Euler(0, -45, 0) * direction;
        float angleIncrementation = 0.5f;
        RaycastHit hit;
        float rotAngle = -visionAngle/2;
        // Raycasts from -45 to 45 each 0.5
        while (rotAngle < visionAngle/2) {
            //Debug.DrawRay(FoV.transform.position, scanVector * range, Color.red, 0.16f);
            if (Physics.Raycast(FoV.transform.position, scanVector, out hit, visionRange)) {
                Debug.Log("Hit: " + hit.collider.name);
                // If we are a hunter and already chasing a prey we focus on that :)
                if (suricateType == Type.Hunter && prey == null && hit.collider.gameObject.CompareTag("Prey")) {
                    prey = hit.collider.gameObject;
                    animator.ResetTrigger(wanderHash);
                    animator.SetTrigger(chaseHash);
                    return;
                }
            }
            scanVector = Quaternion.Euler(0, rotAngle, 0) * direction;
            rotAngle += angleIncrementation;
        }
    }
}
