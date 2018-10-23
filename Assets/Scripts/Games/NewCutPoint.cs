using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.UI;

// Cut indicators tick down as soon as they are created.
// TODO: for polish, make this transition non-linearly.
public class NewCutPoint : MonoBehaviour {
	/* NEW PROPERTIES */
	[BoxGroup("Object Assignments")]
	public SpriteRenderer SpinnerSpriteRenderer,
						  LineSpriteRenderer;
	[BoxGroup("Object Assignments")]
	public SpriteMask LineSpriteMask;

	[BoxGroup("Spinner")]
	public int NumberOfSpins;
	[BoxGroup("Spinner")]
	public float RotationSpeed;
	[BoxGroup("Spinner")]
	public Ease RotationEase;
	[BoxGroup("Spinner")]
	public bool EasePerLoop;
	[BoxGroup("Spinner")]
	public Vector3 SpinnerSelectedScale;
	[BoxGroup("Spinner")]
	public Color SpinnerSelectedColor;
	[BoxGroup("Spinner")]
	public Ease SpinnerSelectedEase;
	[BoxGroup("Spinner")]
	public float SpinnerSelectedEaseTime;

	[BoxGroup("Line")]
	public float LineAppearAtTime;
	[BoxGroup("Line")]
	public float MaskWipeTime;
	[BoxGroup("Line")]
	public Ease MaskWipeEase;
	[BoxGroup("Spinner")]
	public Vector3 LineSelectedScale;
	[BoxGroup("Spinner")]
	public Color LineSelectedColor;
	[BoxGroup("Spinner")]
	public Ease LineSelectedEase;
	[BoxGroup("Spinner")]
	public float LineSelectedEaseTime;

	[BoxGroup("Fade")]
	public float FadeDuration;
	[BoxGroup("Fade")]
	public Ease FadeEase;

	[BoxGroup("Properties")]
	public Vector3 CutVector;
	[BoxGroup("Properties")]
	public float CutTimeout;

	public float SpawnTime {
		get { return LineAppearAtTime; }
	}
	
	public delegate void OnSpawnComplete(NewCutPoint cut);
	public event OnSpawnComplete onSpawnComplete;

	public delegate void OnTimeoutComplete(NewCutPoint cut);
	public event OnTimeoutComplete onTimeoutComplete;

	private Vector3 spinnerOriginalScale,
				  lineOriginalScale;
	private Color spinnerOriginalColor,
				  lineOriginalColor;
	private Sequence animationSeq;
	
	void Start() {
		// NOTE: we assume that all axis' are scaled the same.
		spinnerOriginalScale = SpinnerSpriteRenderer.transform.localScale;
		spinnerOriginalColor = SpinnerSpriteRenderer.color;
		lineOriginalScale = LineSpriteRenderer.transform.localScale;
		lineOriginalColor = LineSpriteRenderer.color;
		
		Initialize();
		animationSeq = RunAnimation();
	}

	[Button("Reset Values")]
	private void Initialize() {
		// These variables have the effect of hiding the cut.
		LineSpriteMask.alphaCutoff = 1;
		SpinnerSpriteRenderer.transform.localScale = Vector3.zero;
	}

