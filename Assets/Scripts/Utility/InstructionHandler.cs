using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class InstructionHandler : MonoBehaviour {
	public enum TweenType {
		Punch, Move, Dual
	}
	
	// String the instruction text will be set to.
	[BoxGroup("Management Settings")]
	public String InstructionName;
	[BoxGroup("Management Settings")]
	public TweenType TweeningType;
	// Use unscaled time...
	[BoxGroup("Management Settings")]
	public bool UseUnscaledTime;
	// Delay before starting.
	[BoxGroup("Management Settings")]
	public float StartDelay;
	// Time the text should 'perform' for.
	[BoxGroup("Management Settings")]
	public int AliveTime;
	
	[BoxGroup("Management Settings")]
	public Vector3 ZoomInPos;
	// Time the text takes to fade in.
	[BoxGroup("Management Settings")]
	public float ZoomInTime;
	[BoxGroup("Management Settings")]
	public Ease ZoomInEase;
	
	[BoxGroup("Management Settings")]
	public Vector3 ZoomOutPos;
	// Time the text takes to fade out.
	[BoxGroup("Management Settings")]
	public float ZoomOutTime;
	[BoxGroup("Management Settings")]
	public Ease ZoomOutEase;
	
	

	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsMoveOrPunch")]
	public float DurationIn = .7f;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsMoveOrPunch")]
	public float DurationOut = .7f;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsMove")]
	public Vector3 Direction = Vector3.right;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsMoveOrPunch")]
	public Ease EaseIn;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsMoveOrPunch")]
	public Ease EaseOut;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsPunch")]
	public float Multiplier;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsMoveOrPunch")]
	public float Delay = 0f;
	
	
	
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public float VerticalMovePeak;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public float VerticalMoveDurationIn;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public float VerticalMoveDurationOut;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public Ease VerticalMoveEaseIn;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public Ease VerticalMoveEaseOut;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public int VerticalMoveLoops;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public float HorizontalMovePeak;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public float HorizontalMoveDurationIn;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public float HorizontalMoveDurationOut;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public Ease HorizontalMoveEaseIn;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public Ease HorizontalMoveEaseOut;
	[BoxGroup("Tween Values")]
	[ShowIf("TweenTypeIsDual")]
	public Ease SequenceEase;
	
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI InstructionText;
	
	private bool started = false;
	private Sequence activeSeq;

	// NaughtyAttributes helper functions.
	private bool TweenTypeIsMove() {
		return TweeningType == TweenType.Move;
	}

	private bool TweenTypeIsPunch() {
		return TweeningType == TweenType.Punch;
	}
	
	private bool TweenTypeIsMoveOrPunch() {
		return TweeningType == TweenType.Move || TweeningType == TweenType.Punch;
	}
	
	private bool TweenTypeIsDual() {
		return TweeningType == TweenType.Dual;
	}

	// Use this for initialization
	void Start () {
		// Initiate an empty sequence so that to avoid a possible error in PushInstruction().
		activeSeq = DOTween.Sequence();
		StartCoroutine(WaitForDelay(StartDelay, PushInstruction));
	}

	public void PushInstruction() {
		// If we've already started, simply restart the sequence.
		if (started) {
			timeAlive = 0;
			// To avoid stuttering, only restart after a loop has completed..
			activeSeq.OnStepComplete(() => activeSeq.Restart());
			//activeSeq.Restart();
			return;
		}
		
		activeSeq.Complete();
		Action functionCall;
		
		// Figure out which tween function to use.
		switch (TweeningType) {
			case TweenType.Move:
				functionCall = StartTweenMove;
				break;
			case TweenType.Punch:
				functionCall = StartTweenPunch;
				break;
			case TweenType.Dual:
				functionCall = StartTweenDual;
				break;
			default:
				functionCall = StartTweenMove;
				break;
		}
		
		InstructionText.text = InstructionName;

        // Text fading in.
        timeAlive = 0;
        transform.GetComponent<RectTransform>().DOAnchorPos(ZoomInPos, ZoomInTime).SetEase(ZoomInEase).SetUpdate(UseUnscaledTime)
            .OnComplete(() => { functionCall(); started = true; });
	}

	[Button("Start Punch")]
	private void StartTweenPunch() {
		activeSeq = DOTween.Sequence();
		Tween t1 = InstructionText.transform.DOScale(Multiplier, DurationIn).SetEase(EaseIn);
		Tween t2 = InstructionText.transform.DOScale(1, DurationOut).SetEase(EaseOut);

		activeSeq.Append(t1);
		activeSeq.Append(t2);
		activeSeq.AppendInterval(Delay);
		activeSeq.SetLoops((int) (AliveTime / (DurationIn + DurationOut)));
		activeSeq.SetUpdate(UseUnscaledTime);
	}
	
	[Button("Start Move")]
	private void StartTweenMove() {
		activeSeq = DOTween.Sequence();
		//Vector3 p = InstructionText.transform.position;
		Tween t1 = InstructionText.transform.DOLocalMove(Direction, DurationIn).SetEase(EaseIn);
		Tween t2 = InstructionText.transform.DOLocalMove(Vector3.zero, DurationOut).SetEase(EaseOut);

		activeSeq.Append(t1);
		activeSeq.Append(t2);
		activeSeq.AppendInterval(Delay);
		activeSeq.SetLoops((int) (AliveTime / (DurationIn + DurationOut)));
		activeSeq.SetUpdate(UseUnscaledTime);
	}

	[Button("Start Dual")]
	private void StartTweenDual() {
		Sequence seq = DOTween.Sequence();
		Tween t1 = InstructionText.transform.DOLocalMoveY(VerticalMovePeak, VerticalMoveDurationIn).SetEase(VerticalMoveEaseIn);
		Tween t2 = InstructionText.transform.DOLocalMoveY(0, VerticalMoveDurationOut).SetEase(VerticalMoveEaseOut);
		seq.Append(t1);
		seq.Append(t2);
		seq.SetLoops(VerticalMoveLoops);
		seq.SetUpdate(UseUnscaledTime);
		
		Sequence seq2 = DOTween.Sequence();
		Tween t3 = InstructionText.transform.DOLocalMoveX(HorizontalMovePeak, HorizontalMoveDurationIn).SetEase(HorizontalMoveEaseIn);
		seq2.Append(seq);
		seq2.Insert(0, t3);
		seq2.SetEase(SequenceEase);
		seq2.SetUpdate(UseUnscaledTime);

		activeSeq = DOTween.Sequence();
		Tween t4 = InstructionText.transform.DOLocalMoveX(0, HorizontalMoveDurationOut).SetEase(HorizontalMoveEaseOut);
		activeSeq.Append(seq2);
		activeSeq.Append(t4);
		activeSeq.AppendInterval(Delay);
		activeSeq.SetLoops((int) (AliveTime / (HorizontalMoveDurationIn + HorizontalMoveDurationOut)));
		activeSeq.SetUpdate(UseUnscaledTime);
		activeSeq.Play();
	}

	private void ZoomOut() {
        // Text fading out after the loop is complete.
		started = false;
        Tween t = transform.GetComponent<RectTransform>().DOAnchorPos(ZoomOutPos, ZoomOutTime).SetEase(ZoomOutEase)
	        .SetUpdate(UseUnscaledTime);
		activeSeq.Append(t);
	}

	// How long have we bene alive for?
	private float timeAlive;
	// Update is called once per frame
	void Update () {
		if (!started) return;

		timeAlive += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
		if (timeAlive > AliveTime) {
			ZoomOut();
		}
	}

	IEnumerator WaitForDelay(float duration, Action functionCallback) {
		if (UseUnscaledTime) {
			yield return new WaitForSecondsRealtime(duration);
		} else {
			yield return new WaitForSeconds(duration);
		}
		
		functionCallback();
	}
}
