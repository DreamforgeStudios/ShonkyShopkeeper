using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnableOptionsScreen : MonoBehaviour
{

	public GameObject optionsScreenInScene, mainShopCanvas, tutorialShopCanvas, tutorialProgressChecker;

	public void EnableOptions()
	{
		if (SceneManager.GetActiveScene().name == "Shop")
		{
			mainShopCanvas.SetActive(false);
			GameManager.Instance.introduceTrueGolem = true;
			GameManager.Instance.canUseTools = false;
			optionsScreenInScene.SetActive(true);
		} else if (SceneManager.GetActiveScene().name == "TutorialShop")
		{
			GameManager.Instance.introduceTrueGolem = true;
			GameManager.Instance.canUseTools = false;
			tutorialShopCanvas.SetActive(false);
			tutorialProgressChecker.SetActive(false);
			optionsScreenInScene.SetActive(true);
		}
	}
}
