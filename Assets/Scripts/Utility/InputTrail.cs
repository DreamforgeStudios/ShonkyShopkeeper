using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTrail : MonoBehaviour {
	private TrailRenderer trailRenderer;
	private bool follow = false;

	void Start() {
		trailRenderer = GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		RuntimePlatform p = Application.platform;
		if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.OSXPlayer)
			// Process mouse inputs.
			ProcessMouse();
		else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
			// Process touch inputs.
			ProcessTouch();
	}

	private void ProcessTouch() {
		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			trailRenderer.emitting = false;
			transform.position = Utility.ConvertToWorldPoint(touch.position);
		} else if (touch.phase == TouchPhase.Moved) {
			trailRenderer.emitting = true;
			transform.position = Utility.ConvertToWorldPoint(touch.position);
		} else if (touch.phase == TouchPhase.Ended) {
			trailRenderer.emitting = false;
		}
	}

	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			trailRenderer.emitting = false;
			transform.position = Utility.ConvertToWorldPoint(Input.mousePosition);
		} else if (Input.GetMouseButton(0)) {
			trailRenderer.emitting = true;
			transform.position = Utility.ConvertToWorldPoint(Input.mousePosition);
		} else if (Input.GetMouseButtonUp(0)) {
			trailRenderer.emitting = false;
		}
	}
}
