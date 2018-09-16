using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

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
	public Ease PointsTextEndEase;
	[BoxGroup("Feel")]
	public float PointsTextEndEaseDuration;
	[BoxGroup("Feel")]
	public Ease PointsTextDiminishEase;
	[BoxGroup("Feel")]
	public float PointsTextDiminishDuration;
	
	
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI PointsText;
	[BoxGroup("Object Assignments")]
	public UpgradeBar UpgradeBar;
	
	
	private RectTransform pointsTextTransform;

	private float points;

	[Range(0, 2)]
	public float PunchAmount = 1f,
		PunchDuration = 1f,
		PunchElasticity = 1f;

	public int PunchVibration = 10;

	// Use this for initialization
	void Start () {
		pointsTextTransform = PointsText.GetComponent<RectTransform>();
	}

	[Button("Punch")]
	private void Punch() {
		pointsTextTransform.DOComplete();
		PointsText.DOComplete();
		pointsTextTransform.DOPunchScale(Vector3.one * PunchAmount, PunchDuration, PunchVibration, PunchElasticity);
		PointsText.DOColor(PointAddColor, ColorEaseDuration).SetEase(ColorEase);
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
		//UpgradeBar.gameObject.SetActive(true);
		UpgradeBar.Appear();
		gameObject.GetComponent<RectTransform>().DOAnchorPos(PointsTextEndPosition, PointsTextEndEaseDuration).SetEase(PointsTextEndEase)
			.OnComplete(() => {
				UpgradeBar.PerformFill(points);
				DOTween.To(() => points, x => { points = x; UpdateText(); }, 0, PointsTextDiminishDuration)
					.SetEase(PointsTextDiminishEase);
			});
	}
	
}
