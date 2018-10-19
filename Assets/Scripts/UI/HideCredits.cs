using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class HideCredits : MonoBehaviour
{

	public GameObject titleScreen, titleCanvas, optionsScreenPrefab;
	public Vector3 startPosition, endPosition;
	public TextMeshProUGUI credits;
	
	public void HideCreditCanvas()
	{
		titleScreen.SetActive(true);
		titleCanvas.SetActive(true);
		gameObject.SetActive(false);
	}

	public void BackToOptionsIntro()
	{
		credits.transform.DOComplete();
		optionsScreenPrefab.GetComponent<OptionsScreen>().optionsCanvas.gameObject.SetActive(true);
		gameObject.SetActive(false);
	}

	public void StartCredits()
	{
		credits.transform.position = startPosition;
		Sequence seq = DOTween.Sequence();
		seq.Append(credits.transform.DOMove(endPosition, 60f, false)
			.OnComplete(() => credits.transform.position = startPosition));
		seq.SetLoops(-1);
		seq.Play();
	}
	
}
