using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

[System.Serializable]
public class QualityPointsDictionary : SerializableDictionary<Quality.QualityGrade, float> {}

public class UpgradeBar : MonoBehaviour {
	[BoxGroup("Balance")]
	// Current amount that the bar is filled + possible grades.
	public Quality.QualityGrade StartingGrade;
	
	[BoxGroup("Feel")]
	// Padding that the inner bar should have from the outer bar / border.
	public float Padding;
	[BoxGroup("Feel")]
	public Ease OverallEase;
	[BoxGroup("Feel")]
	public Ease LevelUpEase;
	[BoxGroup("Feel")]
	public float LevelUpEaseDuration;
	[BoxGroup("Feel")]
	public Vector2 EnterEndPosition;
	[BoxGroup("Feel")]
	public Ease EnterEase;
	[BoxGroup("Feel")]
	public float EnterDuration;
	
	
	// UI objects to use and some helpers for them.
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI TextCurrentLevel;
	[BoxGroup("Object Assignments")]
	public GameObject Background;
	[BoxGroup("Object Assignments")]
	public GameObject Foreground;
	
	[BoxGroup("Debug")]
	public float TestPoints;
	
	//private Image BackgroundImage;
	private RectTransform BackgroundTransform;
	private Image foregroundImage;
	private RectTransform foregroundTransform;
	// Helpers for positioning + moving/resizing.
	private float barHeight;
	private float barMinWidth;
	private float barMaxWidth;
	
	// State
	private float fillAmount = 0;
	private Quality.QualityGrade currentGrade;
	
	// Grade data.
	private LinkedList<Quality.QualityGrade> grades;
	
	[Button("Perform")]
	private void Perform() {
		PerformFill(TestPoints);
	}

	[Button("Reset")]
	private void Reset() {
		currentGrade = StartingGrade;
		TextCurrentLevel.text = Quality.GradeToString(currentGrade);
		TextCurrentLevel.color = Quality.GradeToColor(currentGrade);
		foregroundImage.color = Quality.GradeToColor(currentGrade);
		fillAmount = 0;
		foregroundTransform.DOComplete();
		UpdateFillAmount();
	}


