using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpositionBubbleSizing : MonoBehaviour
{

	public Image scroll, nextButton, backButton;
	public TextMeshProUGUI text;
	
	private RectTransform scrollRect;
	private float desiredHeight;
	
	// Use this for initialization
	void Start () {
		scrollRect = scroll.gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	/*
	void Update () {
		desiredHeight = text.preferredHeight;
		if (desiredHeight + (desiredHeight/5) != scrollRect.sizeDelta.y)
			UpdateSize();
	}
	*/

	public void UpdateSize()
	{
		scrollRect = scroll.gameObject.GetComponent<RectTransform>();
		
		desiredHeight = text.preferredHeight;
		
		//Get desired Height and add padding ~ 20%
		scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x,(desiredHeight + (desiredHeight/5)));
		
		//Modify offset for buttons to match size of scroll
		/*
		RectTransform nextButtonRT = nextButton.gameObject.GetComponent<RectTransform>();
		float offsetNext = (nextButtonRT.sizeDelta.y / 8) * 2;
		nextButtonRT.localPosition = new Vector3(nextButtonRT.localPosition.x, nextButtonRT.localPosition.y - offsetNext, nextButtonRT.localPosition.z);
		
		
		RectTransform backButtonRT = backButton.gameObject.GetComponent<RectTransform>();
		float offsetBack = (backButtonRT.sizeDelta.y / 8) * 2;
		Debug.Log("Current exit button offset is " + backButtonRT.localPosition.y + " new offset is " + (backButtonRT.localPosition.y - offsetBack));
		backButtonRT.localPosition = new Vector3(backButtonRT.localPosition.x, backButtonRT.localPosition.y - offsetBack, backButtonRT.localPosition.z);
		*/
	}
}
