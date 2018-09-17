using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityScript.Steps;

public class PointsManager : MonoBehaviour {
	
	[BoxGroup("Feel")]
	public Color PointAddColor;
	[BoxGroup("Feel")]
	public Ease ColorEase;
	[BoxGroup("Feel")]
	public float ColorEaseDuration;
	[BoxGroup("Feel")]
	public Vector2 PointsTextEndPosition;
	[BoxGroup("Feel")]
	public Ease PointsTextEndEaseOut;
	[BoxGroup("Feel")]
	public Ease PointsTextEndEaseIn;
	[BoxGroup("Feel")]
	public float PointsTextEndEaseDuration;
	[BoxGroup("Feel")]
	public Ease PointsTextDiminishEase;
	[BoxGroup("Feel")]
	[Range(0, 2)]
	public float PunchAmount = 1f,
		PunchDuration = 1f,
		PunchElasticity = 1f;
	[BoxGroup("Feel")]
	public int PunchVibration = 10;
	
	
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI PointsText;
	[BoxGroup("Object Assignments")]
	public UpgradeBar UpgradeBar;
	
	
	private RectTransform pointsTextTransform;
	private float points;

	// Use this for initialization
	void Start () {
		pointsTextTransform = PointsText.GetComponent<RectTransform>();
	}

	[Button("Punch")]
	private void Punch() {
		pointsTextTransform.DOComplete();
		PointsText.DOComplete();
		pointsTextTransform.DOPunchScale(Vector3.one * PunchAmount, PunchDuration, PunchVibration, PunchElasticity);
		Color originalColor = PointsText.color;
		PointsText.DOColor(PointAddColor, ColorEaseDuration).SetEase(ColorEase)
			.OnComplete(() => PointsText.DOColor(originalColor, ColorEaseDuration).SetEase(ColorEase));
	}

	private void UpdateText() {
		PointsText.text = points.ToString("0");
	}

	public float DebugPoints;

	[Button("Add Points")]
	private void AddDebugPoints() {
		AddPoints(DebugPoints);
	}
	
	public void AddPoints(float points) {
		this.points += points;
		UpdateText();
		Punch();
	}

	[Button("Do EndGameTransition")]
	public void DoEndGameTransition() {
		var rect = gameObject.GetComponent<RectTransform>();


		// "Complicated" tweening with parameters is a pita, especially when the objects have been separated...
		//   why did i do that.
		// Comments are execution intervals i.e. all code in 2. executes at the same time.
		var seq = DOTween.Sequence();
		// 1.
		seq.Append(rect.DOScale(0, PointsTextEndEaseDuration).SetEase(PointsTextEndEaseOut));
		// 2.
		seq.AppendCallback(() => {
			UpgradeBar.Appear();
			rect.anchoredPosition = PointsTextEndPosition;
		});
		seq.Append(rect.DOScale(1, PointsTextEndEaseDuration).SetEase(PointsTextEndEaseIn));
		// 3.
		seq.AppendCallback(() => {
			float upgradeDuration = UpgradeBar.PerformFill(points);
			DOTween.To(() => points, x => {
					points = x;
					UpdateText();
				}, 0, upgradeDuration)
				.SetEase(PointsTextDiminishEase);
		});
		
		seq.Play();
				
		
		/*
		// Version without sequence.
		rect.DOScale(0, PointsTextEndEaseDuration).SetEase(PointsTextEndEaseOut)
			.OnComplete(() => {
				UpgradeBar.Appear();
				rect.anchoredPosition = PointsTextEndPosition;
				rect.DOScale(1, PointsTextEndEaseDuration).SetEase(PointsTextEndEaseIn)
					.OnComplete(() => {
						float upgradeDuration = UpgradeBar.PerformFill(points);
                        DOTween.To(() => points, x => { points = x; UpdateText(); }, 0, upgradeDuration)
                            .SetEase(PointsTextDiminishEase);
					});
			});
        */
	}
	
}
