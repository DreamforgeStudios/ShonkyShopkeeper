using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstructionBubbleSizing : MonoBehaviour
{

	public Image scroll;
	public TextMeshProUGUI text;
	private RectTransform scrollRect;

	private float desiredHeight;

	
	private void Start()
	{
		scrollRect = scroll.gameObject.GetComponent<RectTransform>();
	}

	/*
	private void Update()
	{
		//desiredHeight = text.preferredHeight;
		//if (desiredHeight + (desiredHeight/5) != scrollRect.sizeDelta.y)
			UpdateSize();
	}
	*/

	public void UpdateSize()
	{
		scrollRect = scroll.gameObject.GetComponent<RectTransform>();
		
		desiredHeight = text.preferredHeight;
		
		//Get desired Height and add padding ~ 20%
		scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x,(desiredHeight + (desiredHeight/5)));
	}
}