	private void Start() {
		//BackgroundImage = Background.GetComponent<Image>();
		BackgroundTransform = Background.GetComponent<RectTransform>();
		foregroundImage = Foreground.GetComponent<Image>();
		foregroundTransform = Foreground.GetComponent<RectTransform>();

		// Get quality grades for shop level 3.
		grades = new LinkedList<Quality.QualityGrade>(Quality.GetPossibleGrades(3));
		// Pop the current grade.
		//currentGrade = grades.First.Value;
		currentGrade = StartingGrade;
		//grades.RemoveFirst();

		// Initialize with details.
		TextCurrentLevel.text = Quality.GradeToString(currentGrade);
		TextCurrentLevel.color = Quality.GradeToColor(currentGrade);
		foregroundImage.color = Quality.GradeToColor(currentGrade);

		//fillAmount = 0;

		// POSITIONING FOREGROUND BAR.
		// Foreground bar min/max derived from the background.
		// These are inverted from what you would expect because of Unity anchors.
		// barMaxWidth = 0.0 -> barMinWidth = 1.0;
		barHeight = BackgroundTransform.rect.height - Padding * 2f;
		barMaxWidth = Padding * 2f;
		barMinWidth = BackgroundTransform.rect.width + Padding * 2f;

		// Position the middle of the bar correctly in almost all circumstances.
		Vector3 pos = foregroundTransform.anchoredPosition;
		//pos.x = -Padding;
		pos.x = Padding;
		foregroundTransform.anchoredPosition = pos;

		// Fill initial correctly.
		//foregroundTransform.sizeDelta = new Vector2(-barMaxWidth, barHeight);
		float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);
		foregroundTransform.sizeDelta = new Vector2(fill, barHeight);
	}

	public void PerformFill(float points, float startAt = 0) {
		var achievedQuality = Quality.CalculateLevelFromPoints(points);
		Debug.Log("Moving up " + achievedQuality + " levels.");
		
        fillAmount = 0;
        UpdateFillAmount();
		Sequence seq = DOTween.Sequence();
		for (int i = 0; i < (int) achievedQuality.x; i++) {
			// Add a new bar fill animation.
            seq.Append(foregroundTransform.DOSizeDelta(new Vector2(-barMaxWidth, barHeight), LevelUpEaseDuration)
	            .SetEase(LevelUpEase).OnComplete(() => MoveUpQualityLevel()));
		}

		// Fill remaining percentage.
		fillAmount = achievedQuality.y;
		float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);
		seq.Append(foregroundTransform.DOSizeDelta(new Vector2(fill, barHeight), .8f).SetEase(Ease.Linear));

		seq.SetEase(OverallEase);
		seq.Play();
	}

	private void UpdateFillAmount() {
		float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);
		foregroundTransform.sizeDelta = new Vector2(fill, barHeight);
	}

	// Take the current grade as final.
	public Quality.QualityGrade Finish() {
		PlayRelevantQualitySFX(currentGrade);
		return currentGrade;
	}

	public void Appear() {
		gameObject.GetComponent<RectTransform>().DOAnchorPos(EnterEndPosition, EnterDuration).SetEase(EnterEase);
	}

	public void Disappear() {
		// TODO: spawn some effect...
		this.gameObject.SetActive(false);
	}

	public void PlayRelevantQualitySFX(Quality.QualityGrade grade) {
		Debug.Log("Playing grade sound " + grade);
		switch (grade) {
			case Quality.QualityGrade.Junk:
				SFX.Play("Game_Quality_Junk",1f,1f,0f,false,0f);
				break;
			case Quality.QualityGrade.Brittle:
				SFX.Play("Game_Quality_Brittle",1f,1f,0f,false,0f);
				break;
			case Quality.QualityGrade.Passable:
				SFX.Play("Game_Quality_Passable",1f,1f,0f,false,0f);
				break;
			case Quality.QualityGrade.Sturdy:
				SFX.Play("Game_Quality_Sturdy",1f,1f,0f,false,0f);
				break;
			case Quality.QualityGrade.Magical:
				SFX.Play("Game_Quality_Magical",1f,1f,0f,false,0f);
				break;
			case Quality.QualityGrade.Mystic:
				SFX.Play("Game_Quality_Mystic",1f,1f,0f,false,0f);
				break;
		}
	}

	// Moves to the next quality level in the queue.  If none is available, returns false.
	private bool MoveUpQualityLevel(float spare = 0f) {
		if (grades.Count == 5) {
			return false;
		}

		// Update the current grade.
		var current = grades.Find(currentGrade);
		//Debug.Log(subsequent.Value + " next grade");
		if (current == null || current.Previous == null) {
			return false;
		}

		currentGrade = current.Previous.Value;
		//Debug.Log("current grade is " + currentGrade);

		TextCurrentLevel.text = Quality.GradeToString(currentGrade);
		TextCurrentLevel.color = Quality.GradeToColor(currentGrade);
		foregroundImage.color = Quality.GradeToColor(currentGrade);

		foregroundTransform.DOComplete();
		
		fillAmount = 0f;
		UpdateFillAmount();
		//UpdateQualityBar(Ease, LevelChangeEaseTime);
		//Add(spare);

        //float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);
        //foregroundTransform.sizeDelta = new Vector2(fill, barHeight);

        // SFX.Play("??");
        //New SFX placed by Pierre 27/8/18
        SFX.Play("Mini_Game_Quality_gain", 1f, 1f, 0f, false, 0f);

        return true;
	}

	//Needed due to SFX for Pierre in Polishing
	public Quality.QualityGrade ReturnCurrentGrade() {
		return currentGrade;
	}
}
