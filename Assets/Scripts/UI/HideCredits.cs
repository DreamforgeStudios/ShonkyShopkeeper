using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCredits : MonoBehaviour
{

	public GameObject titleScreen, titleCanvas;

	public void HideCreditCanvas()
	{
		titleScreen.SetActive(true);
		titleCanvas.SetActive(true);
		gameObject.SetActive(false);
	}
}
