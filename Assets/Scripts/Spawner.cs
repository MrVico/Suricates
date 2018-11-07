using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject suricatePrefab;
    public GameObject preyPrefab;
    public int nbOfPreys;
    public int initialColonySize;

    // For testing purposes only
    public bool spawnSuricates;

    private List<GameObject> suricates;

    private List<GameObject> preys;
    // Just for testing purposes
    private int totalPreys;

	// Use this for initialization
	void Start () {
        totalPreys = 0;
        preys = new List<GameObject>();
        suricates = new List<GameObject>();
        if(spawnSuricates)
            InitialSuricateSpawn();
        for(int i=0; i<nbOfPreys; i++) {
            SpawnPrey();
        }
    }
	
	// Update is called once per frame
	void Update () {
        // We always want to have a certain amount of preys alive
        if(preys.Count < nbOfPreys) {
            SpawnPrey();
        }
	}

    // TODO: Don't spawn near a Suricate!
    private void SpawnPrey() {
        Vector3 position = new Vector3(Random.Range(-20.0f, 20.0f), preyPrefab.transform.localScale.y/2, Random.Range(-20.0f, 20.0f));
        GameObject prey = Instantiate(preyPrefab, position, Quaternion.identity);
        preys.Add(prey);
        totalPreys++;
        prey.name = "Prey " + totalPreys;
    }

    private void PreyDied(GameObject prey) {
        preys.Remove(prey);
    }

    private void InitialSuricateSpawn(){
        // All the initial suricates start inside their home
        Vector3 position = GameObject.FindGameObjectWithTag("Hole").transform.position;
        // Since the hole is in the ground, we don't what for the suricates
        position.y = suricatePrefab.transform.position.y;
        GameObject suricate;
        // Instantiate them
        for(int i=0; i<initialColonySize; i++){
            suricate = Instantiate(suricatePrefab, position, Quaternion.identity);
            // The colony has always 2 sentinels watching over it
            if(i < 2){
                suricate.GetComponent<Suricate>().SetType(Suricate.Type.Sentinel);
                suricate.name = "Sentinel Suricate "+(i+1);
            }
            else{
                suricate.GetComponent<Suricate>().SetType(Suricate.Type.Hunter);
                suricate.name = "Hunter Suricate "+(i-1);
            }
            suricates.Add(suricate);
        }
    }
}
