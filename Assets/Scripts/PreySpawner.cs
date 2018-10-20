using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreySpawner : MonoBehaviour {

    public GameObject prefab;
    public int nbOfPreys;

    private List<GameObject> preys;
    private int totalPreys;

	// Use this for initialization
	void Start () {
        totalPreys = 0;
        preys = new List<GameObject>();
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
        Vector3 position = new Vector3(Random.Range(-20.0f, 20.0f), prefab.transform.localScale.y/2, Random.Range(-20.0f, 20.0f));
        GameObject prey = Instantiate(prefab, position, Quaternion.identity);
        preys.Add(prey);
        totalPreys++;
        prey.name = "Prey " + totalPreys;
    }

    private void PreyDied(GameObject prey) {
        preys.Remove(prey);
    }
}
