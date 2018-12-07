using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseOnClick : MonoBehaviour {

	public Text text;
	private bool isPaused = false;

	// Use this for initialization
	void Start () {
		text.text = "Pause";
	}

	public void pause() {
		if (!isPaused) {
			text.text = "Play";
			Time.timeScale = 0f;
			isPaused = true;
		}
		else {
			text.text = "Pause";
			Time.timeScale = 1f;
			isPaused = false;
		}
	}
}
