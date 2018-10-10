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

	private float cooldown = 3.0f, lastClick;
	
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
			if (currentIndex < dialogue.Count)
			{
				lastClick = Time.time;
				//Fade old Image and text
				imageHolder.DOFade(0.01f, 1f).OnComplete(() => ShowNewNarrative());
				//Fade in new image and next
			}
		}
	}

	private void ShowNewNarrative()
	{
		
	}
}
