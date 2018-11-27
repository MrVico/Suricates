using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageData : MonoBehaviour {

	public Text nbMeerkatText;
	public Text nbFoodText;
	public Text nbPredatorText;

	private int nbMeerkat;
	private int nbFood;
	private int nbPredator;

	private int interval = 1;
	private float nextTime = 0;

	// Use this for initialization
	void Start () {
		nbMeerkat = 0;
		nbFood = 0;
		nbPredator = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time >= nextTime) {
			nextTime += interval;
			updateEverySecond();
		}
	}

	private void updateEverySecond() {
		nbMeerkat = GameObject.FindGameObjectsWithTag("Suricate").Length;
		nbFood = GameObject.FindGameObjectsWithTag("Prey").Length;
		nbPredator = GameObject.FindGameObjectsWithTag("Predator").Length;
		setDataText();
	}

	private void setDataText() {
		nbMeerkatText.text = "meerkats: " + nbMeerkat.ToString();
		nbFoodText.text = "foods: " + nbFood.ToString();
		nbPredatorText.text = "predators: " + nbPredator.ToString();
	}
}