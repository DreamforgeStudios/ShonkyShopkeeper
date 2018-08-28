using UnityEngine;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

public class Countdown : MonoBehaviour {
	public TextMeshProUGUI TextTimer; 
	public float StartTime;
	
	[ReadOnly]
	public float CurrentTimeRemaining;

	public delegate void OnTick();
	public static event OnTick onTick;

	public delegate void OnComplete();
	public static event OnComplete onComplete;

	private bool complete;

	// Use this for initialization
	void Start () {
		CurrentTimeRemaining = StartTime;
		complete = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (complete) {
			return;
		}

		// Check if there's been a "tick" (whole number changeover).
		float newTime = CurrentTimeRemaining - Time.deltaTime;
		if (Mathf.Ceil(newTime) != Mathf.Ceil(CurrentTimeRemaining)) {
			OnTimerTick();
		}
		
		CurrentTimeRemaining = newTime;

		if (CurrentTimeRemaining < 0) {
			OnCompleteTick();
			CurrentTimeRemaining = 0;
			complete = true;
		}

		UpdateTimerText();
	}
	
	private void UpdateTimerText() {
		TextTimer.text = Mathf.Ceil(CurrentTimeRemaining).ToString("N0");
	}
	
	private void OnTimerTick() {
        //SFX.Play("sound");
        SFX.Play("Mini_Game_timer", 1f, 1f, 0f, false, 0f);
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