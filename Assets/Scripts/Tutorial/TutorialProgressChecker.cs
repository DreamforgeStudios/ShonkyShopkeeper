using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
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
					var container = new GameObject("GameManager");
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
	}

	private void SetupManager() {
		DontDestroyOnLoad(gameObject);
		Ore = true;
		Brick = true;
		booleanProgress = new Dictionary<bool,Image>()
		{
			{Ore,Top},
			{Gem,Bottom},
			{Brick, BrickFade},
			{Jewel, JewelFade},
			{Shell, ShellFade},
			{ChargedJewel, ChargedJewelFade},
			{Golem,GolemFade}
		};
	}
	
	//Manager start
	public bool Ore, Brick, Shell, Gem, Jewel, ChargedJewel, Golem = false;
	public Canvas Progress;
	public Image Top, Bottom, GolemImg, BrickFade, ShellFade, JewelFade, ChargedJewelFade, GolemFade;
	private Dictionary<bool,Image> booleanProgress;

	public void ShowCanvas()
	{
		Progress.enabled = true;
		UpdateCanvas();
	}

	public void HideCanvas()
	{
		Progress.enabled = false;
	}

	private void UpdateCanvas()
	{
		foreach (KeyValuePair<bool,Image> entry in booleanProgress)
		{
			
		}
	}

	public void UpdateItemBoolean(string type, bool achieved)
	{
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
}
