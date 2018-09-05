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
		{
			//HideCanvas();
			CanMoveCamera = true;
			//Destroy(this.gameObject);
		}
	}

	//MAP TUTORIAL START

	public GameObject shopButtonObj, sphere, particles, particleChild, gizmoPrefab;
	public bool clickedOrb, CanMoveCamera  = false;
	public List<string> intro, map;
	private PopupTextManager clone;


	private void Start()
	{
		StartDialogue(intro);
	}
	
	private void CheckForTutProgressChecker()
	{
		DontDestroyOnLoad(gameObject);
		GameObject obj = GameObject.FindGameObjectWithTag("TutorialProgress");
		Destroy(obj);
		shopButtonObj.SetActive(false);
	}
	
	public void StartDialogue(List<string> dialogue)
	{
        clone = Instantiate(gizmoPrefab, GameObject.FindGameObjectWithTag("MainCamera").transform)
	        .GetComponentInChildren<PopupTextManager>();
		clone.PopupTexts = dialogue;
		clone.Init();
		clone.DoEnterAnimation();
	}

	/*
	public void StartTinyDialogue(List<string> dialogue)
	{
		gizmoTiny.SetActive(true);
		clone = gizmoTiny.GetComponent<PopupTextManager>();
		clone.PopupTexts = dialogue;
		clone.Init();
		clone.EnterModified();
	}
	*/
	
	private void Update()
	{
		if (clone.closed && !CanMoveCamera)
		{
			CanMoveCamera = true;
			StartSphereParticles();
		}
	}

	public void ClickedSphere()
	{
		StopSphereParticle();
		clickedOrb = true;
		StartDialogue(map);
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
