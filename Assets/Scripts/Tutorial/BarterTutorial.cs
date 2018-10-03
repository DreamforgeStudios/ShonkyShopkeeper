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
	public GameObject particles, particleChild, speechBubblePrefab, dialogueTarget;
	public Canvas mainCanvas;
	private bool textEnabled = false;
	public bool clickedNPC = false;
	public List<string> tutorialDialogue, tutorialInstructions;
	private InstructionBubble clone;
	public PhysicalShonkyInventory shonkyInv;

	private void CheckForTutProgressChecker()
	{
		Debug.Log("Started barter tutorial manager");
		DontDestroyOnLoad(gameObject);
		GameObject obj = GameObject.FindGameObjectWithTag("TutorialProgress");
		Destroy(obj);
	}

	private void Start()
	{
		GameManager.Instance.BarterNPC = true;
        StartDialogue(tutorialDialogue, tutorialInstructions, mainCanvas,dialogueTarget, true);
	}
	
	public void StartDialogue(List<string> dialogue, List<string> instruction, Canvas canvas, GameObject target, bool canvasElement)
	{	
		if (clone != null)
			clone.DestroyItem();
		
		clone = Instantiate(speechBubblePrefab, mainCanvas.transform)
			.GetComponentInChildren<InstructionBubble>();
		clone.SetText(dialogue,instruction);
		clone.Init(target,canvasElement,canvas);
		clone.MoveScrollsToFront();
	}
	
	public void NextInstruction()
	{
		clone.NextInstructionText();
	}

	public void PreviousInstruction()
	{
		clone.PreviousInstructionText();
	}

	public void StartShonkyParticles()
	{
		GameManager.Instance.introducedNPC = true;
		shonkyInv.HighlightGolems();
	}

	public void RemoveShonkyParticles()
	{
		shonkyInv.RemoveParticles();
	}
}