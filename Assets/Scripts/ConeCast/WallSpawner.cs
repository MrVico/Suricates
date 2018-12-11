using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For testing only!
public class WallSpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.tag = "Wall";
        // TOP & BOT walls
        for(int x=-35; x<35; x++) {
            for(int y=0; y<25; y++) {
                // TOP
                Instantiate(cube, new Vector3(x, y, 25), Quaternion.identity);
                // BOT
                Instantiate(cube, new Vector3(x, y, -25), Quaternion.identity);
            }
        }
        // LEFT & RIGHT walls
        for (int z = -25; z < 25; z++) {
            for (int y = 0; y < 25; y++) {
                // LEFT
                Instantiate(cube, new Vector3(-35, y, z), Quaternion.identity);
                // RIGHT
                Instantiate(cube, new Vector3(35, y, z), Quaternion.identity);
            }
        }
        Destroy(cube);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
