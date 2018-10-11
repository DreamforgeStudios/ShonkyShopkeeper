using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour {
    public List<Button> startButtons;
    public GameObject optionsScreenObject, titleScreenPrefab, creditsCanvas, narrativeCanvas;
    public List<string> narrative;
    public bool text1enabled, continueNarrative, goToTutorial = false;
    private int currentNarrativeString = 0;

    public Inventory defaultInventory, tutorialInventory;
    public ShonkyInventory defaultShonkyInventory, TutorialShonkyInventory;

    private void Start()
    {
        SFX.Play("MainMenuTrack", 1f, 1f, 0f, true, 0f);
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
    }

    public void StartNoTutorial()
    {
        goToTutorial = false;
        GameManager.Instance.ActiveGameMode = GameMode.Story;
        GameManager.Instance.InTutorial = false;
        GameManager.Instance.TutorialIntroComplete = true;
        SaveManager.LoadFromTemplate(defaultInventory);
        SaveManager.LoadFromShonkyTemplate(defaultShonkyInventory);
        SaveManager.SaveInventory();
        SaveManager.SaveShonkyInventory();
        titleScreenPrefab.SetActive(false);
        narrativeCanvas.SetActive(true);
        gameObject.SetActive(false);
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
        optionsScreenObject.SetActive(true);
    }

    public void EnableStartScreen()
    {
        foreach (Button option in startButtons)
        {
            option.enabled = true;
        }
    }
}

