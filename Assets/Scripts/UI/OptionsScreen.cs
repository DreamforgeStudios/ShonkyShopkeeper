using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{
	public TextMeshProUGUI soundButtonText, difficultyButtonText;

	public Inventory defaultInventory, TutorialInventory;
	public ShonkyInventory defaultShonkyInventory, tutorialShonkyInventory;

	//Intro specific variables
	public IntroScene introHandler;
	
	//main Shop specific variables
	public Canvas mainShopCanvas;
	
	//Tutorial Shop specific variables
	public Canvas tutorialShopCanvas;
	public GameObject tutorialProgressChecker;
	
	
	// Use this for initialization
	void Start () {
		DetermineSoundButtonStatus();
		DetermineDifficulty();
	}

	public void DisableOptions()
	{
		if (SceneManager.GetActiveScene().name == "Intro")
		{
			introHandler.EnableStartScreen();
		} else if (SceneManager.GetActiveScene().name == "Shop")
		{
			//Variable used to stop any interactions during golem creation process so I can reuse it here
			ResetVariables();
			mainShopCanvas.gameObject.SetActive(true);
		} else if (SceneManager.GetActiveScene().name == "TutorialShop")
		{
			//Variable used to stop any interactions during golem creation process so I can reuse it here
			ResetVariables();
			if (tutorialProgressChecker != null)
				tutorialProgressChecker.SetActive(true);
			
			tutorialShopCanvas.gameObject.SetActive(true);
		}
		this.gameObject.SetActive(false);
	}

	private void DetermineSoundButtonStatus()
	{
		if (GameManager.Instance.PlayingAudio)
		{
			soundButtonText.text = "Sound: On";
		}
		else
		{
			soundButtonText.text = "Sound: Off";
			SFX.MuteAll();
		}
	}

	private void DetermineDifficulty()
	{
		switch (PersistentData.Instance.Difficulty)
		{
			case Difficulty.Easy:
				difficultyButtonText.text = "Difficulty:\nEasy";
				break;
			case Difficulty.Normal:
				difficultyButtonText.text = "Difficulty:\nNormal";
				break;
			case Difficulty.Hard:
				difficultyButtonText.text = "Difficulty:\nHard";
				break;
		}
	}

	public void UpdateDifficulty()
	{
		switch (PersistentData.Instance.Difficulty)
		{
			case Difficulty.Easy:
				PersistentData.Instance.Difficulty = Difficulty.Normal;
				DetermineDifficulty();
				break;
			case Difficulty.Normal:
				PersistentData.Instance.Difficulty = Difficulty.Hard;
				DetermineDifficulty();
				break;
			case Difficulty.Hard:
				PersistentData.Instance.Difficulty = Difficulty.Easy;
				DetermineDifficulty();
				break;
		}
	}
	
	public void EnableSound()
	{
		if (GameManager.Instance.PlayingAudio)
		{
			GameManager.Instance.PlayingAudio = false;
			SFX.MuteAll();
			DetermineSoundButtonStatus();
			return;
		}
		GameManager.Instance.PlayingAudio = true;
		SFX.UnMuteAll();
		DetermineSoundButtonStatus();
	}

	public void ClearSaveData()
	{
		//Doesn't clear, just resets from defaults
		SaveManager.LoadFromTemplate(defaultInventory);
		SaveManager.LoadFromShonkyTemplate(defaultShonkyInventory);
		SaveManager.LoadFromPersistentDataTemplate();
		SaveManager.SaveInventory();
		SaveManager.SaveShonkyInventory();
		SaveManager.SavePersistentData();
		
		//Reset narrative db
		NarrativeDatabase narrativeDB = Object.Instantiate((NarrativeDatabase) Resources.Load("NarrativeDatabase"));
		narrativeDB.ResetNarrativeFile();
		
		//Reset achievement db
		AchievementDatabase achievementDB = Object.Instantiate((AchievementDatabase) Resources.Load("AchievementDatabase"));
		achievementDB.ResetAchievementFile();

		ResetVariables();
		
		//Send back to main menu
		Initiate.Fade("Intro", Color.black, 2f);
	}

	public void ResetInventory()
	{
		//Reset narrative db
		NarrativeDatabase narrativeDB = Object.Instantiate((NarrativeDatabase) Resources.Load("NarrativeDatabase"));
		narrativeDB.ResetNarrativeFile();
		
		//Reset achievement db
		AchievementDatabase achievementDB = Object.Instantiate((AchievementDatabase) Resources.Load("AchievementDatabase"));
		achievementDB.ResetAchievementFile();
		
		ResetVariables();
		
		if (SceneManager.GetActiveScene().name == "Shop")
		{
			SaveManager.LoadFromTemplate(defaultInventory);
			Initiate.Fade("Shop",Color.black,2f);
		} else if (SceneManager.GetActiveScene().name == "TutorialShop")
		{
			SaveManager.LoadFromTemplate(TutorialInventory);
			Initiate.Fade("TutorialShop",Color.black,2f);
		}
	}

	public void ResetGolems()
	{
		ResetVariables();
		
		if (SceneManager.GetActiveScene().name == "Shop")
		{
			SaveManager.LoadFromShonkyTemplate(defaultShonkyInventory);
			Initiate.Fade("Shop",Color.black,2f);
		} else if (SceneManager.GetActiveScene().name == "TutorialShop")
		{
			SaveManager.LoadFromShonkyTemplate(tutorialShonkyInventory);
			Initiate.Fade("TutorialShop",Color.black,2f);
		}
	}

	private void ResetVariables()
	{
		GameManager.Instance.introduceTrueGolem = false;
		GameManager.Instance.canUseTools = true;
	}

	public void BackToMenu()
	{
		SaveManager.SaveInventory();
		SaveManager.SaveShonkyInventory();
		ResetVariables();
		Initiate.Fade("Intro",Color.black,2f);
	}

	public void Quit()
	{
		Application.Quit();
	}
	
	
}
