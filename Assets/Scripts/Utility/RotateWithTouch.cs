using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithTouch : MonoBehaviour {
	public float speedMouse = 0.2f;
	public float speedTouch = 1f;

	// Update is called once per frame
	void Update () {
		// Check where we are running the program.
		RuntimePlatform p = Application.platform;
		if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.OSXPlayer)
			// Process mouse inputs.
			ProcessMouse();
		else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
			// Process touch inputs.
			ProcessTouch();
	}

	private Vector3 prevMousePos;
	private bool turning = false;
	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			prevMousePos = Input.mousePosition;
			turning = true;
		}

		if (Input.GetMouseButtonUp(0)) {
			turning = false;
		}

		if (turning) {
			Vector3 mouseDelta = Input.mousePosition - prevMousePos;
			transform.Rotate(mouseDelta.y * speedMouse, -mouseDelta.x * speedMouse, 0, Space.World);
			prevMousePos = Input.mousePosition;
		}
	}

	private void ProcessTouch() {
		if (Input.touchCount < 1) {
			return;
		}

		// Only use the first touch.
		Touch touch = Input.GetTouch(0);
		transform.Rotate(touch.deltaPosition.y * speedTouch, -touch.deltaPosition.x * speedTouch, 0, Space.World);
	}
}
