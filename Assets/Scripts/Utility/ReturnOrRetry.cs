﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnOrRetry {
	public static void Return(string itemString, Quality.QualityGrade grade) {
		//SFX.Play("sound");
        Inventory.Instance.InsertItem(new ItemInstance(itemString, 1, grade, true));
		//SceneManager.LoadScene("Shop");
		if (!GameManager.Instance.InTutorial)
			Initiate.Fade("Shop", Color.black, 2f);
		else
		{
			TutorialProgressChecker.Instance.FinishedComponent(itemString);
			Initiate.Fade("TutorialShop", Color.black, 2f);
		}
	}

	public static void Retry() {
        if (GameManager.Instance.RetriesRemaining > 0) {
	        //SFX.Play("sound");
            GameManager.Instance.RetriesRemaining--;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	        Initiate.Fade(SceneManager.GetActiveScene().name, Color.black, 2f);
        }
	}
}
