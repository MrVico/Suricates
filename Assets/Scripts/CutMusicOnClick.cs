using UnityEngine;
using UnityEngine.UI;

public class CutMusicOnClick : MonoBehaviour {

	public Text text;
	private bool isCut = false;

	// Use this for initialization
	void Start()
	{
		text.text = "Music";
	}

	public void cutMusic()
	{
		if (!isCut)
		{
			text.text = "cisuM";
			isCut = true;
		}
		else
		{
			text.text = "Music";
			isCut = false;
		}
	}
}