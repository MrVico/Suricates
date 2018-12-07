using UnityEngine;
using UnityEngine.UI;


public class ShowGraphOnClick : MonoBehaviour {

	public Text text;
	private bool isShow = false;
		
	public GameObject graphWindow;

	// Use this for initialization
	void Start() {
		text.text = "Show Graph";
	}

	public void showGraph() {
		if (!isShow) {
			text.text = "Hide Graph";
			graphWindow.SetActive(true);
			isShow = true;
		}
		else {
			text.text = "Show Graph";
			graphWindow.SetActive(false);
			isShow = false;
		}
	}
}