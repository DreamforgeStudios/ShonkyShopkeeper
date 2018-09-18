using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour {
    //For Old Intro
    public RawImage BG;
    public Button startGame1, startGame2, optionsButton;
    public GameObject optionsObject;
    public TextMeshProUGUI text1;
    public List<string> narrative;
    private bool text1enabled, continueNarrative, goToTutorial = false;
    private int currentNarrativeString = 0;

    public Inventory defaultInventory, tutorialInventory;
    public ShonkyInventory defaultShonkyInventory, TutorialShonkyInventory;

    private void Start()
    {
        text1.CrossFadeAlpha(0f, 0.25f, false);
        SFX.Play("MainMenuTrack", 1f, 1f, 0f, true, 0f);
    }

    public void StartTutorial()
    {
        optionsObject.SetActive(false);
        goToTutorial = true;
        GameManager.Instance.InTutorial = true;
        SaveManager.LoadFromTemplate(tutorialInventory);
        SaveManager.LoadFromShonkyTemplate(TutorialShonkyInventory);
        SaveManager.SaveInventory();
        SaveManager.SaveShonkyInventory();
        StartCoroutine(StartIntro());
        //Initiate.Fade("TutorialShop", Color.black, 2f);
    }

    public void StartNoTutorial()
    {
        optionsObject.SetActive(false);
        GameManager.Instance.InTutorial = false;
        GameManager.Instance.TutorialIntroComplete = true;
        SaveManager.LoadFromTemplate(defaultInventory);
        SaveManager.LoadFromShonkyTemplate(defaultShonkyInventory);
        SaveManager.SaveInventory();
        SaveManager.SaveShonkyInventory();
        StartCoroutine(StartIntro());
        //Initiate.Fade("Shop", Color.black, 2f);
    }

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
    }

    public void EnableOptionsScreen()
    {
        Debug.Log("Clicked Button");
        optionsObject.SetActive(true);
        optionsObject.GetComponent<OptionsScreen>().EnableOptions();
    }
}

