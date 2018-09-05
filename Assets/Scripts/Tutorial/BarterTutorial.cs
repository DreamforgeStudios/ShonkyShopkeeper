using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BarterTutorial : MonoBehaviour
{

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
	public GameObject particles, particleChild, gizmoPrefab;
	private bool textEnabled = false;
	public bool clickedNPC = false;
	public List<string> tutorialDialogue = new List<string>();
	private PopupTextManager clone;

	private void CheckForTutProgressChecker()
	{
		Debug.Log("Started barter tutorial manager");
		DontDestroyOnLoad(gameObject);
		GameObject obj = GameObject.FindGameObjectWithTag("TutorialProgress");
		Destroy(obj);
	}

	private void Start()
	{
		clone = Instantiate(gizmoPrefab,
			GameObject.FindGameObjectWithTag("MainCamera").transform).GetComponent<PopupTextManager>();
		clone.PopupTexts = tutorialDialogue;
		clone.Init();
		clone.DoEnterAnimation();
	}
}