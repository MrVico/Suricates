using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public static int totalSuricates;
    public static int aliveSuricates;

    [SerializeField] GameObject suricatePrefab;
    [SerializeField] GameObject raptorPrefab;
    [SerializeField] GameObject preyPrefab;
    [SerializeField] Material alphaMaleMaterial;
    [SerializeField] Material alphaFemaleMaterial;
    [SerializeField] float raptorSpawnTime;

    private int nbOfPreys;
    private int initialColonySize;

    // For testing purposes only
    public bool spawnSuricates;
    // Just for testing purposes
    private int totalPreys;

    private List<GameObject> suricates;

    private GameObject preyContainer;
    private List<GameObject> preys;

    private float timer;

    private bool simulationStarted = false;

	// Update is called once per frame
	void Update () {
        if(simulationStarted){
            timer += Time.deltaTime;
            // We always want to have a certain amount of preys alive, or do we?
            if(preys.Count < nbOfPreys) {
                SpawnPrey();
            }
            if(timer >= raptorSpawnTime){
                SpawnRaptor();
                timer = 0;
            }
        }
	}

    public void SetUpSimulation(int nbOfSuricates, int nbOfPredators, int nbOfPreys){
        initialColonySize = nbOfSuricates;
        this.nbOfPreys = nbOfPreys;
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
            SpawnPrey();
        }
    }
    
    private void SpawnPrey() {
        Vector3 position = new Vector3(Random.Range(-33f, 33f), preyPrefab.transform.localScale.y/2, Random.Range(-23f, 23f));
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
        // Should the mother always be in their home when giving birth???
        // We should add a general day time so we know how many days passed etc...
        GameObject suricate;
        Vector3 position = transform.position;
        //Debug.Log("Forward: " + transform.forward+" Backward: "+ (-transform.forward));
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
        Vector3 position = new Vector3(0, 10f, Random.Range(-20f, 20f));
        // On the left
        if(Random.value < 0.5f)
            position.x = -50f;
        // On the right
        else
            position.x = 50f;
        GameObject raptor = Instantiate(raptorPrefab, position, Quaternion.identity);
        raptor.name = "Raptor";
    }
}
