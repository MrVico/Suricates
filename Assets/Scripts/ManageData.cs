using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageData : MonoBehaviour {

	public Text nbMeerkatText;
	public Text nbFoodText;

	private int nbMeerkat;
	private int nbFood;

	// Use this for initialization
	void Start () {
		nbMeerkat = 0;
	}
	
	// Update is called once per frame
	void Update () {
		nbMeerkat = GameObject.FindGameObjectsWithTag("Suricate").Length;
		nbFood = GameObject.FindGameObjectsWithTag("Prey").Length;
		setDataText();
	}

	private void setDataText() {
		nbMeerkatText.text = "Nb of meerkats: " + nbMeerkat.ToString();
		nbFoodText.text = "Nb of foods: " + nbFood.ToString();
	}
}