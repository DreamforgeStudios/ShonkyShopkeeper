using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BarterTutorial : MonoBehaviour {

	private static BarterTutorial _instance;

	// Lazy instantiation of GameManager.
	// Means that we don't have to manually place GameManager in each scene.
	// Not sure if this is the best way to do this yet...
	public static BarterTutorial Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<BarterTutorial>();

				if (_instance == null)
				{
					var container = new GameObject("TutorialManager");
					_instance = container.AddComponent<BarterTutorial>();
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

		if (GameManager.Instance.BarterTutorial)
			CheckForTutProgressChecker();
		else
			Destroy(this.gameObject);
	}
	
	//START BARTER TUTORIAL
	public GameObject textObj, particles, particleChild;
	public TextMeshProUGUI textBox;
	public int currentTextNumber;
	private bool textEnabled = false;
	public bool clickedNPC  = false;
	public List<string> tutorialDialogue = new List<string>();
	
	private void CheckForTutProgressChecker()
	{
		Debug.Log("Started barter tutorial manager");
		DontDestroyOnLoad(gameObject);
		GameObject obj = GameObject.FindGameObjectWithTag("TutorialProgress");
		Destroy(obj);
		currentTextNumber = 0;
		ShowCanvas();
	}
	
	private void ShowCanvas()
	{
		textObj.SetActive(true);
		DetermineText();
	}
	
	private void DetermineText()
	{
		OnlyShowTextBox(tutorialDialogue[currentTextNumber]);
	}
	
	private IEnumerator FadeCanvas()
	{
		yield return new WaitForSeconds(3);
		if (currentTextNumber == 0)
		{
			currentTextNumber++;
			DetermineText();
			yield return new WaitForSeconds(3);
		}
		textEnabled = false;
		textObj.SetActive(false);
		
	}
	
	public void OnlyShowTextBox(string text)
	{
		textObj.SetActive(true);
		textEnabled = true;
		textBox.text = text;
		StartCoroutine(FadeCanvas());
	}
}
