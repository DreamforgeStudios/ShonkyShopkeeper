using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
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
					Instance.UpdateDictionary();
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
		Ore = true;
		Gem = true;
	}

	public bool CanvasEnabled()
	{
		return Progress.isActiveAndEnabled;
	}
	
	//Manager start
	public bool Ore, Brick, Shell, Gem, Jewel, ChargedJewel, readyGolem, Golem = false;
	public Canvas Progress;
	public Image Top, Bottom, GolemImg, BrickFade, ShellFade, JewelFade, ChargedJewelFade, GolemFade;
	private Dictionary<Image, bool> booleanProgress = new Dictionary<Image, bool>();
	private string componentMessage;
	private bool newComponent, golemText = false;
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
		UpdateDictionary();
		Debug.Log(booleanProgress.Count);
		foreach (KeyValuePair<Image, bool> entry in booleanProgress)
		{
			Debug.Log("Entry is " + entry.Key + " image is " + entry.Value);
			if (entry.Value && entry.Key.color.a >= 0.5f)
			{
				entry.Key.CrossFadeAlpha(0.0f,3f,false);
			}
		}
		//Update text box
		if (newComponent)
		{
			textbox.text = componentMessage;
			newComponent = false;
		}
		//Fade Canvas
		StartCoroutine(FadeCanvas());

		if (Shell && ChargedJewel)
			readyGolem = true;
		//If ready to make a golem give text help
		if (readyGolem && !golemText)
		{
			golemText = true;
			textbox.text = "Now you can use your wand to combine the shell and charged jewel into a golem!";
			ShowCanvas(false);
		}
	}

	public void UpdateItemBoolean(string type, bool achieved)
	{
		Debug.Log("Got a " + type + " and it is " + true);
		switch (type)
		{
			case "Ore":
				Ore = achieved;
				break;
			case "Brick":
				Brick = achieved;
				break;
			case "Shell":
				Shell = achieved;
				break;
			case "Gem":
				Gem = achieved;
				break;
			case "Jewel":
				Jewel = achieved;
				break;
			case "ChargedJewel":
				ChargedJewel = achieved;
				break;
		}
	}

	private void UpdateDictionary()
	{
		booleanProgress[BrickFade] = Brick;
		booleanProgress[ShellFade] = Shell;
		booleanProgress[JewelFade] = Jewel;
		booleanProgress[ChargedJewelFade] = ChargedJewel;
		booleanProgress[GolemFade] = Golem;
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
