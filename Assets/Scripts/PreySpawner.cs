using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreySpawner : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
        for(int i=0; i<5; i++) {
            Vector3 position = new Vector3(Random.Range(-10.0f, 10.0f), prefab.transform.lossyScale.y/2, Random.Range(5.0f, 20.0f));
            Instantiate(prefab, position, Quaternion.identity);
            // Fix for a glitch (?), otherwise, since the object isn't moving, it doesn't collide
            // ISN'T FIXING SHIT!
            //prefab.GetComponent<Rigidbody>().MovePosition(position);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
