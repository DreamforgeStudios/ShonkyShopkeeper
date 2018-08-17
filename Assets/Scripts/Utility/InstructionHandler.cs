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
	
	// TODO: proper unscaled time on tweens...

	
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI InstructionText;
	
	private bool started = false;

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
		switch (TweeningType) {
			case TweenType.Move:
				StartCoroutine(WaitForDelay(StartDelay, StartTweenMove));
				break;
			case TweenType.Punch:
				StartCoroutine(WaitForDelay(StartDelay, StartTweenPunch));
				break;
			case TweenType.Dual:
				StartCoroutine(WaitForDelay(StartDelay, StartTweenDual));
				break;
		}
		
		InstructionText.text = InstructionName;
	}

	[Button("Start Punch")]
	private void StartTweenPunch() {
		Sequence seq = DOTween.Sequence();
		Tween t1 = InstructionText.transform.DOScale(Multiplier, DurationIn).SetEase(EaseIn);
		Tween t2 = InstructionText.transform.DOScale(1, DurationOut).SetEase(EaseOut);

		seq.Append(t1);
		seq.Append(t2);
		seq.AppendInterval(Delay);
		seq.SetLoops((int) (AliveTime / (DurationIn + DurationOut)));
		seq.SetUpdate(UseUnscaledTime);
		
		if (UseUnscaledTime) {}
	}
	
	[Button("Start Move")]
	private void StartTweenMove() {
		Sequence seq = DOTween.Sequence();
		Vector3 p = InstructionText.transform.position;
		Tween t1 = InstructionText.transform.DOMove(p + Direction, DurationIn).SetEase(EaseIn);
		Tween t2 = InstructionText.transform.DOMove(p, DurationOut).SetEase(EaseOut);

		seq.Append(t1);
		seq.Append(t2);
		seq.AppendInterval(Delay);
		seq.SetLoops((int) (AliveTime / (DurationIn + DurationOut)));
		seq.SetUpdate(UseUnscaledTime);
	}

	[Button("Start Dual")]
	private void StartTweenDual() {
		Sequence seq = DOTween.Sequence();
		Vector3 p = InstructionText.transform.position;
		Tween t1 = InstructionText.transform.DOMoveY(p.y + VerticalMovePeak, VerticalMoveDurationIn).SetEase(VerticalMoveEaseIn);
		Tween t2 = InstructionText.transform.DOMoveY(p.y, VerticalMoveDurationOut).SetEase(VerticalMoveEaseOut);
		seq.Append(t1);
		seq.Append(t2);
		seq.SetLoops(VerticalMoveLoops);
		
		Sequence seq2 = DOTween.Sequence();
		Tween t3 = InstructionText.transform.DOMoveX(p.x + HorizontalMovePeak, HorizontalMoveDurationIn).SetEase(HorizontalMoveEaseIn);
		seq2.Append(seq);
		seq2.Insert(0, t3);
		seq2.SetEase(SequenceEase);

		Sequence masterseq = DOTween.Sequence();
		Tween t4 = InstructionText.transform.DOMoveX(p.x, HorizontalMoveDurationOut).SetEase(HorizontalMoveEaseOut);
		masterseq.Append(seq2);
		masterseq.Append(t4);
		masterseq.AppendInterval(Delay);
		masterseq.SetLoops((int) (AliveTime / (HorizontalMoveDurationIn + HorizontalMoveDurationOut)));
		masterseq.Play();
	}

	private float timeAlive;
	
	// Update is called once per frame
	void Update () {
		if (!started) return;

		timeAlive += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
		if (timeAlive > AliveTime) {
			// Text fading out.
			InstructionText.transform.GetComponent<RectTransform>().DOAnchorPos(ZoomOutPos, ZoomOutTime).SetEase(ZoomOutEase);
			started = false;
		}
	}

	IEnumerator WaitForDelay(float duration, Action functionCallback) {
		if (UseUnscaledTime) {
			yield return new WaitForSecondsRealtime(duration);
		} else {
			yield return new WaitForSeconds(duration);
		}

		// Text fading in.
		// TODO: This shouldn't be tied to this function.
		InstructionText.transform.GetComponent<RectTransform>().DOAnchorPos(ZoomInPos, ZoomInTime).SetEase(ZoomInEase)
			.OnComplete(() => { functionCallback(); started = true; });
	}
}
