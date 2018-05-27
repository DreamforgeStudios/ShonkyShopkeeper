using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Cut indicators tick down as soon as they are created.
// TODO: for polish, make this transition non-linearly.
public class CutPoint : MonoBehaviour {
	// The "countdown" time.
	public float radiusTransitionTime;
	public float thicknessTransitionTime;
	// How big the circle should be initially.
	public float startRadius;
	public float lineStartThickness;
	// How big the circle should be at its smallest point.
	public float endRadius;
	public float lineEndThickness;

	public Color32[] lrColors;
	public Ease ease;

	// How thick the circle should be.
	public float thickness;

	// Helper variables.
	//private float currentRadius;
	private float currentRadiusTime;
	//private float currentThickness;
	private float currentThicknessTime;

	// The vector to draw with a line renderer.
	public Vector3 cutVector;

	// How long the circle should stick around after its reached the final circle.
	// Not sure about this one.
	public float afterlife;

	private Renderer r;
	private LineRenderer lr;

	// Use this for initialization
	void Awake () {
		//currentRadius = startRadius;
		currentRadiusTime = 0;
		//currentThickness = lineStartThickness;
		currentThicknessTime = 0;

		lr = GetComponent<LineRenderer>();
		r = GetComponent<Renderer>();
		r.material.SetFloat("_RadiusWidth", thickness);

		//SetCutVector(cutVector);
	}

	void Start() {
		r.material.DOFloat(endRadius, "_Radius", radiusTransitionTime).SetEase(ease);
	}
	
	// Update is called once per frame
	void Update () {
		//currentRadiusTime += Time.deltaTime;
		//float rw = Mathf.Lerp(startRadius, endRadius, currentRadiusTime / radiusTransitionTime);
		//r.material.SetFloat("_Radius", rw);

		//currentThicknessTime += Time.deltaTime;
		//float w = Mathf.Lerp(lineStartThickness, lineEndThickness, currentThicknessTime / thicknessTransitionTime);

		// Use this until Unity 2018.2 to work around a bug.
		//lr.startWidth = w; -- should work but buggy sometimes.
		//AnimationCurve curve = new AnimationCurve();
		//curve.AddKey(0, w);
		//curve.AddKey(1, 0);
		//lr.widthCurve = curve;
	}

	public void SetCutVector(Vector3 cut) {
		cutVector = cut;
		DrawLine();
	}

	private void DrawLine() {
		lr.positionCount = 2;
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position + cutVector);

		// Messy for now, but we'll get through it.
		// TODO, make the color selection more intuitive.
		lr.DOColor(new Color2(lrColors[0], lrColors[2]), new Color2(lrColors[1], lrColors[3]), thicknessTransitionTime);
	}
}
