using UnityEngine;
using UnityEngine.UI;

public class FasterOnClick : MonoBehaviour {

	public Text text;
	private bool isFast = false;

	// Use this for initialization
	void Start()
	{
		text.text = "Fast";
	}

	public void fast()
	{
		if (!isFast)
		{
			text.text = "Normal";
			Time.timeScale = 4f;
			isFast = true;
		}
		else
		{
			text.text = "Fast";
			Time.timeScale = 1f;
			isFast = false;
		}
	}
}