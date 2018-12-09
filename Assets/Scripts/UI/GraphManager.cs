using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour {

	[SerializeField] Sprite dotSprite;
	[SerializeField] GameObject xAxisLabelGO;
	[SerializeField] GameObject yAxisLabelGO;

	private RectTransform container;
	private RectTransform xAxisLabel;
	private RectTransform yAxisLabel;

	private List<int> values;

	// Use this for initialization
	void Start () {
		container = GetComponent<RectTransform>();
		xAxisLabel = xAxisLabelGO.GetComponent<RectTransform>();
		yAxisLabel = yAxisLabelGO.GetComponent<RectTransform>();
		values = new List<int>();

		StartCoroutine(AddValues());
	}

	private IEnumerator AddValues(){
		for(int i=0; i<40; i++){
			Debug.Log(i*5);
			values.Add(i*5);
			PopulateGraph(values);
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	private GameObject CreatePoint(Vector2 anchoredPosition){
		GameObject point = new GameObject("Point", typeof(Image));
		point.transform.SetParent(container, false);
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
			if(!child.name.Equals("Background") 
				&& !child.name.Equals(xAxisLabelGO.name)
				&& !child.name.Equals(yAxisLabelGO.name)){
					Destroy(child.gameObject);
				}
		}
		float graphHeight = Mathf.Abs(container.sizeDelta.y);
		float graphWidth = Mathf.Abs(container.sizeDelta.x);
		
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
		GameObject lastPoint = null;
		for(int i=0; i<values.Count; i++){
			float xPos = i * xOffset + xMargin;
			float yPos = (values[i] / yMax) * (graphHeight - yMargin*2) + yMargin;
			if(i == values.Count - 1)
				Debug.Log("y: "+yPos);
			GameObject currentPoint = CreatePoint(new Vector2(xPos, yPos));
			if(lastPoint != null){
				LinkDots(lastPoint.GetComponent<RectTransform>().anchoredPosition,
							currentPoint.GetComponent<RectTransform>().anchoredPosition);
			}
			lastPoint = currentPoint;
			/*
			RectTransform labelX = Instantiate(xAxisLabel);
			labelX.SetParent(container, false);
			labelX.gameObject.SetActive(true);
			labelX.anchoredPosition = new Vector2(xPos, 0f);
			labelX.GetComponent<Text>().text = i.ToString();
			*/
		}

		int horizontalSeparatorAmount = 12;
		for(int i=0; i<horizontalSeparatorAmount; i++){
			RectTransform labelX = Instantiate(xAxisLabel);
			labelX.SetParent(container, false);
			labelX.gameObject.SetActive(true);
			labelX.anchoredPosition = new Vector2(i * 1f / horizontalSeparatorAmount * graphWidth + xMargin, 0f);
			labelX.GetComponent<Text>().text = Mathf.RoundToInt((values.Count) * i / (horizontalSeparatorAmount-1)).ToString();
		}

		int verticalSeparatorAmount = 10;
		for(int i=0; i<verticalSeparatorAmount; i++){
			RectTransform labelY = Instantiate(yAxisLabel);
			labelY.SetParent(container, false);
			labelY.gameObject.SetActive(true);
			labelY.anchoredPosition = new Vector2(0f, i * 1f / verticalSeparatorAmount * graphHeight + yMargin);
			labelY.GetComponent<Text>().text = Mathf.RoundToInt((yMax - yMargin) * i / (verticalSeparatorAmount-1)).ToString();
		}
	}

	private void LinkDots(Vector2 dotPosA, Vector2 dotPosB){
		GameObject line = new GameObject("Line", typeof(Image));
		line.transform.SetParent(container, false);
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
