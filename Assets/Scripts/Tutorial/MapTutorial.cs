using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class MapTutorial : MonoBehaviour
{

	private static MapTutorial _instance;

	// Lazy instantiation of GameManager.
	// Means that we don't have to manually place GameManager in each scene.
	// Not sure if this is the best way to do this yet...
	public static MapTutorial Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<MapTutorial>();

				if (_instance == null)
				{
					var container = new GameObject("TutorialManager");
					_instance = container.AddComponent<MapTutorial>();
				}
			}

			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this)
		{
			Destroy(this.gameObject);
		}

		if (GameManager.Instance.InMap)
			CheckForTutProgressChecker();
		else
		{
			CanMoveCamera = true;
			Destroy(this.gameObject);
		}
	}

	//MAP TUTORIAL START

	public GameObject shopButtonObj, sphere, particles, particleChild, speechBubblePrefab, introTarget, runeIndicatorPrefab;
	public bool clickedOrb, CanMoveCamera, canSelectTowns, createdRunes  = false;
	public List<string> intro, introInstructions, map, mapInstructions;
	private List<GameObject> runeIndicators;
	public List<GameObject> towns;
	public InstructionBubble clone;
	public Canvas mainCanvas;
	public Hall hallScript;

	private void Start()
	{
		CanMoveCamera = false;
		InstructionBubble.ClearOldEvents();
		StartDialogue(intro,introInstructions, mainCanvas, sphere, false);
		InstructionBubble.onInstruction += () => CanMoveCamera = true;
		StartSphereParticles();
	}
	
	private void CheckForTutProgressChecker()
	{
		DontDestroyOnLoad(gameObject);
		GameObject obj = GameObject.FindGameObjectWithTag("TutorialProgress");
		if (obj != null)
			Destroy(obj);
		
		shopButtonObj.SetActive(false);
	}
	
	public void StartDialogue(List<string> dialogue, List<string> instruction, Canvas canvas, GameObject target, bool canvasElement)
	{	
		if (clone != null)
			clone.DestroyItem();
		
		clone = Instantiate(speechBubblePrefab, mainCanvas.transform)
			.GetComponentInChildren<InstructionBubble>();
		clone.SetText(dialogue,instruction);
		clone.Init(target,canvasElement,canvas);

	}

	public void ClickedSphere()
	{
		StopSphereParticle();
		InstructionBubble.ClearOldEvents();
		clickedOrb = true;
		//hallScript.canMoveAround = false;
		hallScript.MoveCameraBackButton.SetActive(false);
		StartDialogue(map, mapInstructions, mainCanvas, introTarget,true);
		InstructionBubble.onInstruction += () => AllowTownSelection();
		HighlightAllTowns();
	}

	private void AllowTownSelection()
	{
		hallScript.canMoveAround = true;
		canSelectTowns = true;
	}
	
	public void NextInstruction()
	{
		clone.NextInstructionText();
	}
	
	private InstructionBubble.Instruct StartSphereParticles()
	{
		Debug.Log("starting sphere particles");
		particleChild = Instantiate(particles, sphere.transform.position, sphere.transform.rotation);
		particleChild.transform.parent = sphere.transform;
		return () => { InstructionBubble.onInstruction -= StartSphereParticles(); };
	}
	
	private void StopSphereParticle()
	{
		particleChild = sphere.transform.Find("SphereShine(Clone)").gameObject;
		Destroy(particleChild);
	}

	private void ShowShopButton()
	{
		shopButtonObj.SetActive(true);
	}

	public void HighlightAllTowns()
	{
		/*
		if (!createdRunes)
		{
			runeIndicators = new List<GameObject>();
			createdRunes = true;
			foreach (GameObject town in towns)
			{
				GameObject rune = Instantiate(runeIndicatorPrefab, mainCanvas.transform);
				rune.GetComponent<TutorialRuneIndicator>().SetPosition(town,false);
				runeIndicators.Add(rune);
			}
			//Set instructions to front
			clone.MoveScrollsToFront();
		}*/
		clone.MoveScrollsToFront();
		
	}
	/*

	public void RemoveHighlightAllTowns()
	{
		if (runeIndicators != null)
		{
			runeIndicators.Clear();
		}

		createdRunes = false;
	}

	public void DeactivateAllTownHighlights()
	{
		if (runeIndicators != null)
		{
			foreach (var VARIABLE in runeIndicators)
			{
				VARIABLE.SetActive(false);
			}
		}
	}

	public void ReactivateALlTownHighlights()
	{
		if (runeIndicators != null)
		{
			//Need to reactivate and reset their tracking
			for (int i = 0; i < runeIndicators.Count; i++)
			{
				runeIndicators[i].SetActive(true);
				runeIndicators[i].GetComponent<TutorialRuneIndicator>().SetPosition(towns[i],false);
			}
		}
		else
		{
			HighlightAllTowns();
		}
	}
	*/

}
