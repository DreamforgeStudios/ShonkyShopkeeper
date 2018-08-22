using System.Collections;
using System.Collections.Generic;
using TMPro;
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
			Destroy(this.gameObject);

	}

	//MAP TUTORIAL START

	public GameObject textObj, shopButtonObj, sphere, particles, particleChild;
	public TextMeshProUGUI textBox;
	public int currentTextNumber;
	private bool textEnabled = false;
	public bool clickedOrb, CanMoveCamera  = false;
	public List<string> tutorialDialogue = new List<string>();

	private void CheckForTutProgressChecker()
	{
		DontDestroyOnLoad(gameObject);
		GameObject obj = GameObject.FindGameObjectWithTag("TutorialProgress");
		Destroy(obj);
		currentTextNumber = 0;
		shopButtonObj.SetActive(false);
		ShowCanvas();
	}

	private void Update()
	{
		if (textEnabled && Input.GetMouseButtonDown(0) && currentTextNumber > 3)
		{
			DetermineTutorialProgress();
		}
	}

	private void DetermineTutorialProgress()
	{
		if (currentTextNumber == 4)
		{
			ShowCanvas();
		} else if (currentTextNumber == 5 && clickedOrb)
		{
			ShowCanvas();
		} else if (currentTextNumber == 6 && clickedOrb)
		{
			ShowCanvas();
		}
	}

	private void ShowCanvas()
	{
		textObj.SetActive(true);
		DetermineText();
	}

	private void HideCanvas()
	{
		textObj.SetActive(false);
	}

	private void DetermineText()
	{
		OnlyShowTextBox(tutorialDialogue[currentTextNumber]);
	}

	private IEnumerator FadeCanvas()
	{
		yield return new WaitForSeconds(3);
		if (currentTextNumber < 4)
		{
			currentTextNumber++;
			if (currentTextNumber == 4)
			{
				CanMoveCamera = true;
				StartSphereParticles();
			}

			DetermineText();
			yield return new WaitForSeconds(3f);
		}
		else
		{
			textEnabled = false;
			textObj.SetActive(false);
			if (currentTextNumber == 5)
			{
				currentTextNumber++;
				DetermineText();
			}
		}
	}

	public void ClickedSphere()
	{
		StopSphereParticle();
		clickedOrb = true;
		currentTextNumber++;
		DetermineText();
	}

	public void OnlyShowTextBox(string text)
	{
		textObj.SetActive(true);
		textEnabled = true;
		textBox.text = text;
		StartCoroutine(FadeCanvas());
	}
	
	private void StartSphereParticles()
	{
		particleChild = Instantiate(particles, sphere.transform.position, sphere.transform.rotation);
		particleChild.transform.parent = sphere.transform;
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

}
