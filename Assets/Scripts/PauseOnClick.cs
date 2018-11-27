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
			isPaused = true;
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPaused = true;
#else
			Application.Pause();
#endif
		}
		else {
			text.text = "Pause";
			isPaused = false;
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPaused = false;
#else
			Application.Play();
#endif
		}
	}
}
