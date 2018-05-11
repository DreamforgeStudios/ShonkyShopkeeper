using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening; // Tweening / nice lerping.

// The toolbox class is how all user inventory interactions should be done.
public class Toolbox : MonoBehaviour {
	public enum Tool {
		Inspector,
		Foreceps,
		Wand
	}

	// A layer mask so that we only hit slots.
	public LayerMask layerMask;

	// Variables for the inspector.
	public GameObject inspectionPanel;
	public TextMeshProUGUI textHeading;
	public TextMeshProUGUI textInfo;

	// Helpers.
	private Tool currentTool;
	private Slot currentSelection = null;

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
				PickAndUseTool(hit.transform.GetComponent<Slot>());
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
		
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
					PickAndUseTool(hit.transform.GetComponent<Slot>());
				} else {
					// If there was no hit and we're using the inspector, then we should hide the inspector.
					if (currentTool == Tool.Inspector) {
						HideInspector();
					}
				}
			}
		}
	}

	// Use the right tool on the slot.
	private void PickAndUseTool(Slot slot) {
		switch (currentTool) {
			case Tool.Inspector:
				UseInspector(slot);
				break;
			case Tool.Foreceps:
				UseForceps(slot);
				break;
			case Tool.Wand:
				UseWand(slot);
				break;
		}
	}

	// Inspect an item.
	private void UseInspector(Slot slot) {
		// Can't select 2 items at once.
		if (currentSelection) {
			// Maybe this will cause flickering, might be better to just hide the object.
			HideInspector();
			currentSelection = null;
		}

		// To avoid null errors, always use the x.Get() methods, they check for you.
		Item item;
		if (slot.GetItem(out item)) {
			this.currentSelection = slot;
			inspectionPanel.SetActive(true);

			textHeading.text = item.GetItemName();
			textInfo.text = item.GetItemInfo();

			// Animate using tween library -> see https://easings.net/ for some animaions to use.
			GameObject itemObj;
			if (slot.GetPrefabInstance(out itemObj)) {
				Transform t = itemObj.transform;
				t.DOMove(t.position + (Vector3.up), 0.7f).SetEase(Ease.OutBack);
			}
		}
	}

	// Hide the inspector and the current item (if one is selected).
	private void HideInspector() {
		inspectionPanel.SetActive(false);
		GameObject gameObj;
		if (currentSelection && currentSelection.GetPrefabInstance(out gameObj)) {
			gameObj.transform.DOMove(currentSelection.transform.position, 1f).SetEase(Ease.OutBounce);
			currentSelection = null;
		}
	}

	private void UseForceps(Slot slot) {
		// Not implemented.
	}

	private void UseWand(Slot slot) {
		// Not implemented.
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawRay(previousRay);
	}
}
