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

	[BoxGroup("Properties")]
	public Vector3 CutVector;

	public float SpawnTime {
		get { return LineAppearAtTime; }
	}
	
	public delegate void OnSpawnComplete(NewCutPoint cut);
	public event OnSpawnComplete onSpawnComplete;

	private Vector3 spinnerOriginalScale,
				  lineOriginalScale;
	private Color spinnerOriginalColor,
				  lineOriginalColor;
	// Use this for initialization
	void Awake () {
	}

	void Start() {
		// NOTE: we assume that all axis' are scaled the same.
		spinnerOriginalScale = SpinnerSpriteRenderer.transform.localScale;
		spinnerOriginalColor = SpinnerSpriteRenderer.color;
		lineOriginalScale = LineSpriteRenderer.transform.localScale;
		lineOriginalColor = LineSpriteRenderer.color;
		
		Initialize();
		RunAnimation();
	}

	[Button("Reset Values")]
	private void Initialize() {
		// These variables have the effect of hiding the cut.
		LineSpriteMask.alphaCutoff = 1;
		SpinnerSpriteRenderer.transform.localScale = Vector3.zero;
	}

	[Button("Run Animation")]
	private void RunAnimation() {
		AlignWithCutVector();
		
		var seq = DOTween.Sequence();
		/*
		for (int i = 0; i < NumberOfSpins; i++) {
            seq.Append(CircleSpriteRenderer.transform.DOLocalRotate(Vector3.forward * 360, RotationSpeed, RotateMode.LocalAxisAdd)
                .SetEase(EasePerLoop ? RotationEase : Ease.Linear));
		}
		*/
        seq.Append(SpinnerSpriteRenderer.transform.DOLocalRotate(Vector3.forward * 360, RotationSpeed, RotateMode.LocalAxisAdd)
            .SetEase(EasePerLoop ? RotationEase : Ease.Linear).SetLoops(NumberOfSpins));
		// TODO: make this use parameters.
		seq.Insert(0, SpinnerSpriteRenderer.transform.DOScale(1, .5f).SetEase(Ease.OutBack));
		
		Tween drawLine =  DOTween.To(() => LineSpriteMask.alphaCutoff, x => LineSpriteMask.alphaCutoff = x, 0, MaskWipeTime)
			.SetEase(MaskWipeEase);
		seq.Insert(LineAppearAtTime, drawLine);
		seq.InsertCallback(LineAppearAtTime, OnSpawnCompleteTick);
		
		seq.SetEase(EasePerLoop ? Ease.Linear : RotationEase);
		seq.Play();
		
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
	
	// Update is called once per frame
	private float spawnTimeCounter = 0;
	private bool spawned = false;
	void Update () {
	}

	private void OnSpawnCompleteTick() {
		if (onSpawnComplete != null) {
			onSpawnComplete(this);
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(CutVector, 1f);
	}
}
