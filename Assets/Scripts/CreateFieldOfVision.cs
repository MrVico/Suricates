using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFieldOfVision : MonoBehaviour {

    public Material material;

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

	// Use this for initialization
	void Start () {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GetComponent<MeshRenderer>().material = material;
        // Set the alpha component of the material's color to 0 so the FoV is transparent
        Color materialColor = GetComponent<MeshRenderer>().material.GetColor("_Color");
        materialColor.a = 0f;
        GetComponent<MeshRenderer>().material.color = materialColor;

        vertices = new[] {
            new Vector3(0,0,0),
            new Vector3(-4,0,5),
            new Vector3(4,0,5)
        };

        mesh.vertices = vertices;

        triangles = new[] { 0, 1, 2 };

        mesh.triangles = triangles;

        GetComponent<MeshCollider>().sharedMesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision) {
        Debug.Log("Collided with " + collision.gameObject.name);
    }
}
