using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour {

	public Text text;
	private bool isPaused = false;

	// Use this for initialization
	void Start () {
		text.text = "Pause";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void pauseButton() {
		if (!isPaused) {
			text.text = "Play";
			isPaused = true;
		}
		else {
			text.text = "Pause";
			isPaused = false;
		}
	}
}
