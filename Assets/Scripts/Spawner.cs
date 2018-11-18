using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public static int totalSuricates;
    public static int aliveSuricates;

    public GameObject suricatePrefab;
    public GameObject raptorPrefab;
    public GameObject preyPrefab;
    public Material alphaMaleMaterial;
    public Material alphaFemaleMaterial;
    public int nbOfPreys;
    public int initialColonySize;
    public float raptorSpawnTime;

    // For testing purposes only
    public bool spawnSuricates;

    private List<GameObject> suricates;

    private GameObject preyContainer;
    private List<GameObject> preys;
    // Just for testing purposes
    private int totalPreys;

    private float timer;

	// Use this for initialization
	void Start () {
        totalPreys = 0;
        totalSuricates = 0;
        aliveSuricates = 0;
        timer = 0;
        preys = new List<GameObject>();
        suricates = new List<GameObject>();
        preyContainer = GameObject.FindGameObjectWithTag("PreyContainer");
        if (spawnSuricates)
            InitialSuricateSpawn();
        for(int i=0; i<nbOfPreys; i++) {
            SpawnPrey();
        }
        // TEST
        //SpawnRaptor();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        // We always want to have a certain amount of preys alive, or do we?
        if(preys.Count < nbOfPreys) {
            SpawnPrey();
        }
        /*
        if(timer >= raptorSpawnTime){
            SpawnRaptor();
            timer = 0;
        }
        */
	}
    
    private void SpawnPrey() {
        Vector3 position = new Vector3(Random.Range(-20f, 20f), preyPrefab.transform.localScale.y/2, Random.Range(-20f, 20f));
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
        if (suricate.IsAlpha()){
            foreach(GameObject candidat in GameObject.FindGameObjectsWithTag("Suricate")){
                // The first alive suricate of the same gender becomes the new alpha
                if (candidat.GetComponent<Suricate>().GetGender().Equals(suricate.GetGender())
                    && !candidat.GetComponent<Suricate>().IsDead()){
                    candidat.GetComponent<Suricate>().IsAlpha(true);
                    candidat.name += " Alpha";
                    if(suricate.GetGender().Equals(Suricate.Gender.Male))
                        candidat.GetComponent<Renderer>().material = alphaMaleMaterial;
                    else if (suricate.GetGender().Equals(Suricate.Gender.Female))
                        candidat.GetComponent<Renderer>().material = alphaFemaleMaterial;
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
