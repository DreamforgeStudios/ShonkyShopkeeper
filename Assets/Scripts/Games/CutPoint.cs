using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

// Cut indicators tick down as soon as they are created.
// TODO: for polish, make this transition non-linearly.
public class CutPoint : MonoBehaviour {
	// The "countdown" time.
	[BoxGroup("Circle Properties")]
	public Gradient CircleColor;
	
	// The "countdown" time.
	[BoxGroup("Circle Properties")]
	[Slider(0f, 5f)]
	public float RadiusTransitionTime;
	
	// How thick the circle should be.
	[BoxGroup("Circle Properties")]
	[Slider(0f, .25f)]
	public float StartCircleThickness;
	[BoxGroup("Circle Properties")]
	[Slider(0f, .25f)]
	public float EndCircleThickness;
	
	[BoxGroup("Circle Properties")]
	[Slider(0f, 1f)]
	public float StartCircleRadius;
	[BoxGroup("Circle Properties")]
	[Slider(0f, 1f)]
	public float EndCircleRadius;
	
	[BoxGroup("Circle Properties")]
	public Ease CircleEase;
	
	
	[BoxGroup("Line Properties")]
	[Slider(0f, 5f)]
	public float LineThicknessTransitionTime;
	//[BoxGroup("Line Properties")]
	//[Slider(0f, 5f)]
	//public float LineColorTransitionTime;

	[BoxGroup("Line Properties")]
	public Vector2 StartLineBeginEndThickness;
	[BoxGroup("Line Properties")]
	public Vector2 EndLineBeginEndThickness;

	[BoxGroup("Line Properties")]
	public Gradient LRStart; 
	//[BoxGroup("Line Properties")]
	//public Gradient LREnd; 
	[BoxGroup("Line Properties")]
	public Ease LREase;
	
	
	// When the line is active, these properties will be used.
	[BoxGroup("Active Properties")]
	public Gradient ActiveLRStart;
	[BoxGroup("Active Properties")]
	[Slider(0f, 5f)]
	public float ActiveLineThicknessTransitionTime;
	//[BoxGroup("Active Properties")]
	//public Gradient LREndActive;
	[BoxGroup("Active Properties")]
	public Gradient ActiveCircleColor;
	[BoxGroup("Active Properties")]
	public Vector2 ActiveLineBeginEndThickness;
	[BoxGroup("Active Properties")]
	public Ease ActiveLREase;

	// The vector to draw with a line renderer.
	[BoxGroup("General Properties")]
	public Vector3 CutVector;

	public float SpawnTime {
		get { return RadiusTransitionTime; }
	}

	private Renderer r;
	private LineRenderer lr;
	private Tweener startWidthTween, endWidthTween;

	public delegate void OnSpawnComplete(CutPoint cut);
	public event OnSpawnComplete onSpawnComplete;

	// Use this for initialization
	void Awake () {
		lr = GetComponent<LineRenderer>();
		r = GetComponent<Renderer>();
	}

	void Start() {
		Initialize();
		RunAnimation();
	}

	[Button("Reset Values")]
	private void Initialize() {
		r.material.SetFloat("_RadiusWidth", StartCircleThickness);
		r.material.SetFloat("_Radius", StartCircleRadius);
		lr.startWidth = StartLineBeginEndThickness.x;
		lr.endWidth = StartLineBeginEndThickness.y;
		r.material.SetColor("_Color", CircleColor.Evaluate(0));
		lr.colorGradient = LRStart;
	}

	[Button("Run Animation")]
	private void RunAnimation() {
		//r.material.color = CircleColor.Evaluate(0);
		//r.material.DOColor(CircleColor.Evaluate(1), RadiusTransitionTime).SetEase(CircleEase);
		r.material.DOFloat(EndCircleThickness, "_RadiusWidth", RadiusTransitionTime).SetEase(CircleEase);
		r.material.DOFloat(EndCircleRadius, "_Radius", RadiusTransitionTime).SetEase(CircleEase)
			.OnComplete(() => DrawLine());
	}

	[Button("Set Selected")]
	public void SetSelected() {
		r.material.color = ActiveCircleColor.Evaluate(0);
		lr.colorGradient = ActiveLRStart;

		startWidthTween.Complete();
		startWidthTween = DOTween.To(x => lr.startWidth = x, lr.startWidth, ActiveLineBeginEndThickness.x,
			ActiveLineThicknessTransitionTime).SetEase(ActiveLREase);
		
		endWidthTween.Complete();
		endWidthTween = DOTween.To(x => lr.endWidth = x, lr.endWidth, ActiveLineBeginEndThickness.y,
			ActiveLineThicknessTransitionTime).SetEase(ActiveLREase);
	}
	
	[Button("Unset Selected")]
	public void UnsetSelected() {
		r.material.color = CircleColor.Evaluate(0);
		lr.colorGradient = LRStart;
		
		startWidthTween.Complete();
		startWidthTween = DOTween.To(x => lr.startWidth = x, lr.startWidth, EndLineBeginEndThickness.x,
			LineThicknessTransitionTime).SetEase(LREase);
		
		endWidthTween.Complete();
		endWidthTween = DOTween.To(x => lr.endWidth = x, lr.endWidth, EndLineBeginEndThickness.y,
			LineThicknessTransitionTime).SetEase(LREase);
	}
	
	// Update is called once per frame
	private float spawnTimeCounter = 0;
	private bool spawned = false;
	void Update () {
		// Once the circle is finished, the cut is considered spawned.
		if (!spawned && spawnTimeCounter > RadiusTransitionTime) {
			OnSpawnCompleteTick();
			spawned = true;
		}
		
		spawnTimeCounter += Time.deltaTime;
	}

	[Button("Draw Line")]
	private void DrawLine() {
		lr.positionCount = 2;
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position + CutVector);

		startWidthTween = DOTween.To(x => lr.startWidth = x, StartLineBeginEndThickness.x, EndLineBeginEndThickness.x,
			LineThicknessTransitionTime).SetEase(LREase);
		endWidthTween = DOTween.To(x => lr.endWidth = x, StartLineBeginEndThickness.y, EndLineBeginEndThickness.y,
			LineThicknessTransitionTime).SetEase(LREase);

		//lr.colorGradient = LRStart;
		/*
		lr.DOColor(new Color2(LRStart.Evaluate(0), LRStart.Evaluate(1)),
			       new Color2(LREnd.Evaluate(0), LREnd.Evaluate(1)), LineColorTransitionTime).SetEase(LREase);
			       */
	}

	private void OnSpawnCompleteTick() {
		if (onSpawnComplete != null) {
			onSpawnComplete(this);
		}
	}
}
