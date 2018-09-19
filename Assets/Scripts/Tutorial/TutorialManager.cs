﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	//Default inventories to be load at start
	public Inventory TutorialInventory, RegularInventory;
	public ShonkyInventory EmptyInventory, RegularGolemInventory;
	public TutorialPhysicalInventory physicalInv;
	public PhysicalShonkyInventory golemInv;
	public TutorialToolbox toolbox;

	//Tools to inspect and have been inspected
	public List<GameObject> ItemsToInspect;
	public List<GameObject> ItemsInspected;
	public GameObject currentToolToInspect;

	//UI Elements
	[FormerlySerializedAs("TutorialDialogue")] public List<string> TutorialCameraExpo;
	public List<string> TutorialCameraInstruction;
	public List<string> ToolIntro;
	public List<string> forceps;
	public List<string> magnifier;
	public List<string> magnifierInstruction;
	public List<string> wand;
	public List<string> wandInstruction;
	public List<string> introduceGolem;
	public List<string> pickUpGolem;
	public List<string> retrieveGolem;
	public List<string> tapPouch;
	public List<string> openPouch;
	public List<string> tutorialFinish;

	private int currentDialogue = 0;
	public Button travelButton;

	public Button cameraButton;

	//Image used to highlight cameraButton
	public Image cameraHighlight;
	
	//Gameobject to be used with the mine
	public GameObject mineTarget;

	//Particle system to highlight items to be inspected
	public GameObject particles;
	//public GameObject binParticles;
	private GameObject particleChild;
	private bool InspectedItem = false;

	//For text
	public GameObject speechBubblePrefab;
	private InstructionBubble clone;
	public Canvas mainCanvas;

	// Use this for initialization
	void Start()
	{
		SetupInventories();
		if (!GameManager.Instance.TutorialIntroComplete)
		{
			TutorialProgressChecker.Instance.HideCanvas();
			EnableCameraTap(false, false);
		}


		if (GameManager.Instance.InTutorial)
		{
			travelButton.gameObject.SetActive(false);
			cameraButton.gameObject.SetActive(false);
			if (!GameManager.Instance.TutorialIntroComplete)
			{
				StartDialogue(TutorialCameraExpo, TutorialCameraInstruction, cameraButton.gameObject, true);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		CheckForInput();

		if (!GameManager.Instance.InTutorial)
			travelButton.gameObject.SetActive(true);
	}

	public void StartDialogue(List<string> dialogue, List<string> instruction, GameObject target, bool canvasElement)
	{
		toolbox.canSelect = false;
		
		if (clone != null)
			clone.DestroyItem();
		
        clone = Instantiate(speechBubblePrefab, mainCanvas.transform)
	        .GetComponentInChildren<InstructionBubble>();
		clone.SetText(dialogue,instruction);
		clone.Init(target,canvasElement);

	}

	public void MoveInstructions()
	{
		clone.MoveInstructionScroll();
	}

	public void SkipTutorial()
	{
		GameManager.Instance.InTutorial = false;
		GameManager.Instance.InMap = true;
		LoadNormalInventory();
	}

	private void CheckForInput()
	{
		//Debug.Log("Can select" + toolbox.canSelect);
		
		if (!GameManager.Instance.TutorialIntroTopComplete && clone.Instruction)
		{
			InstructionBubble.onInstruction += EnableCameraTap(true, true);
		} else if (!GameManager.Instance.TutorialIntroComplete && ItemsToInspect.Contains(currentToolToInspect) && clone.Instruction)
		{
			//List is Forceps, ResourceBin, Magnifying Glass & Wand
			Debug.Log("Current tool to inspect is" + currentToolToInspect.gameObject.name);
			string toolString = currentToolToInspect.gameObject.name;
			switch (toolString)
			{
				case "Forceps":
					if (!InspectedItem)
					{
						particleChild = Instantiate(particles, currentToolToInspect.transform.position, currentToolToInspect.transform.rotation);
						particleChild.transform.parent = currentToolToInspect.transform;
						InspectedItem = true;
					}
					toolbox.canSelect = true;
					break;
				case "Magnifying Glass":
					if (!InspectedItem)
					{
						particleChild = Instantiate(particles, currentToolToInspect.transform.position, currentToolToInspect.transform.rotation);
						particleChild.transform.parent = currentToolToInspect.transform;
						InspectedItem = true;
					}
					toolbox.canSelect = true;
					break;
				case "Wand":
					if (!physicalInv.createdParticles)
					{
						InstructionBubble.onInstruction += physicalInv.HighlightOreAndGem();
						InstructionBubble.onInstruction += InspectItem(currentToolToInspect);
						
					}
					toolbox.canSelect = true;
					break;
				default:
					toolbox.canSelect = true;
					break;
			}

			//toolbox.canSelect = true;
		}
		else if (InspectedAllItems())
		{
			toolbox.canSelect = true;
		}
		else if (clone != null)
		{
			InstructionBubble.onInstruction += () => toolbox.canSelect = true;
			InstructionBubble.onInstruction -= () => toolbox.canSelect = true;
			if (ItemsToInspect[0].gameObject.name == "Magnifying Glass")
			{
				InstructionBubble.onInstruction += () => currentToolToInspect = ItemsToInspect[0];
				InstructionBubble.onInstruction -= () => currentToolToInspect = ItemsToInspect[0];
			}
		} 
		else
		{
			toolbox.canSelect = false;
		}

		
		if (TutorialProgressChecker.Instance.golemMade && !GameManager.Instance.MineGoleminteractGolem)
			CheckForCamera();
		else if (GameManager.Instance.MineGoleminteractGolem)// && GameManager.Instance.OpenPouch)
		{
			//PouchText();
			cameraButton.gameObject.SetActive(true);
		}
		
	}

	//Self explanatory
	public InstructionBubble.Instruct EnableCameraTap(bool button, bool glow)
	{
		Debug.Log("enabling camera");
		cameraButton.enabled = button;
		cameraButton.gameObject.SetActive(button);
		cameraHighlight.gameObject.SetActive(glow);
		return () => {InstructionBubble.onInstruction -= EnableCameraTap(false, false); };
	}

	public void IntroduceGolem()
	{
		StartDialogue(introduceGolem, introduceGolem, mineTarget, false);
		GameManager.Instance.SendToMine = true;
	}

	public void FinishForcepsMovement()
	{
		physicalInv.DestroyParticlesOnItems();
		InspectItem(currentToolToInspect);
	}

	public void FinishInspectorUse()
	{
		physicalInv.DestroyParticlesOnItems();
		InspectItem(currentToolToInspect);
	}
	
	private void CheckForCamera()
	{
		if (GameManager.Instance.CameraRotTransfer <= 9f)
		{
			cameraButton.gameObject.SetActive(false);
			if (GameManager.Instance.SendToMine)
			{
				StartDialogue(pickUpGolem, pickUpGolem, mineTarget, false);
				GameManager.Instance.SendToMine = false;
			}
		}
		else
		{
			cameraButton.gameObject.SetActive(true);
			cameraButton.enabled = true;
		}
	}

	public void PouchText()
	{
		cameraButton.gameObject.SetActive(true);
		StartDialogue(openPouch, openPouch, ItemsInspected[2], false);
		GameManager.Instance.OpenPouch = false;
	}

	public void StartToolText()
	{
		GameObject tool = ItemsToInspect[0];
		StartDialogue(ToolIntro,forceps,tool,false);
		currentToolToInspect = tool;
	}

	/*
	public void StartForcepParticles()
	{
		particleChild = Instantiate(particles, tool.transform.position, tool.transform.rotation);
		particleChild.transform.parent = tool.transform;
		currentToolToInspect = tool;
		StartDialogue(forceps, forcepsInstruction, ItemsToInspect[0], false);
	}
	*/

	private void SetupInventories()
	{
		if (!GameManager.Instance.HasInspectedAllInventoryItems)
		{
			SaveManager.LoadFromTemplate(TutorialInventory);
			SaveManager.LoadFromShonkyTemplate(EmptyInventory);
			SaveManager.SaveInventory();
			SaveManager.SaveShonkyInventory();
			//physicalInv.PopulateInitial();
			golemInv.PopulateInitial();
		}
	}

	public void LoadNormalInventory()
	{
		SaveManager.LoadFromTemplate(RegularInventory);
		SaveManager.SaveInventory();
		SaveManager.SaveShonkyInventory();
		//physicalInv.PopulateInitial();
		travelButton.gameObject.SetActive(true);
	}

	public void FinishTutorial()
	{
		StartDialogue(tutorialFinish, tutorialFinish, travelButton.gameObject, true);
		SaveManager.SaveInventory();
		SaveManager.SaveShonkyInventory();
		travelButton.gameObject.SetActive(true);
		GameManager.Instance.CameraRotTransfer = 8f;
	}

	private void StartParticles(GameObject tool)
	{
		if (!GameManager.Instance.HasInspectedAllInventoryItems)
		{
			particleChild = Instantiate(particles, tool.transform.position, tool.transform.rotation);
			particleChild.transform.parent = tool.transform;
		}
	}

	public bool Inspected(GameObject tool)
	{
		if (ItemsToInspect.Contains(tool))
			return false;

		return true;
	}

	public bool InspectedAllItems()
	{
		if (ItemsToInspect.Count == 0)
		{
			GameManager.Instance.HasInspectedAllInventoryItems = true;
			GameManager.Instance.TutorialIntroComplete = true;
			return true;
		}

		if (GameManager.Instance.HasInspectedAllInventoryItems)
			return true;

		return false;
	}

	public void StopParticle(GameObject tool)
	{
		if (tool.transform.Find("TutorialShine(Clone)") != null)
		{
			particleChild = tool.transform.Find("TutorialShine(Clone)").gameObject;
			Destroy(particleChild);
		}			
	}

	public InstructionBubble.Instruct InspectItem(GameObject tool)
	{
		if (!GameManager.Instance.HasInspectedAllInventoryItems &&
		    !GameManager.Instance.InspectedItems.Contains(tool.name) &&
		    !GameManager.Instance.TutorialIntroComplete)
		{
			ItemsToInspect.Remove(tool);
			ItemsInspected.Add(tool);
			GameManager.Instance.InspectedItems.Add(tool.name);
			StopParticle(tool);
			toolbox.canSelect = false;
			InspectedItem = false;
			//Now need to transfer to next tool
			switch (tool.tag)
			{
				case "Forceps":
					//StartParticles(ItemsToInspect[0]);
					//currentToolToInspect = ItemsToInspect[0];
					StartDialogue(magnifier, magnifierInstruction, ItemsToInspect[0], false);
					break;
				case "Magnifyer":
					//StartParticles(ItemsToInspect[0]);
					currentToolToInspect = ItemsToInspect[0];
					StartDialogue(wand, wandInstruction, ItemsToInspect[0], false);
					break;
				case "Wand":
					physicalInv.HighlightOreAndGem();
					break;
			}
		}
		else
		{
			toolbox.canSelect = true;
		}
		return () => { InstructionBubble.onInstruction -= InspectItem(currentToolToInspect); };
	}

	public void NextInstruction()
	{
		clone.NextInstructionText();
	}

	public void HideExposition()
	{
		Debug.Log("Hiding Exposition");
		clone.DestroyItem();
	}

	public void StartItemParticles()
	{
		if (!physicalInv.createdParticles)
			InstructionBubble.onInstruction += physicalInv.HighlightOreAndGem();

		toolbox.canSelect = true;
	}

	public void StartItemParticle()
	{
		
	}
}
