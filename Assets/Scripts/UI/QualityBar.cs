using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;

public class QualityBar : MonoBehaviour
{
	// UI objects to use and some helpers for them.
	public TextMeshProUGUI TextCurrentLevel;
	public GameObject Background;
	//private Image BackgroundImage;
	private RectTransform BackgroundTransform;

	public GameObject Foreground;
	private Image foregroundImage;
	private RectTransform foregroundTransform;

	// Padding that the inner bar should have from the outer bar / border.
	public float Padding;

	// Amount to take away whenever the hourglass ticks.
	public float TickSubtraction;
	
	// Amount to take away (over time) each second -- UpdateSubtraction * Time.deltaTime.
	public float UpdateSubtraction;

	// Ease to use.
	public Ease Ease;
	public float RegularEaseTime;
	public float LevelChangeEaseTime;

	[Range(0, 1)]
	public float StartFill = 1;

	// Helpers for positioning + moving/resizing.
	private float barHeight;
	private float barMinWidth;
	private float barMaxWidth;

	// Current amount that the bar is filled + possible grades.
	private float fillAmount;
	private LinkedList<Quality.QualityGrade> grades;
	public Quality.QualityGrade StartingGrade;

	private Quality.QualityGrade currentGrade;

	// DEBUG.
	public bool debug;
	[ShowIf("debug")]
	public float speedMult;

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

		fillAmount = StartFill;

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

		// Subscribe to countdown tick.
		Countdown.onTick += SubtractFixed;
	}

	private void Update() {
		if (debug) Add(Time.deltaTime * speedMult, true);
		// This might be peformance intensive.
		Subtract(UpdateSubtraction * Time.deltaTime, true);
	}

	public void Add(float amount, bool allowMoveUp = false) {
		fillAmount += amount;
		if (fillAmount >= 1) {
			if (!allowMoveUp) {
				fillAmount = 1f;
				return;
			}

			//Debug.Log("Fill amount is " + fillAmount + " and amount is " + amount);
			if (!MoveUpQualityLevel(fillAmount - 1f)) {
				// TODO: encouragement?
				fillAmount = 1f;
			}
		} else {
			UpdateQualityBar();
		}

	}

	public void Subtract(float amount, bool allowMoveDown = false) {
		fillAmount -= amount;
		if (fillAmount <= 0) {
			if (!allowMoveDown) {
				fillAmount = 0f;
				UpdateQualityBar();
				return;
			}

			if (!MoveDownQualityLevel(-fillAmount)) {
				// TODO: fail state???
				fillAmount = 0f;
			}
		} else {
			UpdateQualityBar();
		}
	}

	public void SubtractFixed() {
		fillAmount -= TickSubtraction;
		if (fillAmount <= 0) {
			if (!MoveDownQualityLevel(-fillAmount)) {
				// TODO: fail state???
				fillAmount = 0f;
			}
		} else {
			UpdateQualityBar();
		}
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

	private void UpdateQualityBar(Ease ease = Ease.Unset, float easeTime = -1f) {
		// Hack to work around Unity's variable assignment not being a compile time constant.
		if (ease == Ease.Unset)
			ease = this.Ease;
		if (easeTime <= -1f)
			easeTime = RegularEaseTime;


		// Amount of 'fill' the current bar holds.
		float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);

		foregroundTransform.DOSizeDelta(new Vector2(fill, barHeight), easeTime).SetEase(ease);
	}

	// Moves to the next quality level in the queue.  If none is available, returns false.
	private bool MoveDownQualityLevel(float spare = 0f)
	{
		if (currentGrade == grades.Last.Value) {
			return false;
		}

		// Update the current grade.
		var current = grades.Find(currentGrade);
		if (current == null || current.Next == null) {
			return false;
		}

		currentGrade = current.Next.Value;

		TextCurrentLevel.text = Quality.GradeToString(currentGrade);
		TextCurrentLevel.color = Quality.GradeToColor(currentGrade);
		foregroundImage.color = Quality.GradeToColor(currentGrade);

		foregroundTransform.DOComplete();
		
		fillAmount = 1f;// - spare;
		UpdateQualityBar(Ease, LevelChangeEaseTime);
		Subtract(spare);
		
		//float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);
		//foregroundTransform.sizeDelta = new Vector2(fill, barHeight);
		

		SFX.Play("quality_bar_deplete");

		return true;
	}

	// Moves to the next quality level in the queue.  If none is available, returns false.
	private bool MoveUpQualityLevel(float spare = 0f)
	{
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
		UpdateQualityBar(Ease, LevelChangeEaseTime);
		Add(spare);
		
		//float fill = -Mathf.Lerp(barMinWidth, barMaxWidth, fillAmount);
		//foregroundTransform.sizeDelta = new Vector2(fill, barHeight);

		// SFX.Play("??");

		return true;
	}
}
