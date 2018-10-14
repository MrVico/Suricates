using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleBaseSM : StateMachineBehaviour {

    protected static GameObject prey;

    protected int diveHash = Animator.StringToHash("dive");
    protected int flyHash = Animator.StringToHash("fly");
    
    public void SetPrey(GameObject p) {
        EagleBaseSM.prey = p;
        Debug.Log("Setting prey " + prey.name);
    }

    public GameObject GetPrey() {
        return EagleBaseSM.prey;
    }
}
