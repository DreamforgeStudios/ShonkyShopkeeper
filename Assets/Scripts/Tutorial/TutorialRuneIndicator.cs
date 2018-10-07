using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialRuneIndicator : MonoBehaviour
{
	public Image rune, core;
	private Transform runeT;
	public GameObject objectOver;
	private bool overCanvasElement, startedCoroutine = false;
	
	// Use this for initialization
	void Start () {
		//Get images relative transforms
		Transform coreT = core.GetComponent<Transform>();
		runeT = rune.GetComponent<Transform>();
		//Create looping sequence for the pulsing core
		Sequence coreSeq = DOTween.Sequence();
		coreSeq.Append(core.DOFade(0.1f, 2f));
		coreSeq.Append(core.DOFade(0.9f, 2f));
		coreSeq.SetLoops(-1);
		coreSeq.Play();

	}

	private void Update()
	{
		runeT.Rotate(Vector3.forward,360f * Time.deltaTime/3f);
		
		if (!startedCoroutine)
			//Start coroutine to keep it over the element
			StartCoroutine(KeepOverElement());
			
	}

	//Sets the rune over the top of either the selected gameobject or canvas element
	public void SetPosition(GameObject objectToBeOver, bool canvasElement)
	{
		objectOver = objectToBeOver;
		overCanvasElement = canvasElement;
		runeT = rune.GetComponent<Transform>();
		Vector2 pos = new Vector2(0f,0f);
		if (canvasElement)
		{
			//pos = Camera.main.ScreenToViewportPoint(objectToBeOver.transform.position);
			//pos = Camera.main.ViewportToScreenPoint(pos);
			//runeT.position = pos;
			runeT.position = objectToBeOver.transform.position;
		}
		else
		{
			pos = Camera.main.WorldToViewportPoint(objectToBeOver.transform.position);
			pos = Camera.main.ViewportToScreenPoint(pos);
			runeT.position = pos;
		}
		//Reset Coroutine
		StopCoroutine(KeepOverElement());
		startedCoroutine = false;

	}

	//Need to handle when objects need to move
	private IEnumerator KeepOverElement()
	{
		while (true)
		{
			startedCoroutine = true;
			if (overCanvasElement)
			{
				if (runeT.position != objectOver.transform.position)
					runeT.position = objectOver.transform.position;
			}
			else
			{
				if (runeT.position != Camera.main.WorldToScreenPoint(objectOver.transform.position))
					runeT.position = Camera.main.WorldToScreenPoint(objectOver.transform.position);
			}
			
			yield return new WaitForSeconds(0.05f);
		}
	}
}
