using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour {
    public List<Button> startButtons;
    public GameObject optionsScreenObject;
    public List<string> narrative;
    private bool text1enabled, continueNarrative, goToTutorial = false;
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
        GameManager.Instance.InTutorial = true;
        SaveManager.LoadFromTemplate(tutorialInventory);
        SaveManager.LoadFromShonkyTemplate(TutorialShonkyInventory);
        SaveManager.SaveInventory();
        SaveManager.SaveShonkyInventory();
        //StartCoroutine(StartIntro());
        Initiate.Fade("TutorialShop", Color.black, 2f);
    }

    public void StartNoTutorial()
    {
        GameManager.Instance.InTutorial = false;
        GameManager.Instance.TutorialIntroComplete = true;
        SaveManager.LoadFromTemplate(defaultInventory);
        SaveManager.LoadFromShonkyTemplate(defaultShonkyInventory);
        SaveManager.SaveInventory();
        SaveManager.SaveShonkyInventory();
        //StartCoroutine(StartIntro());
        Initiate.Fade("Shop", Color.black, 2f);
    }

    public void StartPartyMode()
    {
        //TO BE IMPLEMENTED BY MIKE
    }

    public void RollCredits()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    
/*
    private IEnumerator StartIntro()
    {
        BG.DOColor(Color.black, 2f);
        FadeButtonCompletely(startGame1);
        FadeButtonCompletely(startGame2);
        FadeButtonCompletely(optionsButton);
        yield return new WaitForSeconds(2f);
        text1.text = ProgressNarrative();
        currentNarrativeString++;
        yield return new WaitForSeconds(4f);
        StartCoroutine(ShowText());
        yield return new WaitForSeconds(7f);
        StartCoroutine(ShowText());
        yield return new WaitForSeconds(8f);
        StartCoroutine(ShowText());
        yield return new WaitForSeconds(3f);
        if (goToTutorial)
        {
            Initiate.Fade("TutorialShop", Color.black, 2f);
        }
        else
        {
            Initiate.Fade("Shop", Color.black, 2f);
            NarrativeManager.Read("new_game_01");
        }
    }
    */
    /*
    private void FadeButtonCompletely(Button button)
    {
        button.GetComponent<Image>().DOFade(0f, 2f);
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0f, 2f);
    }

    private IEnumerator ShowText()
    {   
        text1.CrossFadeAlpha(0f,1f,false);
        yield return new WaitForSeconds(1f);
        text1.text = ProgressNarrative();
        text1.CrossFadeAlpha(255f,5f,false);
        StopCoroutine(ShowText());
    }
    
    private string ProgressNarrative()
    {
        if (!text1enabled && currentNarrativeString == 0)
        {
            text1.CrossFadeAlpha(255f, 3f, false);
            text1enabled = false;
            return narrative[currentNarrativeString];
        }
        Debug.Log("Current text " + currentNarrativeString);
        currentNarrativeString++;
        return narrative[currentNarrativeString];
    }*/

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

