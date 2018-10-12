using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroNarrative : MonoBehaviour
{

	public List<Sprite> introImages;
	public List<string> dialogue;
	
	public Image imageHolder;
	public TextMeshProUGUI textHolder;
	private int currentIndex = 0;

	private float cooldown = 0.05f, lastClick;

	public IntroScene introHandler;
	
	// Use this for initialization
	void Start ()
	{
		lastClick = Time.time;
		imageHolder.sprite = introImages[0];
		textHolder.text = dialogue[0];
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(0) && Time.time > lastClick + cooldown)
		{
			if (currentIndex < introImages.Count - 1)
			{
				lastClick = Time.time;
				currentIndex++;
				//Fade old Image and text
				imageHolder.DOFade(0.01f, 1f).OnComplete(() => ShowNewNarrative());
				textHolder.DOFade(0.01f, 1f).OnComplete(() => ShowNewText());
			} else if (currentIndex == 3)
			{
				currentIndex++;
				lastClick = Time.time;
				imageHolder.DOFade(0.01f, 1f).OnComplete(() => ShowNewText());
			}
			else
			{
				if (introHandler.goToTutorial)
					Initiate.Fade("TutorialShop", Color.black, 2.0f);
				else 
					Initiate.Fade("Shop", Color.black, 2f);
			}
		}
	}

	private void ShowNewNarrative()
	{
		imageHolder.sprite = introImages[currentIndex];
		imageHolder.DOFade(1.0f, 1f);
	}

	private void ShowNewText()
	{
		textHolder.text = dialogue[currentIndex];
		textHolder.DOFade(1.0f, 1f);
	}
}
