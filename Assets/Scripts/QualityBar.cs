using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QualityBar : MonoBehaviour {
	// UI objects to use and some helpers for them.
	public TextMeshProUGUI textCurrentLevel;
	public GameObject background;
	private Image backgroundImage;
	private RectTransform backgroundTransform;

	public GameObject foreground;
	private Image foregroundImage;
	private RectTransform foregroundTransform;

	// Padding that the inner bar should have from the outer bar / border.
	public float padding;

	// Amount to take away whenever the hourglass ticks.
	public float tickSubtraction;

	// Ease to use.
	public Ease ease;

	// Helpers for positioning + moving/resizing.
	private float barHeight;
	private float barMinWidth;
	private float barMaxWidth;

	// Current amount that the bar is filled + possible grades.
	private float fillAmount;
	private LinkedList<Quality.QualityGrade> grades;

    // Audio Source for sound effect when the bar changes quality
    public AudioSource source;
    public AudioClip sound;

	private Quality.QualityGrade currentGrade;

	// DEBUG.
	public bool debug;
	public float speedMult;

	private void Start() {
        //Setup audio
        source.clip = sound;

		backgroundImage = background.GetComponent<Image>();
		backgroundTransform = background.GetComponent<RectTransform>();
		foregroundImage = foreground.GetComponent<Image>();
		foregroundTransform = foreground.GetComponent<RectTransform>();

		// Get quality grades for shop level 3.
		grades = new LinkedList<Quality.QualityGrade>(Quality.GetPossibleGrades(3));
		// Pop the current grade.
		currentGrade = grades.First.Value;
		grades.RemoveFirst();

		// Initialize with details.
		textCurrentLevel.text = Quality.GradeToString(currentGrade);
		textCurrentLevel.color = Quality.GradeToColor(currentGrade);
		foregroundImage.color = Quality.GradeToColor(currentGrade);

		fillAmount = 1f;

		// POSITIONING FOREGROUND BAR.
		// Foreground bar min/max derived from the background.
		// These are inverted from what you would expect because of Unity anchors.
		// barMaxWidth = 0.0 -> barMinWidth = 1.0;
		barHeight = backgroundTransform.rect.height - padding*2f;
		barMaxWidth = padding*2f;
		barMinWidth = backgroundTransform.rect.width + padding*2f;

		// Position the middle of the bar correctly in almost all circumstances.
		Vector3 pos = foregroundTransform.anchoredPosition;
		pos.x = -padding;
		foregroundTransform.anchoredPosition = pos;

		// Fill initial correctly.
		foregroundTransform.sizeDelta = new Vector2(-barMaxWidth, barHeight);

		// Subscribe to countdown tick.
		Countdown.onTick += SubtractFixed;
	}

	private void Update() {
		if (debug) Subtract(Time.deltaTime * speedMult);
	}

	public void Add(float amount) {
		fillAmount += amount;
		if (fillAmount >= 1) {
			// Moving up quality levels is EXPERIMENTAL.  DO NOT USE.
			//if (!MoveUpQualityLevel(fillAmount - 1f)) {
				// TODO: encouragement?
				fillAmount = 1f;
			//}
		} else {
			UpdateQualityBar();
		}

	}

	public void Subtract(float amount) {
		fillAmount -= amount;
		if (fillAmount <= 0) {
			if (!MoveDownQualityLevel(-fillAmount)) {
				// TODO: fail state???
				fillAmount = 0f;
			}
		} else {
			UpdateQualityBar();
		}
	}

	public void SetFixedSubtraction(float val) {
		tickSubtraction = val;
	}

	public void SubtractFixed() {
		fillAmount -= tickSubtraction;
		if (fillAmount <= 0) {
			if (!MoveDownQualityLevel(-fillAmount)) {
				// TODO: fail state???
				fillAmount = 0f;
			}

			return;
		}

		UpdateQualityBar();
	}

	// Take the current grade as final.
	public Quality.QualityGrade Finish() {
		Countdown.onTick -= SubtractFixed;
		return currentGrade;
	}

	public void Disappear() {
		// TODO: spawn some effect...
		this.gameObject.SetActive(false);
	}

	private Tween UpdateQualityBar(Ease ease = Ease.OutBack) {
		// Amount of 'fill' the current bar holds.
		float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);

		// Tween the new fill.
		float currentWidth = foregroundTransform.sizeDelta.x;
		return DOTween.To(() => currentWidth, x => foregroundTransform.sizeDelta = new Vector2(x, barHeight), fill, 0.4f).SetEase(ease);
	}

	// Moves to the next quality level in the queue.  If none is available, returns false.
	private bool MoveDownQualityLevel(float spare = 0f) {
		if (grades.Count == 0) {
			return false;
		}
        source.Play();
		// Update the current grade.
		currentGrade = (Quality.QualityGrade) grades.First.Value;
		grades.RemoveFirst();

		textCurrentLevel.text = Quality.GradeToString(currentGrade);
		textCurrentLevel.color = Quality.GradeToColor(currentGrade);
		foregroundImage.color = Quality.GradeToColor(currentGrade);

		fillAmount = 1f;
		float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);
		foregroundTransform.sizeDelta = new Vector2(fill, barHeight);

		fillAmount -= spare;
		UpdateQualityBar(Ease.InCubic);

		return true;
	}

	// Moves to the next quality level in the queue.  If none is available, returns false.
	private bool MoveUpQualityLevel(float spare = 0f) {
		if (grades.Count == 5) {
			return false;
		}

		// Update the current grade.
		//int currentLevel = (int) grades.Last.Value;
		int currentLevel = (int) currentGrade;
		grades.AddFirst(currentGrade);
		currentGrade = (Quality.QualityGrade) currentLevel + 1;

		textCurrentLevel.text = Quality.GradeToString(currentGrade);
		textCurrentLevel.color = Quality.GradeToColor(currentGrade);
		foregroundImage.color = Quality.GradeToColor(currentGrade);

		fillAmount = 0f;
		float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);
		foregroundTransform.sizeDelta = new Vector2(fill, barHeight);

		//fillAmount += spare;
		UpdateQualityBar(Ease.InCubic);

		return true;
	}
}
