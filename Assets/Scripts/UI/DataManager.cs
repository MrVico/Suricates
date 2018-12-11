using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour {

	// Graph
	[SerializeField] GameObject graph;
	[SerializeField] Toggle showGraphToggle;

	// Game buttons
	[SerializeField] Button startButton;
	[SerializeField] Button pauseButton;
	
	// UI parameter changers
	[SerializeField] Slider meerkatSlider;
	[SerializeField] Slider predatorSlider;
	[SerializeField] Slider foodSlider;
	[SerializeField] Toggle kidsForEveryoneToggle;

	// Game parameter values
	[SerializeField] int nbMeerkat;
	[SerializeField] int predatorRespawnTime;
	[SerializeField] int nbFood;
	[SerializeField] bool kidsForEveryone;

	// Data
	private static List<int> dataMeerkatList;

	// Simulation information
	private bool simulationStarted = false;
	private Coroutine graphRoutine;

	// Use this for initialization
	void Start () {
		dataMeerkatList = new List<int>();

		meerkatSlider.onValueChanged.AddListener(delegate {MeerkatSliderValueChanged(); });
		predatorSlider.onValueChanged.AddListener(delegate {PredatorSliderValueChanged(); });
		foodSlider.onValueChanged.AddListener(delegate {FoodSliderValueChanged(); });

		// Default value
		kidsForEveryone = false;
		kidsForEveryoneToggle.onValueChanged.AddListener((value) =>{
			kidsForEveryone = value;
		});

		showGraphToggle.interactable = false;
		showGraphToggle.onValueChanged.AddListener((value) =>{
			// Show graph
			if(value){
				graph.SetActive(true);
				graphRoutine = StartCoroutine(UpdateDataEverySecond());
			}
			// Hide graph
			else{
				StopCoroutine(graphRoutine);
				graph.SetActive(false);
			}
		});

		// Load default values into the parameter handlers
		SetData();
	}
	
	// Called from UI, to start the simulation
	public void StartSimulation(){
		simulationStarted = true;
		FindObjectOfType<Spawner>().SetUpSimulation(nbMeerkat, predatorRespawnTime, nbFood, kidsForEveryone);
		startButton.gameObject.SetActive(false);
		pauseButton.gameObject.SetActive(true);
		meerkatSlider.interactable = false;
		predatorSlider.interactable = false;
		foodSlider.interactable = false;
		kidsForEveryoneToggle.interactable = false;
		
		showGraphToggle.interactable = true;
		graph.SetActive(true);
		graphRoutine = StartCoroutine(UpdateDataEverySecond());		
	}

	private IEnumerator UpdateDataEverySecond(){
		while(simulationStarted){
			dataMeerkatList.Add(GameObject.FindGameObjectsWithTag("Suricate").Length);
			FindObjectOfType<GraphManager>().PopulateGraph(nbMeerkat, dataMeerkatList);
			yield return new WaitForSeconds(1);
		}
	}

	private void MeerkatSliderValueChanged(){
		if(!simulationStarted){
			meerkatSlider.GetComponentInChildren<Text>().text = meerkatSlider.value.ToString();
			nbMeerkat = (int) meerkatSlider.value;
		}
	}

	private void PredatorSliderValueChanged(){
		if(!simulationStarted){
			predatorSlider.GetComponentInChildren<Text>().text = predatorSlider.value.ToString()+"s";
			predatorRespawnTime = (int) predatorSlider.value;
		}
	}
	
	private void FoodSliderValueChanged(){
		if(!simulationStarted){
			foodSlider.GetComponentInChildren<Text>().text = foodSlider.value.ToString();
			nbFood = (int) foodSlider.value;
			FindObjectOfType<Spawner>().SpawnPreys(nbFood);
		}
	}

	private void SetData() {
		meerkatSlider.value = nbMeerkat;
		meerkatSlider.GetComponentInChildren<Text>().text = nbMeerkat.ToString();
		predatorSlider.value = predatorRespawnTime;
		predatorSlider.GetComponentInChildren<Text>().text = predatorRespawnTime.ToString()+"s";
		foodSlider.value = nbFood;
		foodSlider.GetComponentInChildren<Text>().text = nbFood.ToString();
	}
}