using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// The inspector is basically just the magnifying glass detached from the toolbox.
public class Inspector : MonoBehaviour {
	// For debug.
	private Ray previousRay;

	public GameObject inspectionPanel;
	public TextMeshProUGUI textHeading;
	public TextMeshProUGUI textInfo;

	// TODO: could use layermask for more efficiency / (better?) raycasts.

	// Use this for initialization
	void Start () {
		
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

	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			previousRay = ray;
		
			// Get shonky and show inspector if we hit a shony, otherwise hide it.
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				Shonky shonky = hit.transform.GetComponent<Shonky>();
				if (shonky) {
					ShowInspectionMenu(shonky);
				}
			} else {
				HideInspectionMenu();
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
		
				// Get shonky and show inspector if we hit a shony, otherwise hide it.
				if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
					Shonky shonky = hit.transform.GetComponent<Shonky>();
					if (shonky) {
						ShowInspectionMenu(shonky);
					}
				} else {
					HideInspectionMenu();
				}
			}
		}
	}

	private void ShowInspectionMenu(Shonky shonky) {
		inspectionPanel.SetActive(true);
		textHeading.text = shonky.ItemName();
		textInfo.text = shonky.ItemInfo();
	}

	private void HideInspectionMenu() {
		inspectionPanel.SetActive(false);
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawRay(previousRay);
	}
}
