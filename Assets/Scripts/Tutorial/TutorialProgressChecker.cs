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
	public Canvas Progress;
	public Image BrickFade, ShellFade, JewelFade, ChargedJewelFade, TopArrow,BottomArrow,CombineArrow, GolemFade;
	private string brick = "Brick", shell = "Shell", jewel = "Jewel", chargedJewel = "Charged Jewel", golem = "Golem";
	private Dictionary<string, ImageStatus> schematicProgress;
	private string componentMessage;
	private bool newComponent = false;
	public bool golemMade, readyGolem = false;
	public TextMeshProUGUI textbox;
	public GameObject textBackground;
	public GameObject UIProgressImages;

	public void ShowCanvas(bool showImage)
	{
		Progress.gameObject.SetActive(true);
		Progress.GetComponent<CanvasGroup>().alpha = 1f;
		textBackground.SetActive(true);
		
		if (!showImage){UIProgressImages.SetActive(false);}
		else {UIProgressImages.SetActive(true);}
		
		UpdateCanvas();
	}

	public void HideCanvas()
	{
		Progress.gameObject.SetActive(false);
		textBackground.SetActive(false);
	}

	private void UpdateCanvas()
	{
		foreach (KeyValuePair<string, ImageStatus> entry in schematicProgress)
		{
			Debug.Log("Entry is " + entry.Key + " image is " + entry.Value);
			ImageHandling(entry.Key, entry.Value);
		}

		if (schematicProgress[golem] == ImageStatus.Achieved)
		{
			golemMade = true;
		}

		//Update text box
		if (newComponent)
		{
			textbox.text = componentMessage;
			newComponent = false;
		}
		//Fade Canvas
		StartCoroutine(FadeCanvas());

		//If ready to make a golem give text help
		if (schematicProgress[chargedJewel] == ImageStatus.Achieved && schematicProgress[shell] == ImageStatus.Achieved 
		                                                      && schematicProgress[golem] == ImageStatus.UnAchieved)
		{
			readyGolem = true;
			textbox.text = "Now you can use your wand to combine the shell and charged jewel into a golem!";
			ShowCanvas(false);
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
						image.CrossFadeAlpha(1.0f,3f,false);
						var imgAlpha = image.color;
						imgAlpha.a = 1.0f;
						image.DOFade(1.0f, 3f).OnComplete(() => image.color = imgAlpha);
					}
					SetToAchieved(relevantImages);
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

	private void SetToAchieved(List<Image> images)
	{
		if (images.Contains(BrickFade))
		{
			schematicProgress[brick] = ImageStatus.Achieved;
		} else if (images.Contains(ShellFade))
		{
			schematicProgress[shell] = ImageStatus.Achieved;
		} else if (images.Contains(JewelFade))
		{
			schematicProgress[jewel] = ImageStatus.Achieved;
		} else if (images.Contains(ChargedJewelFade))
		{
			schematicProgress[chargedJewel] = ImageStatus.Achieved;
		}
		else
		{
			schematicProgress[golem] = ImageStatus.Achieved;
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
		}
	}

	public void FinishedComponent(string component)
	{
		componentMessage = string.Format("You just crafted a {0}!", component);
		newComponent = true;
	}

	private IEnumerator FadeCanvas()
	{
		yield return new WaitForSeconds(5);
		Progress.GetComponent<CanvasGroup>().alpha = 0f;
		HideCanvas();
	}

	public void OnlyShowTextBox(string text)
	{
		Progress.gameObject.SetActive(true);
		Progress.GetComponent<CanvasGroup>().alpha = 1f;
		textBackground.SetActive(true);
		UIProgressImages.SetActive(false);
		textbox.text = text;
		StartCoroutine(FadeCanvas());
		StopCoroutine(FadeCanvas());
	}
}