	[Button("Run Animation")]
	private Sequence RunAnimation() {
		AlignWithCutVector();
		
		// Spawning instantly now.
		OnSpawnCompleteTick();
		
		var seq = DOTween.Sequence();
		var spinnerseq = DOTween.Sequence();
        spinnerseq.Append(SpinnerSpriteRenderer.transform.DOLocalRotate(Vector3.forward * 360, RotationSpeed, RotateMode.LocalAxisAdd)
            .SetEase(EasePerLoop ? RotationEase : Ease.Linear).SetLoops(NumberOfSpins));
		spinnerseq.SetEase(EasePerLoop ? Ease.Linear : RotationEase);
		
		seq.Append(spinnerseq);
		// TODO: make this use parameters.
		seq.Insert(0, SpinnerSpriteRenderer.transform.DOScale(1, .5f).SetEase(Ease.OutBack));
		
		// A bit messy, but seems necessary for this kind of approach.
		Tween drawLine =  DOTween.To(() => LineSpriteMask.alphaCutoff, x => LineSpriteMask.alphaCutoff = x, 0, MaskWipeTime)
			.SetEase(MaskWipeEase).OnComplete(() => {
				LineSpriteMask.enabled = false;
				LineSpriteRenderer.maskInteraction = SpriteMaskInteraction.None;
			});
		seq.Insert(LineAppearAtTime, drawLine);

		seq.Insert(CutTimeout, SpinnerSpriteRenderer.DOFade(0, FadeDuration));
		seq.Insert(CutTimeout, LineSpriteRenderer.DOFade(0, FadeDuration));
		seq.InsertCallback(CutTimeout + FadeDuration, OnTimeoutTick);
		
		seq.SetEase(EasePerLoop ? Ease.Linear : RotationEase);
		seq.Play();

		return seq;
		// TODO: change color as the circle "warms up"?
	}

	[Button("Align With Vector")]
	private void AlignWithCutVector() {
		// Rotate the line to face the direction of the cut.
		float angle = Utility.CalculateAngle(transform.right, CutVector);
		transform.Rotate(Vector3.forward, angle);
	}

	[Button("Draw Line")]
	private void DrawLine() {
		DOTween.To(() => LineSpriteMask.alphaCutoff, x => LineSpriteMask.alphaCutoff = x, 0, MaskWipeTime)
			.SetEase(MaskWipeEase);
	}
	
	[Button("Set Selected")]
	public void SetSelected() {
		if (animationSeq.Elapsed() < LineAppearAtTime)
			animationSeq.Goto(LineAppearAtTime, true);
		else if (animationSeq.Elapsed() > LineAppearAtTime + MaskWipeTime) {
			animationSeq.Goto(LineAppearAtTime + MaskWipeTime);
			if (!animationSeq.IsPlaying()) {
				animationSeq.Play();
			}
		}
		
		SpinnerSpriteRenderer.transform.DOScale(SpinnerSelectedScale, SpinnerSelectedEaseTime)
			.SetEase(SpinnerSelectedEase);
		SpinnerSpriteRenderer.DOColor(SpinnerSelectedColor, SpinnerSelectedEaseTime).SetEase(SpinnerSelectedEase);
		
		LineSpriteRenderer.transform.DOScale(LineSelectedScale, LineSelectedEaseTime)
			.SetEase(SpinnerSelectedEase);
		LineSpriteRenderer.DOColor(LineSelectedColor, LineSelectedEaseTime).SetEase(LineSelectedEase);

		SpinnerSpriteRenderer.sortingOrder = 1;
		LineSpriteRenderer.sortingOrder = 1;
	}
	
	[Button("Unset Selected")]
	public void UnsetSelected() {
		SpinnerSpriteRenderer.transform.DOScale(spinnerOriginalScale, SpinnerSelectedEaseTime)
			.SetEase(SpinnerSelectedEase);
		SpinnerSpriteRenderer.DOColor(spinnerOriginalColor, SpinnerSelectedEaseTime).SetEase(SpinnerSelectedEase);
		
		LineSpriteRenderer.transform.DOScale(lineOriginalScale, LineSelectedEaseTime)
			.SetEase(SpinnerSelectedEase);
		LineSpriteRenderer.DOColor(lineOriginalColor, LineSelectedEaseTime).SetEase(LineSelectedEase);
		
		SpinnerSpriteRenderer.sortingOrder = 0;
		LineSpriteRenderer.sortingOrder = 0;
	}

	private void OnSpawnCompleteTick() {
		if (onSpawnComplete != null) {
			onSpawnComplete(this);
		}
	}

	private void OnTimeoutTick() {
		if (onTimeoutComplete != null) {
			onTimeoutComplete(this);
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(CutVector, 1f);
	}
}
