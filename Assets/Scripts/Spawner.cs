using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour {

    public static int totalSuricates;
    public static int aliveSuricates;

    [SerializeField] GameObject suricatePrefab;
    [SerializeField] GameObject raptorPrefab;
    [SerializeField] GameObject preyPrefab;
    [SerializeField] Material alphaMaleMaterial;
    [SerializeField] Material alphaFemaleMaterial;
    [SerializeField] float raptorRespawnTime;

    private int nbOfPreys;
    private int initialColonySize;

    // For testing purposes only
    public bool spawnSuricates;
    // Just for testing purposes
    private int totalPreys;

    private List<GameObject> suricates;

    private GameObject preyContainer;
    private List<GameObject> preys;

    private List<Vector3> sentinelPosts;

    private float timer;

    private bool simulationStarted = false;   
    private int chosenPredatorRespawnTime;
    
    // The ground zone boundaries
    private Bounds groundBoundaries;

    void Awake(){
        groundBoundaries = GameObject.FindGameObjectWithTag("Ground").GetComponent<Renderer>().bounds;
        sentinelPosts = new List<Vector3>(new Vector3[] { new Vector3(0, 0.5f, -24), new Vector3(0, 0.5f, 24) });
    }

	// Update is called once per frame
	void Update () {
        if(simulationStarted){
            timer += Time.deltaTime;
            // We always want to have a certain amount of preys alive, so we respawn the dead ones
            for(int i=GameObject.FindGameObjectsWithTag("Prey").Length; i<nbOfPreys; i++){
                SpawnPrey(false);
            }
            if(timer >= raptorRespawnTime){
                SpawnRaptor();
                raptorRespawnTime = Random.Range(chosenPredatorRespawnTime - chosenPredatorRespawnTime/10, chosenPredatorRespawnTime + chosenPredatorRespawnTime/10);
                timer = 0;
            }
        }
	}

    public void SetUpSimulation(int nbOfSuricates, int predatorReSpawnTime, int nbOfPreys, bool kidsForEveryone){
        initialColonySize = nbOfSuricates;
        chosenPredatorRespawnTime = predatorReSpawnTime;
        raptorRespawnTime = Random.Range(predatorReSpawnTime - predatorReSpawnTime/10, predatorReSpawnTime + predatorReSpawnTime/10);
        this.nbOfPreys = nbOfPreys;
        Suricate.kidsForEveryone = kidsForEveryone;
        simulationStarted = true;
        // Set up
        totalSuricates = 0;
        aliveSuricates = 0;
        timer = 0;
        suricates = new List<GameObject>();
        if (spawnSuricates)
            InitialSuricateSpawn();
    }

    public int GetNumberOfPostedSentinels(){
        int amount = 0;
        foreach(GameObject suricate in GameObject.FindGameObjectsWithTag("Suricate")){
            if(suricate.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Sentinel)){
                amount++;
            }
        }
        return amount;
    }

    // Called from ManageData to set up the food
    public void SpawnPreys(int nbOfPreys){
        preys = new List<GameObject>();
        preyContainer = GameObject.FindGameObjectWithTag("PreyContainer");
        totalPreys = 0;
        // Clear
        foreach(GameObject prey in GameObject.FindGameObjectsWithTag("Prey")){
            Destroy(prey);
        }
        for(int i=0; i<nbOfPreys; i++) {
            SpawnPrey(true);
        }
    }
    
    private void SpawnPrey(bool initialSpawn) {
        Vector3 position;
        if(initialSpawn){
            position = new Vector3(Random.Range(groundBoundaries.min.x+2, groundBoundaries.max.x-2), 
                                            preyPrefab.transform.localScale.y/2, 
                                            Random.Range(groundBoundaries.min.z+2, groundBoundaries.max.z-2));
        }
        // We spawn them a little more to the center
        else{
            position = new Vector3(Random.Range(groundBoundaries.center.x+20, groundBoundaries.center.x-20), 
                                            preyPrefab.transform.localScale.y/2, 
                                            Random.Range(groundBoundaries.center.z+15, groundBoundaries.center.z-15));
        }
        GameObject prey = Instantiate(preyPrefab, position, Quaternion.identity);
        preys.Add(prey);
        totalPreys++;
        prey.name = "Prey " + totalPreys;
        prey.transform.parent = preyContainer.transform;
    }

    private void PreyDied(GameObject prey) {
        preys.Remove(prey);
    }

    private void SuricateDied(Suricate suricate){
        aliveSuricates--;
        // This way the suricate is seen as dead by the game straight away
        suricate.tag = "Untagged";
        // If an alpha died we need to elect a new one
        if (suricate.IsAlpha()){
            // If it's a male, the first male suricate becomes the new alpha
            if(suricate.GetGender().Equals(Suricate.Gender.Male)){      
                foreach(GameObject candidat in GameObject.FindGameObjectsWithTag("Suricate")){
                    if (candidat.GetComponent<Suricate>().GetGender().Equals(suricate.GetGender())
                        && !candidat.GetComponent<Suricate>().IsDead()){
                            candidat.GetComponent<Suricate>().IsAlpha(true);
                            candidat.name += " Alpha";
                            candidat.GetComponent<Renderer>().material = alphaMaleMaterial;
                            return;
                    }
                }
            }
            // If it's a female we need to get the biggest female to replace her
            else{
                GameObject newAlphaFemale = null;
                foreach(GameObject candidat in GameObject.FindGameObjectsWithTag("Suricate")){
                    if (candidat.GetComponent<Suricate>().GetGender().Equals(suricate.GetGender())
                        && !candidat.GetComponent<Suricate>().IsDead()){
                            if(newAlphaFemale == null){
                                newAlphaFemale = candidat;
                            }
                            // If the candidat ate more it's bigger!
                            else if(candidat.GetComponent<Suricate>().GetAmountEaten() > newAlphaFemale.GetComponent<Suricate>().GetAmountEaten()){
                                newAlphaFemale = candidat;
                            }
                    }
                }
                // If we found a female
                if(newAlphaFemale != null){
                    newAlphaFemale.GetComponent<Suricate>().IsAlpha(true);
                    newAlphaFemale.name += " Alpha";
                    newAlphaFemale.GetComponent<Renderer>().material = alphaFemaleMaterial;
                }
            }
        }

        // A sentinel died, we have to get a new one!
        if(suricate.GetSuricateType().Equals(Suricate.Type.Sentinel)){
            foreach(GameObject candidat in GameObject.FindGameObjectsWithTag("Suricate")){
                if(candidat.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Hunter) && !candidat.GetComponent<Suricate>().IsAlpha()){
                    // We only want to have 2 sentinels at the same time
                    if(GetNumberOfPostedSentinels() < 2)
                        candidat.GetComponent<Suricate>().OnSentinelDuty(suricate);
                    return;
                }
            }
        }
    }

    // IRL there are about 20 members per family, always an alpha male & female
    private void InitialSuricateSpawn(){
        // All the initial suricates start inside their home
        Vector3 position = GameObject.FindGameObjectWithTag("Hole").transform.position;
        // Since the hole is in the ground, we don't what for the suricates
        position.y = suricatePrefab.transform.position.y;
        GameObject suricate;
        // Instantiate them
        for(int i=0; i<initialColonySize; i++) {
            totalSuricates++;
            aliveSuricates++;
            suricate = Instantiate(suricatePrefab, position, Quaternion.identity);
            // The colony has always 2 sentinels watching over it
            if(i < 2){
                suricate.GetComponent<Suricate>().SetSuricateType(Suricate.Type.Sentinel);
                // Actual name will be set in the Start method of Suricate.cs
                DetermineSuricateGender(suricate.GetComponent<Suricate>());
            }
            // The rest are hunters
            else{
                // Create the two alphas
                if(i < 4){
                    suricate.GetComponent<Suricate>().IsAlpha(true);
                    // One male
                    if (i == 2) {
                        suricate.GetComponent<Suricate>().SetGender(Suricate.Gender.Male);
                        suricate.GetComponent<Renderer>().material = alphaMaleMaterial;
                    }
                    // One female
                    else if (i == 3) {
                        suricate.GetComponent<Suricate>().SetGender(Suricate.Gender.Female);
                        suricate.GetComponent<Renderer>().material = alphaFemaleMaterial;
                    }
                }
                else {
                    DetermineSuricateGender(suricate.GetComponent<Suricate>());
                }
                suricate.GetComponent<Suricate>().SetSuricateType(Suricate.Type.Hunter);
            }
            suricates.Add(suricate);
        }
    }

    // Between 2 and 4 suricates per brood
    private void SpawnBabies(Transform transform) {
        GameObject suricate;
        Vector3 position = transform.position;
        // The suricate shouldn't be flying!
        position.y /= 2;
        for(int i=0; i<Random.Range(2, 4); i++) {
            totalSuricates++;
            aliveSuricates++;
            // We want to spawn the baby a bit behind it's mother
            Vector3 direction = Quaternion.AngleAxis(Random.Range(-70, 70), Vector3.up) * -transform.forward;
            position += direction * 1.5f;
            suricate = Instantiate(suricatePrefab, position, Quaternion.identity);
            // A baby is half as big as an adult
            suricate.transform.localScale /= 2;
            suricate.GetComponent<Suricate>().SetSuricateType(Suricate.Type.Baby);
            DetermineSuricateGender(suricate.GetComponent<Suricate>());
            suricate.name = "" + totalSuricates;
            suricates.Add(suricate);
        }        
    }

    private void DetermineSuricateGender(Suricate suricate) {
        // I guess it's 1/2 male/female
        if (Random.value < 0.5) {
            suricate.SetGender(Suricate.Gender.Male);
        }
        else {
            suricate.SetGender(Suricate.Gender.Female);
        }
    }

    // For the sake of simplicty we only spawn raptors on the left or on the right...
    private void SpawnRaptor(){
        Vector3 position = new Vector3(0, 10f, Random.Range(groundBoundaries.min.z+5f, groundBoundaries.max.z-5f));
        // On the left
        if(Random.value < 0.5f)
            position.x = groundBoundaries.min.x - 15f;
        // On the right
        else
            position.x = groundBoundaries.max.x + 15f;
        GameObject raptor = Instantiate(raptorPrefab, position, Quaternion.identity);
        raptor.name = "Raptor";
    }

    public Vector3 GetUnoccupiedSentinelPost(){
        Vector3 otherPost;
        foreach(GameObject suricate in GameObject.FindGameObjectsWithTag("Suricate")){
            if(suricate.GetComponent<Suricate>().GetSuricateType().Equals(Suricate.Type.Sentinel)){
                otherPost = suricate.GetComponent<Suricate>().GetPost();
                // We compare the posts and take the one that is unoccupied
                if(sentinelPosts.ElementAt(0) == otherPost)
                    return sentinelPosts.ElementAt(1);
                else
                    return sentinelPosts.ElementAt(0);
            }
        }
        return Vector3.zero;
    }

    public int GetNumberOfSafeSuricates(){
        int amount = 0;
        foreach(GameObject suricate in GameObject.FindGameObjectsWithTag("Suricate")){
            if(suricate.GetComponent<Suricate>().IsSafe()){
                amount++;
            }
        }
        return amount;
    }
}
