using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Suricate : MonoBehaviour {

    //DEBUG
    public bool debug = false;

    public enum Type { Hunter, Sentinel, Baby };
    public enum Gender { Null, Male, Female };

    // All the possible suricate state's
    public static int collectHash = Animator.StringToHash("collect");
    public static int chaseHash = Animator.StringToHash("chase");
    public static int wanderHash = Animator.StringToHash("wander");
    public static int herdHash = Animator.StringToHash("herd");
    public static int runHash = Animator.StringToHash("run");
    public static int deadHash = Animator.StringToHash("dead");
    public static int adultHash = Animator.StringToHash("adulthood");

    // Can all females have kids or just the alpha one?
    public static bool kidsForEveryone;

    private static int nbOfSuricates;
    private static bool everyoneSafe;
    private static float waitingForLastOne;

    private int suricateID;
    private Type suricateType;
    private Gender suricateGender;
    private bool alpha;
    public Material material;
    [Range(0, 1)]
    private float visionAlpha = 1f;
    private float visionAngle = 90f;
    private float visionRange = 5f;
    private int hideTime = 2;
    
    private Animator animator;
    private GameObject prey;
    private GameObject raptor;

    private GameObject[] holes;

    private Vector3 myPost;
    // Sentinel --> Hunter
    private bool backUpCalled;
    private Suricate predecessor;

    private bool alert;
    private bool safe;
    private float hideTimer;

    private GameObject FoV;

    private Slider infoBar;
    private float maxBarValue;
    private float currentBarValue;
    private float amountEaten;

    private bool dead;

    // The bigger this number the faster they starve
    private float hungerRate = 0.01f;

    // 11 weeks
    private float pregnancyTime;
    private float timeSinceLastPregnancy;
    // Get pregnant after 3-4 months if there's an alpha male
    private float seedPlantingTime;
    private float minSeed = 10f;
    private float maxSeed = 30f;
    private float pregnancyDuration = 15f;

    private GameObject tutor;
    // The amount the baby ate
    private float babyGrowthEating = 0f;

    // When you are the tutor
    private bool collectingBabies;
    private List<GameObject> youths;

    // Use this for initialization
    void Start() {
        nbOfSuricates++;
        suricateID = nbOfSuricates;
        animator = GetComponent<Animator>();
        if (suricateType == Type.Hunter) {
            animator.SetBool("hunter", true);
            animator.SetTrigger(wanderHash);
        }
        else if (suricateType == Type.Sentinel) {
            animator.SetBool("sentinel", true);
            animator.SetTrigger(herdHash);
            myPost = FindObjectOfType<Spawner>().GetUnoccupiedSentinelPost();
        }
        else if (suricateType == Type.Baby) {
            animator.SetBool("baby", true);
            visionRange /= 2;
        }
        gameObject.name = "Suricate "+ suricateID + " "+suricateType +" "+suricateGender;
        if (alpha) {
            gameObject.name += " Alpha";
        }
        // A female can become an alpha female and needs this variable to be initialized!
        if (suricateGender == Gender.Female) {
            pregnancyTime = 0;
            timeSinceLastPregnancy = 0;
            seedPlantingTime = Random.Range(minSeed, maxSeed);
        }
        FoV = CreateVisionField();
        dead = false;
        alert = false;
        safe = false;
        youths = new List<GameObject>();
        backUpCalled = false;
        MemoriseHoles();
        InitFullnessBar();
    }

    // Update is called once per frame
    void Update() {
		if (!dead) {
            if (!alert && suricateType == Type.Hunter && !collectingBabies) {
                detectCollision();
            }    
            // If we are alert && ALL the suricates are home we start the hide timer
            if(alert && !everyoneSafe && (FindObjectOfType<Spawner>().GetNumberOfSafeSuricates() == GameObject.FindGameObjectsWithTag("Suricate").Length)) {
                everyoneSafe = true;
            }
            if(debug){
                Debug.Log("Alert: "+alert+" collecting: "+ collectingBabies+" type: "+suricateType+" prey: "+prey);
            }
			// Once everyone is safe we wait it out
			if (everyoneSafe)
				hideTimer += Time.deltaTime;
            // After a certain time we come back out, everyone at once!
            if (hideTimer >= hideTime) {
                alert = false;
                animator.ResetTrigger(runHash);
                if (suricateType == Type.Hunter)
                    animator.SetTrigger(wanderHash);
                else if (suricateType == Type.Sentinel)
                    animator.SetTrigger(herdHash);
                else if (suricateType == Type.Baby)
                    animator.SetBool("baby", true);
                // reset
                hideTimer = 0;
                everyoneSafe = false;
            }
            if(alert && !safe){
                animator.ResetTrigger(wanderHash);
                animator.SetTrigger(runHash);
            }
            UpdateFullnessBar();
            if (!alert && (alpha || (!alpha && kidsForEveryone)) && suricateGender == Gender.Female && suricateType != Suricate.Type.Baby && suricateType != Suricate.Type.Sentinel) {
                // We are not yet pregnant and don't have kids to look after
                if (pregnancyTime == 0 && youths.Count == 0) {
                    timeSinceLastPregnancy += Time.deltaTime;
                    if (timeSinceLastPregnancy >= seedPlantingTime) {
                        // Congratulations! You are pregnant!
                        pregnancyTime = Time.deltaTime;
                    }
                }
                // We are now pregnant
                else if(pregnancyTime > 0){
                    pregnancyTime += Time.deltaTime;
                    // After pregnancyDuration we deliver the brats
                    if (pregnancyTime >= pregnancyDuration) {
                        pregnancyTime = 0;
                        timeSinceLastPregnancy = 0;
                        FindObjectOfType<Spawner>().SendMessage("SpawnBabies", transform);
                    }
                }
            }
            // We need to check if they didn't grow up, if so, remove them from the list
            if(youths.Count > 0) {
                // ToArray() to create a copy, thus the live modification to the list won't affect us
                foreach(GameObject youth in youths.ToArray()) {
                    // The youth could've died
                    if (youth != null && !youth.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Baby)) {
                        youths.Remove(youth);
                    }
                }
            }
        }
        // This way the dead suricate is shown properly
        else if(dead && raptor != null && transform.localPosition != new Vector3(0, -0.6f, 0.8f)) {
            transform.localPosition = new Vector3(0, -0.6f, 0.8f);
        }
	}

    // Called when a baby becomes an adult!
    private void GrownAssBaby() {
        transform.localScale *= 2;
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        visionRange *= 2;
        GameObject visionCone = transform.Find("Vision Cone").gameObject;
        visionCone.transform.localPosition = new Vector3(0f, -0.3f, 0f);
        // We've to be a sentinel 'cause the old ones died
        if(FindObjectOfType<Spawner>().GetNumberOfPostedSentinels() < 2){
            gameObject.GetComponent<Suricate>().SetSuricateType(Suricate.Type.Sentinel);
            animator.SetBool("sentinel", true);
            if(!alert)
                animator.SetTrigger(Suricate.herdHash);
        }
        else{
            gameObject.GetComponent<Suricate>().SetSuricateType(Suricate.Type.Hunter);
            animator.SetBool("hunter", true);
            if(!alert)
                animator.SetTrigger(Suricate.wanderHash);
        }
        gameObject.name = "Suricate " + suricateID + " " + suricateType + " " + suricateGender;
        // The baby can well be an alpha!
        if (alpha)
            gameObject.name += " Alpha";
    }

    // We enter our home, we are safe
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Hole")) {
            safe = true;
        }
    }

    // We exit our home, into the dangerous world!
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Hole")) {
            safe = false;
        }
    }

    private void InitFullnessBar() {
        infoBar = transform.GetComponentInChildren<Slider>();
        maxBarValue = 100f;
        currentBarValue = maxBarValue;
        amountEaten = 0;
    }

    private void UpdateFullnessBar() {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1.5f));
        infoBar.transform.position = screenPosition;
        currentBarValue -= hungerRate * Time.timeScale;
        infoBar.value = currentBarValue / maxBarValue;
        // If we are a hungry sentinel on post we want to go hunt!
        if(!alert && !backUpCalled && GetSuricateType().Equals(Suricate.Type.Sentinel) && currentBarValue < 40f && transform.position == myPost) {
            GameObject candidat = null;
            // We select the closest hunter that isn't starving to relieve us
            foreach(GameObject suricate in GameObject.FindGameObjectsWithTag("Suricate")) {
                if(suricate.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter) 
                && !suricate.GetComponent<Suricate>().IsAlpha()
                && suricate.GetComponent<Suricate>().GetFullness() > 60f
                &&  ((candidat == null) || (candidat != null && Vector3.Distance(suricate.transform.position, transform.position) < Vector3.Distance(candidat.transform.position, transform.position))) ) {
                    if(candidat != null)
                        candidat.GetComponent<Suricate>().SetSuricateType(Suricate.Type.Hunter);
                    candidat = suricate;
                    candidat.GetComponent<Suricate>().SetSuricateType(Suricate.Type.Sentinel);
                }
            }
            // If we found someone to relieve us from our duty
            if (candidat != null) {
                backUpCalled = true;
                // We give him our post!
                candidat.SendMessage("OnSentinelDuty", this);
            }
        }
        // Dead
        if(currentBarValue <= 0) {
            Die();
        }
    }

    public Suricate GetPredecessor() {
        return predecessor;
    }

    public void OnSentinelDuty(Suricate sentinel) {
        // A sentinel doesn't have kids to look after
        youths.Clear();
        predecessor = sentinel;
        myPost = sentinel.GetPost();
        suricateType = Type.Sentinel;
        animator.SetBool("hunter", false);
        animator.SetBool("sentinel", true);
        animator.ResetTrigger(wanderHash);
        animator.ResetTrigger(chaseHash);
        // Only if we aren't under attack
        if(!alert)
            animator.SetTrigger(herdHash);
        gameObject.name = "Suricate " + suricateID + " " + suricateType + " " + suricateGender;
    }

    public Vector3 GetPost() {
        return myPost;
    }

    public void RelievedFromPost() {
        backUpCalled = false;
        suricateType = Type.Hunter;
        animator.SetBool("sentinel", false);
        animator.SetBool("hunter", true);
        animator.ResetTrigger(herdHash);
        // We want to run right now not wander!
        if(!alert)
            animator.SetTrigger(wanderHash);
        gameObject.name = "Suricate " + suricateID + " " + suricateType + " " + suricateGender;
    }

    public float GetFullness() {
        return currentBarValue;
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

    public void SetSuricateType(Suricate.Type type){
        suricateType = type;
    }

    public Suricate.Type GetSuricateType() {
        return suricateType;
    }
   
   // We are dead
    private void CaughtBy(GameObject raptor) {
        this.raptor = raptor;
        // The eagle is taking us with him
        gameObject.transform.parent = raptor.transform;
        // Notify everyone of the danger!
        foreach (GameObject suricate in GameObject.FindGameObjectsWithTag("Suricate")) {
            suricate.SendMessage("ToSafety");
        }
        Die();
    }

    public GameObject GetRaptor() {
        return raptor;
    }

    public GameObject GetPrey() {
        return prey;
    }

    public void TutorMe(GameObject baby) {       
        collectingBabies = true;
        youths.Add(baby);
        animator.ResetTrigger(Suricate.chaseHash);
        animator.ResetTrigger(Suricate.wanderHash);
        animator.SetTrigger(Suricate.collectHash);
    }

    public void SetTutor(GameObject tutor){
        this.tutor = tutor;
    }

    public GameObject GetTutor(){
        return tutor;
    }

    public void CollectFinished() {
        collectingBabies = false;
    }

    public List<GameObject> GetYouths() {
        List<GameObject> actualYouth = new List<GameObject>();
        foreach(GameObject youth in youths){
            // Only our youth if we are its tutor
            if(youth != null && youth.GetComponent<Suricate>().GetTutor() == gameObject){
                actualYouth.Add(youth);
            }
        }
        return actualYouth;
    }
    
    // Called from Chase.cs is being eaten
    public void TakeABite(GameObject prey) {
        // The prey takes a hit!
        prey.SendMessage("Aww");
        // Babies don't need that much food
        if (suricateType == Suricate.Type.Baby) {
            currentBarValue += 1.5f;
            babyGrowthEating += 1;
        }
        else{
            currentBarValue += 0.5f;
        }
        if (currentBarValue > 100)
            currentBarValue = 100;
        // Needed to determine the new alpha female
        amountEaten++;
    }

    public bool IsOnAlert(){
        return alert;
    }

    public float GetAmountEaten(){
        return amountEaten;
    }

    public float GetBabyGrowth() {
        return babyGrowthEating;
    }

    // The suricate died
    private void Die() {
        // We destroy the vision field
        if(transform.Find("Vision Cone") != null)
            Destroy(transform.Find("Vision Cone").gameObject);
        animator.ResetTrigger(Suricate.wanderHash);
        animator.ResetTrigger(Suricate.chaseHash);
        animator.ResetTrigger(Suricate.herdHash);        
        animator.SetTrigger(Suricate.deadHash);
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
        if(!dead && !alert) {
            alert = true;
            animator.ResetTrigger(wanderHash);
            animator.ResetTrigger(chaseHash);
            animator.ResetTrigger(herdHash);
            animator.ResetTrigger(collectHash);
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
                if(debug)
                    Debug.Log("Collision Prey: "+prey+" Collider: " + hit.collider.gameObject.name+" Tag: "+ hit.collider.gameObject.tag);
                // If we are a hunter and already chasing a prey we focus on that :)
                if (suricateType == Type.Hunter && prey == null && hit.collider.gameObject.CompareTag("Prey")) {
                    prey = hit.collider.gameObject;
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