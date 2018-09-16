using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstructionBubble : MonoBehaviour
{
	private Camera mainCamera;
	public LayerMask LayerMask;
	public RectTransform canvasRectTransform;
	public TextMeshProUGUI expositionTextBox, instructionTextBox;
	public bool Instruction, canvasElement = true;

	public int activePage;
	public List<string> textToDisplay;
	public GameObject ExpositionBubbleObj, InstructionBubbleObj;
	private RectTransform expoBubbleRectT, instrBubbleRectT;
	public Button nextButton, exitButton;
	
	//Gameobject the instruction scroll with appear next to 
	private GameObject targetObj;
	
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
		if (textToDisplay.Count > 1)
		{
			expositionTextBox.text = textToDisplay[activePage];
			UpdateCloser();
			Debug.Log("Setting exposition to active");
			ExpositionBubbleObj.SetActive(true);
			InstructionBubbleObj.SetActive(false);
			Instruction = false;
		}
		else
		{
			//Dirty way right now to handle single item lists
			activePage--;
			ShowInstructionBubbleNextTo();

		}
	}

	public void ShowInstructionBubbleNextTo()
	{
		Debug.Log("Showing instruction bubble and canvasElement is " + canvasElement);
		Instruction = true;
		instructionTextBox.text = textToDisplay[++activePage];
		Vector2 pos = new Vector2(0f,0f);
		
		//Need to manage canvas vs normal gameObjects
		if (canvasElement)
		{
			pos = targetObj.transform.position;
			Debug.Log("pos = " + pos);
			pos = ModifyPosition(pos);
			
		}
		else
		{
			pos = Camera.main.WorldToViewportPoint(targetObj.transform.position);
			Debug.Log("pos = " + pos);
			pos = ModifyPosition(pos);
			
		}

		InstructionBubbleObj.GetComponent<RectTransform>().anchoredPosition = pos;
		InstructionBubbleObj.transform.position = pos;
		InstructionBubbleObj.SetActive(true);
		ExpositionBubbleObj.SetActive(false);
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
		Debug.Log(activePage + " is active page and textCount is " + textToDisplay.Count);
		if (activePage + 1 >= textToDisplay.Count) return;
		activePage++;
		expositionTextBox.text = textToDisplay[activePage];
		
		UpdateCloser();
	}

	//Used to move the scroll away from the target obj, towards the centre of the screen
	private Vector2 ModifyPosition(Vector2 pos)
	{
		if (pos.x >= 400f)
			pos.x -= 100f;
		else
			pos.x += 100f;

		if (pos.y <= 230f)
			pos.y += 100f;
		else
			pos.y -= 100f;

		return pos;
	}
	
	// Update the closer so that if we're on the last page it can be closed.
	private void UpdateCloser() {
		if (activePage >= textToDisplay.Count - 2) {
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
