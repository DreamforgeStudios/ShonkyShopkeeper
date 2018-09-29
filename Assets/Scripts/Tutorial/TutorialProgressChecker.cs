using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialProgressChecker : MonoBehaviour {
	private static TutorialProgressChecker _instance;

	// Lazy instantiation of GameManager.
	// Means that we don't have to manually place GameManager in each scene.
	// Not sure if this is the best way to do this yet...
	public static TutorialProgressChecker Instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<TutorialProgressChecker>();

				if (_instance == null) {
					var container = new GameObject("TutorialManager");
					_instance = container.AddComponent<TutorialProgressChecker>();
					_instance.SetupManager();
				}
			}

			return _instance;
		}
	}
	
	private void Awake() {
		if (_instance == null) {
			_instance = this;
		} else if (_instance != this) {
			Destroy(this.gameObject);
		}
		
		SetupManager();
		HideCanvas();
	}

	private void SetupManager() {
		DontDestroyOnLoad(gameObject);
		schematicProgress = new Dictionary<string, ImageStatus>()
		{
			{brick, ImageStatus.UnAchieved},
			{shell, ImageStatus.UnAchieved},
			{jewel, ImageStatus.UnAchieved},
			{chargedJewel, ImageStatus.UnAchieved},
			{golem, ImageStatus.UnAchieved}
		};
	}

	public bool CanvasEnabled()
	{
		return Progress.isActiveAndEnabled;
	}

	public enum ImageStatus
	{
		Achieved,
		JustAchieved,
		UnAchieved
	}
	
	//Manager start
	public Canvas Progress, mainCanvas;
	public Image BrickFade, ShellFade, JewelFade, ChargedJewelFade, TopArrow,BottomArrow,CombineArrow, GolemFade;
	private string brick = "Brick", shell = "Shell", jewel = "Jewel", chargedJewel = "Charged Jewel", golem = "Golem";
	private Dictionary<string, ImageStatus> schematicProgress;
	private string componentMessage;
	private bool newComponent = false;
	public bool golemMade, readyGolem, golemTextDone = false;
	public GameObject UIProgressImages;
	public bool canvasEnabled = false;
	
	//For text
	public List<string> dialogue, dialogueInstruction;
	public TutorialManager tutManager;

	private void Update()
	{
		//Debug.Log("Canvas enabled is " + canvasEnabled);
		if (canvasEnabled && Input.GetMouseButtonDown(0))
		{
			HideCanvas();
		}
	}

	//Used to store edits to the dictionary
	private List<string> stringsToUpdate;

	public void ShowCanvas(bool showImage)
	{
		Debug.Log("Canvas is shown");
		Progress.gameObject.SetActive(true);
		Progress.GetComponent<CanvasGroup>().alpha = 1f;

		if (!showImage)
		{
			UIProgressImages.SetActive(false);
		}
		else
		{
			UIProgressImages.SetActive(true);
		}

		UpdateCanvas();
	}

	public void HideCanvas()
	{
		Progress.GetComponent<CanvasGroup>().alpha = 0f;
		Progress.gameObject.SetActive(false);
		
		//If ready to make a golem give text help
		if (schematicProgress[chargedJewel] == ImageStatus.Achieved && schematicProgress[shell] == ImageStatus.Achieved 
									&& schematicProgress[golem] == ImageStatus.UnAchieved && !readyGolem)
		{
			readyGolem = true;
			tutManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
			tutManager.StartDialogue(dialogue,dialogueInstruction, tutManager.mainCanvas, tutManager.toolbox.wand, false);
			tutManager.MoveInstructionScroll();
			tutManager.StartFinalComponentParticles();
			tutManager.MoveScrollsToFront();
		}

		if (golemMade && !golemTextDone)
		{
			canvasEnabled = false;
			golemTextDone = true;
			tutManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
			tutManager.IntroduceGolem();
		}
	}

	private void UpdateCanvas()
	{
		canvasEnabled = true;
		stringsToUpdate = new List<string>();
		foreach (KeyValuePair<string, ImageStatus> entry in schematicProgress)
		{
			Debug.Log("Entry is " + entry.Key + " image is " + entry.Value);
			ImageHandling(entry.Key, entry.Value);
		}
		SetToAchieved();

		if (schematicProgress[golem] == ImageStatus.Achieved)
		{
			golemMade = true;
		}
	}

	private void ImageHandling(String item, ImageStatus status)
	{
		List<Image> relevantImages = ItemToImage(item);
		switch (status)
		{
				case ImageStatus.UnAchieved:
					foreach (var image in relevantImages)
					{
						image.gameObject.SetActive(false);
					}
					break;
				case ImageStatus.JustAchieved:
					foreach (var image in relevantImages)
					{
						image.gameObject.SetActive(true);
						//image.CrossFadeAlpha(1.0f,3f,false);
						var imgAlpha = image.color;
						imgAlpha.a = 1.0f;
						image.DOFade(1.0f, 3f).OnComplete(() => image.color = imgAlpha);
					}
					stringsToUpdate.Add(item);
					break;
				case ImageStatus.Achieved:
					foreach (var image in relevantImages)
					{
						image.gameObject.SetActive(true);
					}

					break;
		}
	}

	private List<Image> ItemToImage(string item)
	{
		List<Image> images = new List<Image>();
		switch (item)
		{
			case "Brick":
				images.Add(BrickFade);
				break;
			case "Shell":
				images.Add(ShellFade);
				images.Add(TopArrow);
				break;
			case "Jewel":
				images.Add(JewelFade);
				break;
			case "Charged Jewel":
				images.Add(ChargedJewelFade);
				images.Add(BottomArrow);
				break;
			case "Golem":
				images.Add(GolemFade);
				images.Add(CombineArrow);
				break;
		}
		return images;
	}

	private void SetToAchieved()
	{
		foreach (string entry in stringsToUpdate)
		{
			schematicProgress[entry] = ImageStatus.Achieved;
		}
	}

	public void UpdateItemStatus(string type, ImageStatus achieved)
	{
		switch (type)
		{
			case "Brick":
				schematicProgress[brick] = achieved;
				break;
			case "Shell":
				schematicProgress[shell] = achieved;
				break;
			case "Jewel":
				schematicProgress[jewel] = achieved;
				break;
			case "ChargedJewel":
				schematicProgress[chargedJewel] = achieved;
				break;
			case "Golem":
				schematicProgress[golem] = achieved;
				break;
		}
	}

	public void FinishedComponent(string component)
	{
		componentMessage = string.Format("You just crafted a {0}!", component);
		newComponent = true;
	}

	public void OnlyShowTextBox(string text)
	{
		Progress.gameObject.SetActive(true);
		Progress.GetComponent<CanvasGroup>().alpha = 1f;
		UIProgressImages.SetActive(false);
	}
}
