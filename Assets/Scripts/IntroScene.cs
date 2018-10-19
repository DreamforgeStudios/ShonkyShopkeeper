using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour {
    public List<Button> startButtons, existingSaveButtons, noSaveButtons;
    public GameObject optionsScreenObject, titleScreenPrefab, creditsCanvas, narrativeCanvas;
    public bool goToTutorial = false;

    public Inventory defaultInventory, tutorialInventory;
    public ShonkyInventory defaultShonkyInventory, TutorialShonkyInventory;

    private void Start()
    {
        SFX.Play("MainMenuTrack", 1f, 1f, 0f, true, 0f);
        if (PlayerPrefs.GetInt("ExistingSave") == 1)
        {
            foreach(Button option in existingSaveButtons)
                option.gameObject.SetActive(true);
            
            foreach(Button option in noSaveButtons)
                option.gameObject.SetActive(false);
        }
        else
        {
            foreach(Button option in existingSaveButtons)
                option.gameObject.SetActive(false);
            
            foreach(Button option in noSaveButtons)
                option.gameObject.SetActive(true);
        }
    }

    public void StartTutorial()
    {
        goToTutorial = true;
        GameManager.Instance.ActiveGameMode = GameMode.Story;
        GameManager.Instance.InTutorial = true;
        SaveManager.LoadFromTemplate(tutorialInventory);
        SaveManager.LoadFromShonkyTemplate(TutorialShonkyInventory);
        SaveManager.SaveInventory();
        SaveManager.SaveShonkyInventory();
        titleScreenPrefab.SetActive(false);
        narrativeCanvas.SetActive(true);
        gameObject.SetActive(false);
        
        //Reset narrative db
        NarrativeDatabase narrativeDB = Object.Instantiate((NarrativeDatabase) Resources.Load("NarrativeDatabase"));
        narrativeDB.ResetNarrativeFile();
		
        //Reset achievement db
        AchievementDatabase achievementDB = Object.Instantiate((AchievementDatabase) Resources.Load("AchievementDatabase"));
        achievementDB.ResetAchievementFile();
    }

    public void StartNoTutorial()
    {
        goToTutorial = false;
        GameManager.Instance.ActiveGameMode = GameMode.Story;
        GameManager.Instance.InTutorial = false;
        GameManager.Instance.TutorialIntroComplete = true;
        GameManager.Instance.firstTownSelect = true;
        SaveManager.LoadFromTemplate(defaultInventory);
        SaveManager.LoadFromShonkyTemplate(defaultShonkyInventory);
        SaveManager.SaveInventory();
        SaveManager.SaveShonkyInventory();
        titleScreenPrefab.SetActive(false);
        narrativeCanvas.SetActive(true);
        gameObject.SetActive(false);
        
        //Reset narrative db
        NarrativeDatabase narrativeDB = Object.Instantiate((NarrativeDatabase) Resources.Load("NarrativeDatabase"));
        narrativeDB.ResetNarrativeFile();
		
        //Reset achievement db
        AchievementDatabase achievementDB = Object.Instantiate((AchievementDatabase) Resources.Load("AchievementDatabase"));
        achievementDB.ResetAchievementFile();
    }

    public void ResumeGame()
    {
        SaveManager.LoadOrInitializeInventory(defaultInventory);
        SaveManager.LoadOrInitializeShonkyInventory(defaultShonkyInventory);
        Initiate.Fade("Shop",Color.black,2.0f);
    }

    public void StartPartyMode()
    {
        GameManager.Instance.ActiveGameMode = GameMode.Party;
        Initiate.Fade("Setup", Color.black, 2f);
    }

    public void RollCredits()
    {
        titleScreenPrefab.SetActive(false);
        gameObject.SetActive(false);
        creditsCanvas.SetActive(true);
        creditsCanvas.GetComponent<HideCredits>().StartCredits();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EnableOptionsScreen()
    {
        foreach (Button option in startButtons)
        {
            option.enabled = false;
        }
        foreach (Button option in existingSaveButtons)
        {
            option.enabled = false;
        }
        foreach (Button option in noSaveButtons)
        {
            option.enabled = false;
        }
        optionsScreenObject.SetActive(true);
    }

    public void EnableStartScreen()
    {
        foreach (Button option in startButtons)
        {
            option.enabled = true;
        }
        foreach (Button option in existingSaveButtons)
        {
            option.enabled = true;
        }
        foreach (Button option in noSaveButtons)
        {
            option.enabled = true;
        }
    }
}

