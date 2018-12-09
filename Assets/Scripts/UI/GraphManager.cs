using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour {

	[SerializeField] Sprite dotSprite;
	[SerializeField] GameObject xAxisLabelGO;
	[SerializeField] GameObject yAxisLabelGO;
	[SerializeField] GameObject xAxisDashGO;
	[SerializeField] GameObject yAxisDashGO;

	private RectTransform mainContainer;
	private RectTransform dataContainer;
	private RectTransform xAxisLabel;
	private RectTransform yAxisLabel;
	private RectTransform xAxisDash;
	private RectTransform yAxisDash;

	private List<int> values;

	// Use this for initialization
	void Start () {
		mainContainer = GetComponent<RectTransform>();
		xAxisLabel = xAxisLabelGO.GetComponent<RectTransform>();
		yAxisLabel = yAxisLabelGO.GetComponent<RectTransform>();
		xAxisDash = xAxisDashGO.GetComponent<RectTransform>();
		yAxisDash = yAxisDashGO.GetComponent<RectTransform>();
		values = new List<int>();

		StartCoroutine(AddValues());
	}

	private IEnumerator AddValues(){
		for(int i=0; i<100; i++){
			Debug.Log((i+1)*10);
			values.Add((i+1)*10);
			PopulateGraph(values);
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	private GameObject CreatePoint(Vector2 anchoredPosition){
		GameObject point = new GameObject("Point", typeof(Image));
		point.transform.SetParent(dataContainer, false);
		point.GetComponent<Image>().sprite = dotSprite;
		RectTransform rectTransform = point.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = anchoredPosition;
		rectTransform.sizeDelta = new Vector2(2, 2);
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);
		return point;	
	}

	private void PopulateGraph(List<int> values){
		// Clear the graph before populating it
		foreach(Transform child in transform){
			// Delete the whole data hierarchy
			if(child.name.Equals("Data"))
				Destroy(child.gameObject);
		}
		// Recreate the data container
		dataContainer = new GameObject("Data", typeof(RectTransform)).GetComponent<RectTransform>();
		dataContainer.transform.SetParent(mainContainer, false);
		dataContainer.anchorMin = new Vector2(0, 0);
		dataContainer.anchorMax = new Vector2(1, 1);
		dataContainer.pivot = new Vector2(0, 0);

		float graphHeight = Mathf.Abs(mainContainer.sizeDelta.y);
		float graphWidth = Mathf.Abs(mainContainer.sizeDelta.x);
		
		// Top and bottom margins
		float xMargin = 5f;
		float yMargin = 5f;
		// Base max value
		float yMax = 100f;
		// Adjust max value accordingly
		if(values[values.Count-1] > yMax - yMargin){
			yMax = values[values.Count-1] + yMargin;
		}
		// Adjust width so it never goes offscreen
		float xOffset = (graphWidth - xMargin*2) / values.Count;
		float xPos = 0f;
		float yPos = 0f;
		GameObject lastPoint = null;
		for(int i=0; i<values.Count; i++){
			xPos = i * xOffset + xMargin;
			yPos = (values[i] / yMax) * (graphHeight - yMargin*2) + yMargin;
			if(i == values.Count - 1)
				Debug.Log("y: "+yPos+" for i: "+i);
			GameObject currentPoint = CreatePoint(new Vector2(xPos, yPos));
			if(lastPoint != null){
				LinkDots(lastPoint.GetComponent<RectTransform>().anchoredPosition,
							currentPoint.GetComponent<RectTransform>().anchoredPosition);
			}
			lastPoint = currentPoint;
		}

		// Axis X
		float firstPosX = xMargin;
		float lastPosX = xPos;
		int horizontalSeparatorAmount = 12;
		for(int i=0; i<=horizontalSeparatorAmount; i++){
			float currentPosX = firstPosX + i*(lastPosX - firstPosX)/horizontalSeparatorAmount;
			// Dashes
			RectTransform dashX = Instantiate(xAxisDash);
			dashX.SetParent(dataContainer, false);
			dashX.gameObject.SetActive(true);
			dashX.anchoredPosition = new Vector2(currentPosX, 5f);

			// Labels
			RectTransform labelX = Instantiate(xAxisLabel);
			labelX.SetParent(dataContainer, false);
			labelX.gameObject.SetActive(true);
			labelX.anchoredPosition = new Vector2(currentPosX, 0f);
			labelX.GetComponent<Text>().text = Mathf.RoundToInt((values.Count) * i / (horizontalSeparatorAmount)).ToString();
		}

		// Axis Y
		float firstPosY = yMargin;
		float lastPosY = yPos;
		int verticalSeparatorAmount = 10;
		for(int i=0; i<=verticalSeparatorAmount; i++){
			float currentPosY = firstPosY + i*(lastPosY - firstPosY)/verticalSeparatorAmount;
			// Dashes
			RectTransform dashY = Instantiate(yAxisDash);
			dashY.SetParent(dataContainer, false);
			dashY.gameObject.SetActive(true);
			dashY.anchoredPosition = new Vector2(10f, currentPosY);

			// Labels
			RectTransform labelY = Instantiate(yAxisLabel);
			labelY.SetParent(dataContainer, false);
			labelY.gameObject.SetActive(true);
			labelY.anchoredPosition = new Vector2(0f, currentPosY);
			labelY.GetComponent<Text>().text = Mathf.RoundToInt((yMax - yMargin) * i / (verticalSeparatorAmount)).ToString();
		}
	}

	private void LinkDots(Vector2 dotPosA, Vector2 dotPosB){
		GameObject line = new GameObject("Line", typeof(Image));
		line.transform.SetParent(dataContainer, false);
		line.GetComponent<Image>().color = new Color(0f, 0f, 0f);
		Vector2 direction = (dotPosB - dotPosA).normalized;
		float distance = Vector2.Distance(dotPosA, dotPosB);
		RectTransform rectTransform = line.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(distance, 2);
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);
		rectTransform.anchoredPosition = dotPosA + direction * distance / 2;
		rectTransform.localEulerAngles = new Vector3(0, 0, getAngleFromVector(direction));
	}

	private float getAngleFromVector(Vector2 direction)
	{
		direction = direction.normalized;
		float n = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		if (n < 0) 
			n += 360;
		return n;
	}
}
