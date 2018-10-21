using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cinematic : MonoBehaviour
{

	public Canvas creditCanvas;

	public Inventory defaultInventory;

	public ShonkyInventory defaultShonkyInventory;
	// Use this for initialization
	void Start () {
		SFX.Play("MainMenuTrack", 1f, 1f, 0f, true, 0f);
	}
	

	public void ExitToShop()
	{
		//Wipe existing save and send back to main menu
		PlayerPrefs.SetInt("ExistingSave",0);

		
		//Reset GameManager
		GameManager.Instance.InTutorial = false;
		GameManager.Instance.TutorialIntroComplete = false;
		GameManager.Instance.TutorialIntroTopComplete = false;
		GameManager.Instance.InMap = false;
		GameManager.Instance.BarterTutorial = false;
		GameManager.Instance.OfferNPC = false;
		GameManager.Instance.HasInspectedAllInventoryItems = false;
		GameManager.Instance.TutorialGolemMade = false;
		GameManager.Instance.MineGoleminteractGolem = false;
		GameManager.Instance.SendToMine = false;
		GameManager.Instance.HasMinePouch = false;
		GameManager.Instance.WaitingForTimer = false;
		GameManager.Instance.TimerComplete = false;
		GameManager.Instance.ReturnPouch = false;
		GameManager.Instance.OpenPouch = false;
		GameManager.Instance.firstTownSelect = false;
		GameManager.Instance.introduceTrueGolem = false;
		GameManager.Instance.canUseTools = true;
		
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
		
		//Show Credits
		creditCanvas.gameObject.SetActive(true);
		creditCanvas.GetComponent<HideCredits>().StartCredits();
	}

	public void ExitCredits()
	{
		Initiate.Fade("Intro", Color.black,2f);
	}
}
