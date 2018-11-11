using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Suricate : MonoBehaviour {

    // All the possible suricate state's
    public static int chaseHash = Animator.StringToHash("chase");
    public static int wanderHash = Animator.StringToHash("wander");
    public static int herdHash = Animator.StringToHash("herd");
    public static int runHash = Animator.StringToHash("run");
    public static int deadHash = Animator.StringToHash("dead");
    
    public enum Type { Hunter, Sentinel, Baby };
    public enum Gender { Null, Male, Female };

    private Type suricateType;
    private Gender suricateGender;
    private bool alpha;
    public Material material;
    [Range(0, 1)]
    private float visionAlpha = 1f;
    private float visionAngle = 90f;
    private float visionRange = 5f;
    private int hideTime = 5;
    
    private Animator animator;
    private GameObject prey;
    private GameObject raptor;
    private GameObject[] holes;
    private bool safe;
    private float hideTimer;

    private GameObject FoV;

    private Slider infoBar;
    private float maxBarValue;
    private float currentBarValue;

    private bool dead;

    /*
        Gestation 11 semaines
        4 portées par an
        2-4 petits
        Les petits ne sortent du terrier qu'au bout de 21 jours
    */

    // 11 weeks
    private float pregnancyTime;
    private float timeSinceLastPregnancy;
    // Get pregnant after 3-4 months if there's an alpha male
    private float seedPlantingTime;

    // When you are the tutor of babies, you wait for them
    private bool wait;

    // TEST
    private bool mother = false;
    
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
            FoV = null;
        }
        else if (suricateType == Type.Baby) {
            GetComponent<Animator>().SetBool("baby", true);
            visionRange /= 2;
            FoV = CreateVisionField();
        }
        gameObject.name = "Suricate "+gameObject.name+" "+suricateType +" "+suricateGender;
        if (alpha) {
            gameObject.name += " Alpha";
            if (suricateGender == Gender.Female) {
                pregnancyTime = 0;
                timeSinceLastPregnancy = 0;
                // For now this is between 10s & 30s
                seedPlantingTime = 2/*Random.Range(10, 30)*/;
            }
        }
        dead = false;
        safe = false;
        MemoriseHoles();
        InitInfoBar();
    }

    // Update is called once per frame
    void Update() {
        if (!dead) {
            Debug.Log(name + " range: " + visionRange);
            if (suricateType == Type.Hunter) {
                detectCollision();
            }            
            // We want to stay safe for a while
            if (safe)
                hideTimer += Time.deltaTime;
            // After a certain time we come back out, should be the same for everyone!
            if (hideTimer >= hideTime) {
                animator.ResetTrigger(runHash);
                if (suricateType == Type.Hunter)
                    animator.SetTrigger(wanderHash);
                else if (suricateType == Type.Sentinel)
                    animator.SetTrigger(herdHash);
                else if (suricateType == Type.Baby)
                    animator.SetBool("baby", true);
                safe = false;
                hideTimer = 0;
            }
            UpdateInfoBar();
        }
        // We update the visionAlpha of the vision field in case the user changed it
        if (suricateType == Type.Hunter) {
            Color materialColor = FoV.GetComponent<MeshRenderer>().material.GetColor("_Color");
            materialColor.a = visionAlpha;
            FoV.GetComponent<MeshRenderer>().material.color = materialColor;
        }
        if (alpha && suricateGender == Gender.Female) {
            // We are not yet pregnant
            if (pregnancyTime == 0) {
                timeSinceLastPregnancy += Time.deltaTime;
                if (timeSinceLastPregnancy >= seedPlantingTime /*TEST*/ && !mother) {
                    // Congratulations! You are pregnant!
                    pregnancyTime = Time.deltaTime;
                }
            }
            // We are now pregnant
            else {
                //Debug.Log("PREGNANT");
                pregnancyTime += Time.deltaTime;
                // After 15s we deliver the brats
                if (pregnancyTime >= 3/*15f*/) {
                    pregnancyTime = 0;
                    timeSinceLastPregnancy = 0;
                    mother = true;
                    Debug.Log("BABIES");
                    FindObjectOfType<Spawner>().SendMessage("SpawnBabies", transform);
                }
            }
        }
    }

    // Called when a baby becomes an adult!
    private void GrownAssBaby() {
        transform.localScale *= 2;
        visionRange *= 2;
        gameObject.name = "Suricate " + gameObject.name + " " + suricateType + " " + suricateGender;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hole")) {
            //Debug.Log(name + " is safe!");
            safe = true;
        }
    }

    private void InitInfoBar() {
        infoBar = transform.GetComponentInChildren<Slider>();
        maxBarValue = 100f;
        currentBarValue = maxBarValue;
    }

    private void UpdateInfoBar() {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.5f));
        infoBar.transform.position = screenPosition;
        currentBarValue -= 0.1f;
        infoBar.value = currentBarValue / maxBarValue;
        // Dead
        if(currentBarValue <= 0) {
            Die();
        }
    }

    public bool IsDead(){
        return dead;
    }

    public void SetGender(Suricate.Gender gender){
        suricateGender = gender;
    }

    public Suricate.Gender GetGender(){
        return suricateGender;
    }

    public void IsAlpha(bool value){
        alpha = value;
    }

    public bool IsAlpha(){
        return alpha;
    }

    public void SetType(Suricate.Type type){
        suricateType = type;
    }

    public Suricate.Type GetSuricateType() {
        return suricateType;
    }
   
   // We are dead
    private void CaughtBy(GameObject raptor) {
        this.raptor = raptor;
        Debug.Log("RIP " + gameObject.name);
        // The eagle is taking it with him
        gameObject.transform.parent = raptor.transform;
        //obj.transform.position = new Vector3(0, -0.6f, 0.8f);
        Die();
    }

    public GameObject GetRaptor() {
        return raptor;
    }

    public GameObject GetPrey() {
        return prey;
    }
    
    // Called from Chase.cs is being eaten
    public void TakeABite(GameObject prey) {
        // The prey takes a hit!
        prey.SendMessage("Aww");
        // Babies don't need that much food
        if (suricateType == Suricate.Type.Baby)
            currentBarValue += 1.5f;
        else
            currentBarValue += 0.5f;
        if (currentBarValue > 100)
            currentBarValue = 100;
    }

    // The suricate died
    private void Die() {
        GetComponent<MeshRenderer>().material.color = Color.red;
        // We "disable" the vision field
        visionAlpha = 0;
        animator.ResetTrigger(Suricate.wanderHash);
        animator.ResetTrigger(Suricate.chaseHash);
        animator.ResetTrigger(Suricate.herdHash);        
        animator.SetTrigger(Suricate.deadHash);
        Debug.Log("Die");
        dead = true;
        // We hide the infobar
        transform.GetComponentInChildren<Canvas>().enabled = false;
        // Notify the spawner
        FindObjectOfType<Spawner>().SendMessage("SuricateDied", this);
        gameObject.name = "Dead Suricate";
    }

    // A raptor was spotted, run away!
    private void ToSafety() {
        // We can't run if we are dead!
        if(!dead){
            Debug.Log("RUUUUUUUN");
            animator.ResetTrigger(wanderHash);
            animator.ResetTrigger(chaseHash);
            animator.ResetTrigger(herdHash);
            animator.SetBool("baby", false);
            animator.SetTrigger(runHash);
        }
    } 

    public bool IsSafe() {
        return safe;
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
        // Set the visionAlpha component of the material's color to 0 so the FoV is transparent
        Color materialColor = visionCone.GetComponent<MeshRenderer>().material.GetColor("_Color");
        materialColor.a = visionAlpha;
        visionCone.GetComponent<MeshRenderer>().material.color = materialColor;

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
                // If we are a hunter and already chasing a prey we focus on that :)
                if (suricateType == Type.Hunter && prey == null && hit.collider.gameObject.CompareTag("Prey")) {
                    prey = hit.collider.gameObject;
                    Debug.Log("Chasing "+prey.name);
                    prey.SendMessage("SetEnemy", gameObject);
                    animator.ResetTrigger(wanderHash);
                    animator.SetTrigger(chaseHash);
                }
                // If we see another prey we notify it so it can run
                else if(suricateType == Type.Hunter && prey != null && hit.collider.gameObject.CompareTag("Prey")) {
                    if(hit.collider.gameObject != prey) {
                        hit.collider.gameObject.SendMessage("SetEnemy", gameObject);
                    }
                }
            }
            scanVector = Quaternion.Euler(0, rotAngle, 0) * direction;
            rotAngle += angleIncrementation;
        }
    }
}
