using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityScript.Steps;

public class PointsManager : MonoBehaviour {
	private float points;
	
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
	
	
	public delegate void OnFinishLeveling();
	public event OnFinishLeveling onFinishLeveling;
	
	private RectTransform pointsTextTransform;
	// Keep track of the original color so dotween doesn't fuck us and lose it.
	private Color originalColor;
	//private Tween colorTween;
	//private Tween punchTween;

	// Use this for initialization
	void Start () {
		pointsTextTransform = PointsText.GetComponent<RectTransform>();
		originalColor = PointsText.color;
	}
	
	[Button("Punch")]
	private void Punch() {
		pointsTextTransform.DOComplete();
		PointsText.DOComplete();
		pointsTextTransform.DOPunchScale(Vector3.one * PunchAmount, PunchDuration, PunchVibration, PunchElasticity);
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

	public float GetPoints() {
		return points;
	}

	public void OnFinishLevelingTick() {
		if (onFinishLeveling != null) {
			onFinishLeveling();
		}
		
	}

	[Button("Do EndGameTransition")]
	public void DoEndGameTransition() {
		var rect = PointsText.GetComponent<RectTransform>();

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
				.SetEase(PointsTextDiminishEase)
				.OnComplete(OnFinishLevelingTick);
		});
		
		seq.Play();
	}

	[Button("Do EndGameTransitionParty")]
	public void DoEndGameTransitionParty() {
		var rect = PointsText.GetComponent<RectTransform>();

		var seq = DOTween.Sequence();
		seq.Append(rect.DOScale(0, PointsTextEndEaseDuration).SetEase(PointsTextEndEaseOut));
		seq.Append(rect.DOScale(1, PointsTextEndEaseDuration).SetEase(PointsTextEndEaseIn));
		seq.AppendCallback(OnFinishLevelingTick);
		
		seq.Play();
	}
	
}
