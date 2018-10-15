using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldSuricate : MonoBehaviour {

    public enum Type { Hunter, Sentinel };
    
    public Type type;

    //private Type type;

    // Use this for initialization
    void Start () {
        if (type == Type.Hunter)
            GetComponent<Animator>().SetBool("hunter", true);
        else if (type == Type.Sentinel)
            GetComponent<Animator>().SetBool("sentinel", true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Type GetSuricateType() {
        return type;
    }
}
