using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCredits : MonoBehaviour
{

	public GameObject titleScreen, titleCanvas, optionsScreenPrefab;

	public void HideCreditCanvas()
	{
		titleScreen.SetActive(true);
		titleCanvas.SetActive(true);
		gameObject.SetActive(false);
	}

	public void BackToOptionsIntro()
	{
		optionsScreenPrefab.GetComponent<OptionsScreen>().optionsCanvas.gameObject.SetActive(true);
		gameObject.SetActive(false);
	}
}
