﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject suricatePrefab;
    public GameObject raptorPrefab;
    public GameObject preyPrefab;
    public int nbOfPreys;
    public int initialColonySize;
    public float raptorSpawnTime;

    // For testing purposes only
    public bool spawnSuricates;

    private List<GameObject> suricates;

    private List<GameObject> preys;
    // Just for testing purposes
    private int totalPreys;

    private float timer;

	// Use this for initialization
	void Start () {
        totalPreys = 0;
        timer = 0;
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

    // TODO: Don't spawn near a Suricate!
    private void SpawnPrey() {
        Vector3 position = new Vector3(Random.Range(-20f, 20f), preyPrefab.transform.localScale.y/2, Random.Range(-20f, 20f));
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
