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
			CanMoveCamera = true;
			Destroy(this.gameObject);
		}
	}

	//MAP TUTORIAL START

	public GameObject shopButtonObj, sphere, particles, particleChild, speechBubblePrefab, introTarget;
	public bool clickedOrb, CanMoveCamera  = false;
	public List<string> intro, introInstructions, map, mapInstructions;
	private InstructionBubble clone;
	public Canvas mainCanvas;


	private void Start()
	{
		StartDialogue(intro,introInstructions, mainCanvas, null,false);
	}
	
	private void CheckForTutProgressChecker()
	{
		DontDestroyOnLoad(gameObject);
		GameObject obj = GameObject.FindGameObjectWithTag("TutorialProgress");
		Destroy(obj);
		shopButtonObj.SetActive(false);
	}
	
	public void StartDialogue(List<string> dialogue, List<string> instruction, Canvas canvas, GameObject target, bool canvasElement)
	{	
		if (clone != null)
			clone.DestroyItem();
		
		clone = Instantiate(speechBubblePrefab, mainCanvas.transform)
			.GetComponentInChildren<InstructionBubble>();
		clone.SetText(dialogue,instruction);
		clone.Init(target,canvasElement,canvas);

	}
	
	private void Update()
	{
		
		if (!CanMoveCamera && clone.Instruction)
		{			
			InstructionBubble.onInstruction += StartSphereParticles();
		}
	}

	public void ClickedSphere()
	{
		CanMoveCamera = true;
		StopSphereParticle();
		clickedOrb = true;
		StartDialogue(map, mapInstructions, mainCanvas, sphere,false);
	}
	
	private InstructionBubble.Instruct StartSphereParticles()
	{
		Debug.Log("starting sphere particles");
		CanMoveCamera = true;
		particleChild = Instantiate(particles, sphere.transform.position, sphere.transform.rotation);
		particleChild.transform.parent = sphere.transform;
		return () => { InstructionBubble.onInstruction -= StartSphereParticles(); };
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
