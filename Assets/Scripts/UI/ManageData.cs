using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageData : MonoBehaviour {

	[SerializeField] GameObject graph;

	[SerializeField] Button startButton;
	[SerializeField] Button pauseButton;
	
	[SerializeField] Slider meerkatSlider;
	[SerializeField] Slider predatorSlider;
	[SerializeField] Slider foodSlider;

	[SerializeField] int nbMeerkat;
	[SerializeField] int predatorRespawnTime;
	[SerializeField] int nbFood;
	[SerializeField] int nbPredator;

	// Graph
	public GameObject graphWindow;
	public RectTransform graphConainer;
	public RectTransform labelTemplateX;
	public RectTransform labelTemplateY;

	[SerializeField] Sprite dotSprite;

	// Data
	private static List<int> dataMeerkatList;
	private static List<int> dataFoodList;
	private static List<int> dataPredList;

	// Add or Remove meerkat
	public GameObject suricatePrefab;
	public GameObject raptorPrefab;
	public GameObject preyPrefab;

	private bool simulationStarted = false;

	// Use this for initialization
	void Start () {
		dataMeerkatList = new List<int>();
		dataFoodList = new List<int>();
		dataPredList = new List<int>();

		meerkatSlider.onValueChanged.AddListener(delegate {MeerkatSliderValueChanged(); });
		predatorSlider.onValueChanged.AddListener(delegate {PredatorSliderValueChanged(); });
		foodSlider.onValueChanged.AddListener(delegate {FoodSliderValueChanged(); });

		setData();
	}
	
	// Update is called once per frame
	void Update () {

	}

	// Called from UI, to start the simulation
	public void StartSimulation(){
		simulationStarted = true;
		FindObjectOfType<Spawner>().SetUpSimulation(nbMeerkat, predatorRespawnTime, nbFood);
		startButton.gameObject.SetActive(false);
		pauseButton.gameObject.SetActive(true);
		graph.SetActive(true);
		StartCoroutine(UpdateDataEverySecond());
	}

	private IEnumerator UpdateDataEverySecond(){
		while(simulationStarted){
			updateEverySecond();
			FindObjectOfType<GraphManager>().PopulateGraph(dataMeerkatList);
			yield return new WaitForSeconds(1);
		}
	}

	private void updateEverySecond() {
		nbMeerkat = GameObject.FindGameObjectsWithTag("Suricate").Length;
		nbFood = GameObject.FindGameObjectsWithTag("Prey").Length;
		nbPredator = GameObject.FindGameObjectsWithTag("Predator").Length;
		setData();

		dataMeerkatList.Add(nbMeerkat);
		dataFoodList.Add(nbFood);
		dataPredList.Add(nbPredator);

		if (graphWindow.activeSelf == true) {
			printGraph(dataMeerkatList, 0);
			printGraph(dataFoodList, 1);
			printGraph(dataPredList, 2);

			/*if(dataMeerkatList.Count > 15) {
				clearGraph();
				printGraph(dataMeerkatList);
			}				
			else {
				//clearGraph();
				printGraph(dataMeerkatList);
			}*/
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

	// TODO: Make a seperate stat panel for the live stats
	private void setData() {
		meerkatSlider.value = nbMeerkat;
		meerkatSlider.GetComponentInChildren<Text>().text = nbMeerkat.ToString();
		predatorSlider.value = predatorRespawnTime;
		predatorSlider.GetComponentInChildren<Text>().text = predatorRespawnTime.ToString()+"s";
		foodSlider.value = nbFood;
		foodSlider.GetComponentInChildren<Text>().text = nbFood.ToString();
	}

	private GameObject printDot(Vector2 position)
	{
		GameObject go_dot = new GameObject("dot", typeof(Image));

		go_dot.transform.SetParent(graphConainer, false);
		go_dot.GetComponent<Image>().sprite = dotSprite;

		RectTransform rectTransform = go_dot.GetComponent<RectTransform>();

		rectTransform.anchoredPosition = position;
		rectTransform.sizeDelta = new Vector2(0, 0);
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);

		return go_dot;
	}

	private void printGraph(List<int> valueList, int type)
	{
		float x_size = 30f;
		float y_max = 50f;
		float graph_height = graphConainer.sizeDelta.y;

		GameObject last_dot = null;

		float pos_x;
		float pos_y;
		for (int i = 0; i < valueList.Count; ++i)
		{
			pos_x = i * x_size;
			pos_y = (valueList[i] / y_max) * graph_height;

			GameObject dot = printDot(new Vector2(pos_x, pos_y));

			if (last_dot != null)
				createConnection(last_dot.GetComponent<RectTransform>().anchoredPosition, dot.GetComponent<RectTransform>().anchoredPosition, type);

			last_dot = dot;

			RectTransform label_x = Instantiate(labelTemplateX);
			label_x.SetParent(graphConainer);
			label_x.gameObject.SetActive(true);
			label_x.anchoredPosition = new Vector2(pos_x, 20f);
			label_x.GetComponent<Text>().color = Color.black;
			label_x.GetComponent<Text>().text = i.ToString();
		}

		int cpt = 10;
		for (int i = 0; i <= cpt; ++i)
		{
			RectTransform label_y = Instantiate(labelTemplateY);
			label_y.SetParent(graphConainer);
			label_y.gameObject.SetActive(true);

			float val_y = i * 1f / cpt;

			label_y.anchoredPosition = new Vector2(-7f, val_y * graph_height);
			label_y.GetComponent<Text>().color = Color.black;
			label_y.GetComponent<Text>().text = Mathf.RoundToInt(val_y * y_max).ToString();
		}
	}

	private void printGraphBis(List<int> valueList, int begin, int type) {
		float x_size = 50f;
		float y_max = 100f;
		float graph_height = graphConainer.sizeDelta.y;

		GameObject last_dot = null;

		float pos_x;
		float pos_y;
		for (int i = begin; i < valueList.Count; ++i)
		{
			pos_x = i * x_size;
			pos_y = (valueList[i] / y_max) * graph_height;

			GameObject dot = printDot(new Vector2(pos_x, pos_y));

			if (last_dot != null)
				createConnection(last_dot.GetComponent<RectTransform>().anchoredPosition, dot.GetComponent<RectTransform>().anchoredPosition, type);

			last_dot = dot;

			RectTransform label_x = Instantiate(labelTemplateX);
			label_x.SetParent(graphConainer);
			label_x.gameObject.SetActive(true);
			label_x.anchoredPosition = new Vector2(pos_x, 20f);
			label_x.GetComponent<Text>().color = Color.black;
			label_x.GetComponent<Text>().text = i.ToString();
		}

		int cpt = 10;
		for (int i = 0; i <= cpt; ++i)
		{
			RectTransform label_y = Instantiate(labelTemplateY);
			label_y.SetParent(graphConainer);
			label_y.gameObject.SetActive(true);

			float val_y = i * 1f / cpt;

			label_y.anchoredPosition = new Vector2(-7f, val_y * graph_height);
			label_y.GetComponent<Text>().color = Color.black;
			label_y.GetComponent<Text>().text = Mathf.RoundToInt(val_y * y_max).ToString();
		}
	}

	private void createConnection(Vector2 pos_dot_A, Vector2 pos_dot_B, int type)
	{
		GameObject connection = new GameObject("connection", typeof(Image));
		connection.transform.SetParent(graphConainer, false);

		// Suricate
		if(type == 0)
			connection.GetComponent<Image>().color = Color.yellow;
		// Bouffe
		if(type == 1)
			connection.GetComponent<Image>().color = Color.black;
		// Predateur
		if(type == 2)
			connection.GetComponent<Image>().color = Color.red;




		Vector2 direction = (pos_dot_B - pos_dot_A).normalized;

		float distance = Vector2.Distance(pos_dot_A, pos_dot_B);

		RectTransform rt = connection.GetComponent<RectTransform>();
		rt.anchorMin = new Vector2(0, 0);
		rt.anchorMax = new Vector2(0, 0);
		rt.sizeDelta = new Vector2(distance, 3f);
		rt.anchoredPosition = pos_dot_A + direction * distance * 0.5f;
		rt.localEulerAngles = new Vector3(0, 0, getAngleFromVector(direction));
	}

	private float getAngleFromVector(Vector2 direction)
	{
		direction = direction.normalized;

		float n = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		if (n < 0) n += 360;

		return n;
	}

	private void clearGraph() {
		int childs = graphConainer.childCount;
		if(childs > 18) {
			for (int i = 1; i < childs - 18; ++i) {
				GameObject.Destroy(graphConainer.GetChild(i).gameObject);
			}
		}
			
	}

	// Add 1 meerkat hunter to the hole
	public void addMeerkat() {
		Vector3 position = GameObject.FindGameObjectWithTag("Hole").transform.position;
		position.y = suricatePrefab.transform.position.y;

		GameObject suricate;
		suricate = Instantiate(suricatePrefab, position, Quaternion.identity);
		suricate.GetComponent<Suricate>().SetSuricateType(Suricate.Type.Hunter);

	}

	// Remove 1 meerkat hunter
	public void removeMeerkat() {
		GameObject[] suricates;
		suricates = GameObject.FindGameObjectsWithTag("Suricate");

		GameObject suricate = new GameObject();
		int last = suricates.Length - 1;
		bool loop = true;
		while(loop) {
			if(suricates[last].GetComponent<Suricate>().GetSuricateType() == Suricate.Type.Hunter) {
				suricate = suricates[last];
				loop = false;
			}

			last -= 1;
		}

		GameObject.Destroy(suricate);
	}

	public void addfood() {
		Vector3 position = new Vector3(Random.Range(-33f, 33f), preyPrefab.transform.localScale.y / 2, Random.Range(-23f, 23f));
		GameObject prey = Instantiate(preyPrefab, position, Quaternion.identity);
	}

	public void removefood() {
		GameObject[] preys;
		preys = GameObject.FindGameObjectsWithTag("Prey");

		int last = preys.Length - 1;
		GameObject prey = preys[last];

		GameObject.Destroy(prey);
	}

	// Spawn a predator
	public void addPredator() {
		Vector3 position = new Vector3(0, 10f, Random.Range(-20f, 20f));
		// On the left
		if (Random.value < 0.5f)
			position.x = -50f;
		// On the right
		else
			position.x = 50f;
		GameObject raptor = Instantiate(raptorPrefab, position, Quaternion.identity);
		raptor.name = "Raptor";
	}

	// Remove predator
	public void removePredator() {
		GameObject[] predators;
		predators = GameObject.FindGameObjectsWithTag("Predator");

		int last = predators.Length - 1;
		GameObject predator = predators[last];

		GameObject.Destroy(predator);
	}
}