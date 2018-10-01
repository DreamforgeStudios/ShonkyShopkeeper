using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class MoveAtTime : MonoBehaviour {
	public Vector3 EndPosition;
	public Ease Ease;
	public float StartTime;
	public float EaseTime;
	public bool UseUnscaledTime;
	
	public bool RevertOnGameEnd;
	[ShowIf("RevertOnGameEnd")]
	public float EaseEndTime;
	[ShowIf("RevertOnGameEnd")]
	public Ease EaseEnd;

	private float elapsedTime = 0;
	private Vector3 startPosition;

	void Start() {
		startPosition = transform.position;
		
		if (RevertOnGameEnd) {
			Countdown.onComplete += Reverse;
		}
	}

	private bool executed = false;
	void Update () {
		elapsedTime += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
		if (!executed && elapsedTime >= StartTime) {
			transform.DOMove(EndPosition, EaseTime).SetEase(Ease).SetUpdate(UseUnscaledTime);
			executed = true;
		}
	}

	private void Reverse() {
		Countdown.onComplete -= Reverse;
		transform.DOMove(startPosition, EaseEndTime).SetEase(EaseEnd);
	}
}
