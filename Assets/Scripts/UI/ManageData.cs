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

	// Graph
	public GameObject graphWindow;
	public RectTransform graphConainer;
	public RectTransform labelTemplateX;
	public RectTransform labelTemplateY;

	[SerializeField] private Sprite dotSprite;


	// Data
	private static List<int> dataMeerkatList;
	private static List<int> dataFoodList;
	private static List<int> dataPredList;



	// Use this for initialization
	void Start () {
		nbMeerkat = 0;
		nbFood = 0;
		nbPredator = 0;

		dataMeerkatList = new List<int> ();
		dataFoodList = new List<int>();
		dataPredList = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time >= nextTime) {
			nextTime += interval;
			updateEverySecond();
		}

		//clearGraph();
	}

	private void updateEverySecond() {
		nbMeerkat = GameObject.FindGameObjectsWithTag("Suricate").Length;
		nbFood = GameObject.FindGameObjectsWithTag("Prey").Length;
		nbPredator = GameObject.FindGameObjectsWithTag("Predator").Length;
		setDataText();

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

	private void setDataText() {
		nbMeerkatText.text = "meerkats: " + nbMeerkat.ToString();
		nbFoodText.text = "foods: " + nbFood.ToString();
		nbPredatorText.text = "predators: " + nbPredator.ToString();
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
		float x_size = 10f;
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
}