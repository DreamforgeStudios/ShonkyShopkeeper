using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{

	public Image BG;
	public GameObject parentObject, optionsParent, creditsParent;
	public List<Button> OptionButtons;
	public TextMeshProUGUI soundButtonText;

	public Inventory defaultInventory;
	public ShonkyInventory defaultShonkyInventory;
	
	// Use this for initialization
	void Start () {
		//parentObject.SetActive(false);
	}

	public void EnableOptions()
	{
		parentObject.SetActive(true);
		BG.CrossFadeAlpha(255f,2f,false);
		optionsParent.SetActive(true);
		creditsParent.SetActive(false);
		DetermineSoundButtonStatus();
	}

	public void DisableOptions()
	{
		BG.CrossFadeAlpha(2f,2f,false);
		parentObject.SetActive(false);
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
			SFX.StopAll();
		}
		
	}
	public void EnableSound()
	{
		if (GameManager.Instance.PlayingAudio)
		{
			GameManager.Instance.PlayingAudio = false;
			DetermineSoundButtonStatus();
			return;
		}
		GameManager.Instance.PlayingAudio = true;
		DetermineSoundButtonStatus();
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Credits()
	{
		optionsParent.SetActive(false);
		creditsParent.SetActive(true);
	}

	public void ClearSaveData()
	{
		//Doesn't clear, just resets from defaults
		GameManager.Instance.InTutorial = false;
		GameManager.Instance.TutorialIntroComplete = true;
		SaveManager.LoadFromTemplate(defaultInventory);
		SaveManager.LoadFromShonkyTemplate(defaultShonkyInventory);
		SaveManager.SaveInventory();
		SaveManager.SaveShonkyInventory();
	}
	
	
}
