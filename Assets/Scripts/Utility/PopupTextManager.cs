using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;

public class PopupTextManager : MonoBehaviour {
	[BoxGroup("Scroll Parameters")]
	public Vector3 StartScrollPosition;
	[BoxGroup("Scroll Parameters")]
	public Vector3 EndScrollPosition;
	[BoxGroup("Scroll Parameters")]
	public float ScrollDurationIn;
	[BoxGroup("Scroll Parameters")]
	public float ScrollDurationOut;
	[BoxGroup("Scroll Parameters")]
	public Ease ScrollEaseIn;
	[BoxGroup("Scroll Parameters")]
	public Ease ScrollEaseOut;
	
	[BoxGroup("Text Parameters")]
	public List<string> PopupTexts;
	[BoxGroup("Text Parameters")]
	public float FadeDurationIn = .7f;
	[BoxGroup("Text Parameters")]
	public float FadeDurationOut = .7f;
	[BoxGroup("Text Parameters")]
	public Ease FadeEaseIn = Ease.InOutSine;
	[BoxGroup("Text Parameters")]
	public Ease FadeEaseOut = Ease.InOutSine;

	[BoxGroup("Object Assignments")]
	public CanvasScaler scaler;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI TextFront;
	[BoxGroup("Object Assignments")]
	public TextMeshProUGUI TextBack;
	[BoxGroup("Object Assignments")]
	public GameObject Turner;
	[BoxGroup("Object Assignments")]
	public GameObject Closer;
	
	[ReadOnly]
	public int activePage = 0;

	private Image closerImg;
	private Button closerBtn;
	private RectTransform rTransform;

	private bool entered = false;
	
	// TODO: responsive page turner.

	// Use this for initialization
	void Start () {
		TextFront.text = PopupTexts[activePage];

		closerImg = Closer.GetComponent<Image>();
		closerBtn = Closer.GetComponent<Button>();
		
		UpdateCloser();
	}

	[Button("Enter")]
	// TODO; add some force to the 'vial' shader here.
	public void DoEnterAnimation() {
		if (entered) return;
		
        GetComponent<RectTransform>().DOAnchorPos(EndScrollPosition, ScrollDurationIn).SetEase(ScrollEaseIn)
            .OnComplete(() => entered = true);
	}

	[Button("Exit")]
	public void DoExitAnimation() {
		if (!entered) return;
		
		GetComponent<RectTransform>().DOAnchorPos(StartScrollPosition, ScrollDurationOut).SetEase(ScrollEaseOut)
			.OnComplete(() => entered = false);
	}

	private void UpdateCloser() {
		if (entered && activePage >= PopupTexts.Count - 1) {
			closerImg.color = Color.green;
			closerBtn.enabled = true;
		} else {
			closerImg.color = Color.red;
			closerBtn.enabled = false;
		}
	}

	[Button("Next")]
	public void NextText() {
		if (activePage + 1 >= PopupTexts.Count) return;
		
		TextBack.alpha = 1f;
		TextFront.alpha = 0f;
		
		TextBack.text = TextFront.text;
        TextFront.text = PopupTexts[++activePage];

		DOTween.To(x => TextBack.alpha = x, 1f, 0f, FadeDurationOut).SetEase(FadeEaseOut);
		DOTween.To(x => TextFront.alpha = x, 0f, 1f, FadeDurationIn).SetEase(FadeEaseIn);

		UpdateCloser();
	}

	// TODO: animate.
	[Button("Previous")]
	public void PreviousText() {
		if (activePage <= 0) return;
		
		TextBack.alpha = 1f;
		TextFront.alpha = 0f;
		
		TextBack.text = TextFront.text;
        TextFront.text = PopupTexts[--activePage];

		DOTween.To(x => TextBack.alpha = x, 1f, 0f, FadeDurationOut).SetEase(FadeEaseOut);
		DOTween.To(x => TextFront.alpha = x, 0f, 1f, FadeDurationIn).SetEase(FadeEaseIn);
		
		UpdateCloser();
	}
}
