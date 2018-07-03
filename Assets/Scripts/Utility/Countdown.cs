using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Countdown : MonoBehaviour {
	public TextMeshProUGUI textTimer; 

	public float startTime;

	private float currentTimeRemaining;

	public delegate void OnTick();
	public static event OnTick onTick;

	public delegate void OnComplete();
	public static event OnComplete onComplete;

	private bool complete;

	// Use this for initialization
	void Start () {
		currentTimeRemaining = startTime;
		complete = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (complete) {
			return;
		}

		// Check if there's been a "tick" (whole number changeover).
		float newTime = currentTimeRemaining - Time.deltaTime;
		if (Mathf.Ceil(newTime) != Mathf.Ceil(currentTimeRemaining)) {
			OnTimerTick();
		}
		currentTimeRemaining = newTime;

		if (currentTimeRemaining < 0) {
			OnCompleteTick();
			currentTimeRemaining = 0;
			complete = true;
		}

		UpdateTimerText();
	}

	private void UpdateTimerText() {
		textTimer.text = Mathf.Ceil(currentTimeRemaining).ToString("N0");
	}

	private void OnTimerTick() {
		transform.DOPunchRotation(Vector3.forward * 25, 0.4f, 18);
		if (onTick != null)
			onTick();
	}

	private void OnCompleteTick() {
		if (onComplete != null) {
			onComplete();
		}
	}
}