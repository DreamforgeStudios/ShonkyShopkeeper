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
	public GameObject ExpositionBubbleObj, InstructionBubbleObj;
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
		expoBubbleRectT = ExpositionBubbleObj.GetComponent<RectTransform>();
		//ExpositionBubbleObj.gameObject.SetActive(false);
		//InstructionBubbleObj.gameObject.SetActive(false);
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

	public void Init(GameObject itemToTarget, bool CanvasElement)
	{
		HideBubble();
		activePage = 0;
		canvasElement = CanvasElement;
		targetObj = itemToTarget;

		expositionTextBox.text = informationTextToDisplay[activePage];
		UpdateCloser();
		Debug.Log("Setting exposition to active");
		ExpositionBubbleObj.SetActive(true);
		InstructionBubbleObj.SetActive(false);
		Instruction = false;
	}

	public void SetText(List<string> expositionText, List<string> instructionText)
	{
		informationTextToDisplay = expositionText;
		this.instructionText = instructionText;
	}

	public void ShowInstructionBubbleNextTo()
	{
		Debug.Log("Showing instruction bubble and canvasElement is " + canvasElement);
		if (instructionText == null)
		{
			HideBubble();
			return;
		}
		
		activePage = 0;
		Instruction = true;
		instructionTextBox.text = instructionText[activePage];
		Vector2 pos = new Vector2(0f,0f);

		//Need to manage canvas vs normal gameObjects
		if (targetObj != null)
		{
			if (canvasElement)
			{
				pos = targetObj.transform.position;
				Debug.Log("pos = " + pos);
				pos = ModifyPosition(pos);
			}
			else
			{
				pos = Camera.main.WorldToScreenPoint(targetObj.transform.position);
				Debug.Log("pos = " + pos);
				pos = ModifyPosition(pos);
			}
			InstructionBubbleObj.transform.position = pos;
		}

		//InstructionBubbleObj.GetComponent<RectTransform>().anchoredPosition = pos;
		
		InstructionBubbleObj.SetActive(true);
		ExpositionBubbleObj.SetActive(false);
		OnInstruct();
	}

	public void NextInstructionText()
	{
		if (activePage + 1 < instructionText.Count)
		{
			instructionTextBox.text = instructionText[++activePage];
			OnInstruct();
		}
	}

	public void DestroyItem()
	{
		Instruction = false;
		Destroy(this.gameObject);
	}

	public void HideBubble()
	{
		InstructionBubbleObj.SetActive(false);
		ExpositionBubbleObj.SetActive(false);
		Instruction = false;
	}
	
	public void NextText() {
		Debug.Log(activePage + " is active page and textCount is " + informationTextToDisplay.Count);
		if (activePage + 1 >= informationTextToDisplay.Count) return;
		activePage++;
		expositionTextBox.text = informationTextToDisplay[activePage];
		
		UpdateCloser();
	}

	//Used to move the scroll away from the target obj, towards the centre of the screen
	private Vector2 ModifyPosition(Vector2 pos)
	{
		if (pos.x >= 230f)
			pos.x -= 160f;
		else
			pos.x += 200f;

		if (pos.y <= 150f)
			pos.y += 100f;
		else
			pos.y -= 100f;

		return pos;
	}

	public void MoveInstructionScroll()
	{
		InstructionBubbleObj.transform.DOMove(instructionSecondPos, 2f, false);
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
