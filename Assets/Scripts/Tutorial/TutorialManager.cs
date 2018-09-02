﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	//Default inventories to be load at start
	public Inventory TutorialInventory, RegularInventory;
	public ShonkyInventory EmptyInventory, RegularGolemInventory;
	public TutorialPhysicalInventory physicalInv;
	public PhysicalShonkyInventory golemInv;

	//Tools to inspect and have been inspected
	public List<GameObject> ItemsToInspect;
	public List<GameObject> ItemsInspected;
	public GameObject currentToolToInspect;
	
	//UI Elements
	public List<string> TutorialDialogue;
	public List<string> ToolDialogue;
	public Canvas tutorialCanvas;
	public TextMeshProUGUI tutorialText;
	private int currentDialogue = 0;
	public Button travelButton;
	public Button cameraButton;
	//Image used to highlight cameraButton
	public Image cameraHighlight;
	
	//Particle system to highlight items to be inspected
	public GameObject particles;
	private GameObject particleChild;
	
	// Use this for initialization
	void Start () {
		SetupInventories();
		//StartParticles();
		tutorialCanvas.gameObject.SetActive(false);
		if (!GameManager.Instance.TutorialIntroComplete)
			TutorialProgressChecker.Instance.HideCanvas();

		if (GameManager.Instance.InTutorial)
		{
			travelButton.gameObject.SetActive(false);
			cameraButton.gameObject.SetActive(false);
			if (!GameManager.Instance.TutorialIntroComplete)
				StartDialogue();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckForInput();

		if (!GameManager.Instance.InTutorial)
		{
			tutorialCanvas.enabled = false;
			travelButton.gameObject.SetActive(true);
		}
			
	}

	private void StartDialogue()
	{
		tutorialCanvas.gameObject.SetActive(true);
		tutorialText.text = TutorialDialogue[0];
		
	}

	public void SkipTutorial()
	{
		GameManager.Instance.InTutorial = false;
		GameManager.Instance.InMap = true;
		LoadNormalInventory();
	}

	private void CheckForInput()
	{
		
		if (Input.GetMouseButtonDown(0) && !GameManager.Instance.TutorialIntroTopComplete)
		{
			NextDialogue();
		} else if (Input.GetMouseButtonDown(0) && GameManager.Instance.TutorialIntroComplete &&
		           TutorialProgressChecker.Instance.CanvasEnabled())
		{
			TutorialProgressChecker.Instance.HideCanvas();
		}

		if (TutorialProgressChecker.Instance.golemMade && !GameManager.Instance.MineGoleminteractGolem)
			CheckForCamera();
		else if (GameManager.Instance.MineGoleminteractGolem && GameManager.Instance.OpenPouch)
		{
			PouchText();
			cameraButton.gameObject.SetActive(true);
		}
			
	}

	public void NextDialogue()
	{
		//Debug.Log(currentDialogue + " out of " + TutorialDialogue.Count);
		if (currentDialogue != 2)
		{
			currentDialogue += 1;
			if (currentDialogue == 2)
			{
				cameraButton.enabled = true;
				cameraButton.gameObject.SetActive(true);
				cameraHighlight.gameObject.SetActive(true);
			} else if (currentDialogue == 3)
			{
				cameraHighlight.gameObject.SetActive(false);
				StartParticles(ItemsToInspect[0]);
			}
		} else if (currentDialogue == 2)
		{
			if (GameManager.Instance.TutorialIntroTopComplete)
			{
				cameraHighlight.gameObject.SetActive(false);
				currentDialogue++;
			}
		} 
		if (currentDialogue < TutorialDialogue.Count)
		{
			tutorialText.text = TutorialDialogue[currentDialogue];
		}
		else
		{
			GameManager.Instance.TutorialIntroComplete = true;
			tutorialCanvas.gameObject.SetActive(false);
		}
		
		//Need to start particles for tools		
	}

	private void CheckForCamera()
	{
		if (GameManager.Instance.CameraRotTransfer <= 9f)
		{
			cameraButton.gameObject.SetActive(false);
			tutorialCanvas.gameObject.SetActive(false);
			if (GameManager.Instance.SendToMine)
			{
				TutorialProgressChecker.Instance.OnlyShowTextBox("Try picking up your golem and sending it to the mine");
				GameManager.Instance.SendToMine = false;
				HideCanvas();
			}
		}
		else 
		{
			cameraButton.gameObject.SetActive(true);
		}	
	}

	private void PouchText()
	{
		cameraButton.gameObject.SetActive(true);
		TutorialProgressChecker.Instance.OnlyShowTextBox("Use the magnifying glass on the pouch to see what is inside");
		GameManager.Instance.OpenPouch = false;
	}
	public void StartForcepParticles()
	{
		GameObject tool = ItemsToInspect[0];
		particleChild = Instantiate(particles, tool.transform.position, tool.transform.rotation);
		particleChild.transform.parent = tool.transform;
		currentToolToInspect = tool;
	}

	public void HideCanvas()
	{
		tutorialCanvas.gameObject.SetActive(false);
	}

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
		tutorialCanvas.gameObject.SetActive(true);
		travelButton.gameObject.SetActive(true);
		tutorialText.text =
			"Congratulations on making your first golem! I have filled your inventory with more resources." +
			" you can continue to practice or click the map to start your journey";
	}

	private void StartParticles(GameObject tool)
	{
		if (!GameManager.Instance.HasInspectedAllInventoryItems)
		{
			
			particleChild = Instantiate(particles, tool.transform.position, tool.transform.rotation);
			particleChild.transform.parent = tool.transform;
			/*
			foreach (GameObject obj in ItemsToInspect)
			{
				particleChild = Instantiate(particles, obj.transform.position, obj.transform.rotation);
				particleChild.transform.parent = obj.transform;
			}
			*/
			
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

	private void StopParticle(GameObject tool)
	{
		particleChild = tool.transform.Find("TutorialShine(Clone)").gameObject;
		Destroy(particleChild);
	}

	public void InspectItem(GameObject tool)
	{
		if (!GameManager.Instance.HasInspectedAllInventoryItems && !GameManager.Instance.InspectedItems.Contains(tool.name) &&
		    !GameManager.Instance.TutorialIntroComplete)
		{
			ItemsToInspect.Remove(tool);
			ItemsInspected.Add(tool);
			GameManager.Instance.InspectedItems.Add(tool.name);
			StopParticle(tool);
			switch (tool.tag)
			{
				case "Forceps":
					tutorialCanvas.gameObject.SetActive(true);
					tutorialText.text = ToolDialogue[2];
					Debug.Log("Forceps text");
					StartParticles(ItemsToInspect[0]);
					currentToolToInspect = ItemsToInspect[0];
					break;
				case "Magnifyer":
					tutorialCanvas.gameObject.SetActive(true);
					tutorialText.text = ToolDialogue[0];
					StartParticles(ItemsToInspect[0]);
					currentToolToInspect = ItemsToInspect[0];
					break;
				case "Wand":
					tutorialCanvas.gameObject.SetActive(true);
					tutorialText.text = ToolDialogue[3];
					physicalInv.HighlightOreAndGem();
					StartCoroutine(WandInteraction());
					break;
				case "Bin":
					tutorialCanvas.gameObject.SetActive(true);
					tutorialText.text = ToolDialogue[1];
					StartParticles(ItemsToInspect[0]);
					currentToolToInspect = ItemsToInspect[0];
					break;
			}
		}

		//Time.timeScale = 0f;
	}

	IEnumerator WandInteraction()
	{
		yield return new WaitForSeconds(1f);
		tutorialText.text = ToolDialogue[4];
		StopCoroutine(WandInteraction());
	}

	IEnumerator WaitforsecondsCanvasHide(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		tutorialCanvas.gameObject.SetActive(false);
	}
}
