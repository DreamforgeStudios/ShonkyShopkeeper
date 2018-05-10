using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Toolbox : MonoBehaviour {
	public enum Tool {
		Inspector,
		Foreceps,
		Wand
	}

	public GameObject inspectionPanel;
	public TextMeshProUGUI textHeading;
	public TextMeshProUGUI textInfo;

	public LayerMask layerMask;

	private Tool currentTool;

	// Debug.
	private Ray previousRay;


	// Use this for initialization
	void Start () {
		// TODO: Change this to the default tool.
		currentTool = Tool.Inspector;
	}
	
	// Update is called once per frame
	void Update () {
		// Check where we are running the program.
		RuntimePlatform p = Application.platform;
		if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer)
			// Process mouse inputs.
			ProcessMouse();
		else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
			// Process touch inputs.
			ProcessTouch();
	}

	public void SwitchTool(Tool tool) {
		currentTool = tool;
	}

	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			previousRay = ray;
		
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
				Debug.Log("Hit: " + hit.transform.name);
				PickAndUseTool(hit.transform.GetComponent<Item>());
			} else {
				if (currentTool == Tool.Inspector) {
					HideInspector();
				}
			}
		}
	}
	
	private void ProcessTouch() {
		if (Input.touchCount == 0) {
			return;
		}

		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				previousRay = ray;
		
				// TODO: use an items layermask so that we only hit items, not drawers or other models.
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
					PickAndUseTool(hit.transform.GetComponent<Item>());
				} else {
					if (currentTool == Tool.Inspector) {
						HideInspector();
					}
				}
			}
		}
	}

	private void PickAndUseTool(Item item) {
		switch (currentTool) {
			case Tool.Inspector:
				UseInspector(item);
				break;
			case Tool.Foreceps:
				UseForceps(item);
				break;
			case Tool.Wand:
				UseWand(item);
				break;
		}
	}

	private void UseInspector(Item item) {
		inspectionPanel.SetActive(true);
		//textHeading.text = item.ItemName();
		//textInfo.text = item.ItemInfo();

		// TODO: bring item up to camera.
	}

	private void HideInspector() {
		inspectionPanel.SetActive(false);
	}

	private void UseForceps(Item item) {
		// Not implemented.
	}

	private void UseWand(Item item) {
		// Not implemented.
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawRay(previousRay);
	}
}
