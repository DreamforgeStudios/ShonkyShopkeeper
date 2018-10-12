using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class InstructionBubble : MonoBehaviour
{
	private Camera mainCamera;
	public LayerMask LayerMask;
	public RectTransform canvasRectTransform;
	public TextMeshProUGUI expositionTextBox, instructionTextBox;
	public bool Instruction, canvasElement;

	public int activePage;
	public List<string> informationTextToDisplay, instructionText;
	public GameObject ExpositionBubbleObj, InstructionBubbleObj, tutorialRuneObj, expositionBubblePrefab, instructionBubblePrefab, TutorialRunePrefab;
	public Vector2 instructionSecondPos;
	private RectTransform expoBubbleRectT, instrBubbleRectT;
	public Button nextButton, exitButton;
	
	//Gameobject the instruction scroll with appear next to 
	public GameObject targetObj;
	
	//Event when it goes from exposition -> instruction
	public delegate void Instruct();
	public static event Instruct onInstruction;

	private void Start()
	{
		mainCamera = Camera.main;
	}
	
	void Update() {
		// Check where we are running the program.
		RuntimePlatform p = Application.platform;
		if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.OSXPlayer)
			// Process mouse inputs.
			ProcessMouse();
		else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
			// Process touch inputs.
			ProcessTouch();
	}
	
	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask)) {
				Debug.Log("Hit" + hit.transform.name);
				if (hit.transform.CompareTag("MainButton"))
					NextText();
			}
		}
	}

	private void ProcessTouch() {
		if (Input.touchCount == 0) {
			return;
		}

		// Get the first touch, and if the touch has just started, check if it hit the buttons.
		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			RaycastHit hit;
			Ray ray = mainCamera.ScreenPointToRay(touch.position);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask)) {
				if (hit.transform.CompareTag("MainButton"))
					NextText();
			}
		}
	}

	public void Init(GameObject itemToTarget, bool CanvasElement, Canvas canvasToApply)
	{
		HideBubble();
		activePage = 0;
		canvasElement = CanvasElement;
		targetObj = itemToTarget;
		canvasRectTransform = canvasToApply.GetComponent<RectTransform>();

		//Debug.Log("Setting exposition to active");
		/*
		 * Link all variables. Really dirty right now
		 */
		ExpositionBubbleObj = Instantiate(expositionBubblePrefab, canvasToApply.transform);
		InstructionBubbleObj = Instantiate(instructionBubblePrefab, canvasToApply.transform);
		tutorialRuneObj = Instantiate(TutorialRunePrefab, canvasToApply.transform);
		nextButton = ExpositionBubbleObj.transform.GetChild(1).GetComponent<Button>();
		exitButton = ExpositionBubbleObj.transform.GetChild(2).GetComponent<Button>();
		expositionTextBox = ExpositionBubbleObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		instructionTextBox = InstructionBubbleObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		expositionTextBox.text = informationTextToDisplay[activePage];
		ExpositionBubbleObj.SetActive(true);
		InstructionBubbleObj.SetActive(false);
		tutorialRuneObj.SetActive(false);
		Instruction = false;
		nextButton.onClick.AddListener(NextText);
		//clear old events
		if (onInstruction != null)
		{
			Delegate[] clientList = onInstruction.GetInvocationList();
			foreach (var d in onInstruction.GetInvocationList())
				onInstruction -= (d as Instruct);
		}

		//Add listener
		exitButton.onClick.AddListener(delegate { ShowInstructionBubbleNextTo(instructionText); });
		
		//Set position to middle of screen
		Vector2 defaultPos = new Vector2(0.5f,0.5f);
		ExpositionBubbleObj.transform.position = Camera.main.ViewportToScreenPoint(defaultPos);
		UpdateCloser();
	}

	public void SetText(List<string> expositionText, List<string> instructionText)
	{
		informationTextToDisplay = expositionText;
		this.instructionText = instructionText;
		
	}

	public void ShowInstructionBubbleNextTo(List<string> instructions)
	{
		//Debug.Log("Showing instruction bubble and canvasElement is " + canvasElement);
		if (instructions == null)
		{
			HideBubble();
			return;
		}
		
		activePage = 0;
		Instruction = true;
		//Debug.Log(instructions.Count + " is instruction length");
		instructionTextBox.text = instructions[activePage];
		Vector2 pos = new Vector2(0f,0f);
		RectTransform rectTransform = InstructionBubbleObj.GetComponent<RectTransform>();

		//Need to manage canvas vs normal gameObjects
		if (targetObj != null)
		{
			if (canvasElement)
			{
				pos = Camera.main.ScreenToViewportPoint(targetObj.transform.position);
				//Debug.Log("pos = " + pos);
				pos = ModifyPosition(pos);
				pos = Camera.main.ViewportToScreenPoint(pos);
				InstructionBubbleObj.transform.position = pos;
			}
			else
			{
				pos = Camera.main.WorldToViewportPoint(targetObj.transform.position);
				//Debug.Log("pos = " + pos);
				pos = ModifyPosition(pos);
				//Debug.Log("Modified pos = " + pos);
				pos = Camera.main.ViewportToScreenPoint(pos);
				//Debug.Log("Final pos = " + pos);
				InstructionBubbleObj.transform.position = pos;
				//InstructionBubbleObj.GetComponent<RectTransform>().anchoredPosition = pos;
			}
			
		}
		InstructionBubbleObj.SetActive(true);
		ExpositionBubbleObj.SetActive(false);
		
		//Put tutorial indicator over item
		tutorialRuneObj.GetComponent<TutorialRuneIndicator>().SetPosition(targetObj,canvasElement);
		tutorialRuneObj.SetActive(true);
		
		OnInstruct();
	}

	public void NextInstructionText()
	{
		//Need to destroy the indicator as changing the instruction means changing its location. Right now it is easier
		//To instantiate the indicator within the inventory than through the instruction bubble
		if (tutorialRuneObj != null)
			Destroy(tutorialRuneObj);
		
		if (activePage + 1 < instructionText.Count)
		{
			instructionTextBox.text = instructionText[++activePage];
			OnInstruct();
		}
	}

	public void PreviousInstructionText()
	{
		if (activePage - 1 > 0)
		{
			instructionTextBox.text = instructionText[--activePage];
			OnInstruct();
		}
	}

	public void DestroyItem()
	{
		Instruction = false;
		if(ExpositionBubbleObj != null)
			Destroy(ExpositionBubbleObj);
		if (InstructionBubbleObj != null)
			Destroy(InstructionBubbleObj);
		if (tutorialRuneObj != null)
			Destroy(tutorialRuneObj);
		
		Destroy(this.gameObject);
	}

	public void HideBubble()
	{
		if (InstructionBubbleObj != null)
		InstructionBubbleObj.SetActive(false);
		
		if (ExpositionBubbleObj != null)
		ExpositionBubbleObj.SetActive(false);
		
		Instruction = false;
	}

	public void MoveScrollsToFront()
	{
		if (ExpositionBubbleObj != null)
			ExpositionBubbleObj.transform.SetAsLastSibling();
		
		if (InstructionBubbleObj != null)
			InstructionBubbleObj.transform.SetAsLastSibling();
	}
	
	public void NextText() {
		//Debug.Log(activePage + " is active page and textCount is " + informationTextToDisplay.Count);
		if (activePage + 1 >= informationTextToDisplay.Count) return;
		activePage++;
		expositionTextBox.text = informationTextToDisplay[activePage];
		
		UpdateCloser();
	}

	//Used to move the scroll away from the target obj, towards the centre of the screen
	private Vector2 ModifyPosition(Vector2 pos)
	{
		if (pos.x >= 0.5f)
			pos.x -= 0.35f;
		else
			pos.x += 0.35f;

		if (pos.y <= 0.5f)
			pos.y += 0.2f;
		else
			pos.y -= 0.2f;

		return pos;
	}

	public void MoveInstructionScroll()
	{
		//Make it the 'top' element
		InstructionBubbleObj.transform.SetAsLastSibling();
		
		//Move it
		RectTransform rectTransform = InstructionBubbleObj.GetComponent<RectTransform>();
		Vector2 pos = new Vector3(0.85f, 0.75f);
		pos = Camera.main.ViewportToScreenPoint(pos);
		InstructionBubbleObj.transform.DOMove(pos, 2f, false);
	}
	
	public void MoveInstructionScrollLower()
	{
		//Make it the 'top' element
		InstructionBubbleObj.transform.SetAsLastSibling();
		
		//Move it
		RectTransform rectTransform = InstructionBubbleObj.GetComponent<RectTransform>();
		Vector2 pos = new Vector3(0.85f, 0.25f);
		pos = Camera.main.ViewportToScreenPoint(pos);
		InstructionBubbleObj.transform.DOMove(pos, 2f, false);
	}

	public void MoveRuneIndicator(GameObject newObject)
	{
		tutorialRuneObj.GetComponent<TutorialRuneIndicator>().SetPosition(newObject,canvasElement);
	}
	
	// Update the closer so that if we're on the last page it can be closed.
	private void UpdateCloser() {
		if (activePage >= informationTextToDisplay.Count - 1) {
			exitButton.gameObject.SetActive(true);
			nextButton.gameObject.SetActive(false);
		} else {
			exitButton.gameObject.SetActive(false);
			nextButton.gameObject.SetActive(true);
		}
	}
	
	// Occurs when the Gizmo has left the scene (just before being destroyed).
	public void OnInstruct() {
		if (onInstruction != null)
			onInstruction();
	}
}
