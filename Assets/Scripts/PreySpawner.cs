using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreySpawner : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
        /*
        Instantiate(prefab, new Vector3(8,0,0), Quaternion.identity);
        Instantiate(prefab, new Vector3(-8, 0, 0), Quaternion.identity);
        Instantiate(prefab, new Vector3(0, 0, -8), Quaternion.identity);
        Instantiate(prefab, new Vector3(0, 0, 8), Quaternion.identity);
        */
        
        for(int i=0; i<5; i++) {
            Vector3 position = new Vector3(Random.Range(-10.0f, 10.0f), 0.1f/*prefab.transform.lossyScale.y/2*/, Random.Range(5.0f, 20.0f));
            Instantiate(prefab, position, Quaternion.identity);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
