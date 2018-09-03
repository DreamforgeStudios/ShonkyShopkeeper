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
	[BoxGroup("Text Parameters")]
    // A layer mask so that we only hit slots.
    public LayerMask LayerMask;

	//[BoxGroup("Object Assignments")]
	//public CanvasScaler scaler;
	[BoxGroup("Object Assignments")]
	public TextMeshPro TextFront;
	[BoxGroup("Object Assignments")]
	public TextMeshPro TextBack;
	[BoxGroup("Object Assignments")]
	public GameObject Turner;
	// The 'closer' is the object that allows the user to close the text box.
	[BoxGroup("Object Assignments")]
	public GameObject Closer;
	
	[ReadOnly] // only read only in inspector.
	public int ActivePage = 0;

	private Material closerMat;
	//private Button closerBtn;
	//private RectTransform rTransform;

	private bool entered = false;

	void Start() {
		// Init();
	}
	
	void Update() {
		// Check where we are running the program.
		RuntimePlatform p = Application.platform;
		if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.OSXPlayer)
			// Process mouse inputs.
			ProcessMouse();
		else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
			// Process touch inputs.
			ProcessTouch();
	}

	// Put the narrative manager back to a default state.
	public void Init() {
		closerMat = Closer.GetComponent<MeshRenderer>().material;
		ActivePage = 0;
		TextFront.text = PopupTexts[ActivePage];
		UpdateCloser();
	}

	private void ProcessMouse() {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask)) {
				if (hit.transform.CompareTag("MainButton"))
					NextText();
				else if (hit.transform.CompareTag("Aux"))
					DoExitAnimation();
			}
		}
	}
	
	private void ProcessTouch() {
        if (Input.touchCount == 0) {
            return;
        }

		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask)) {
			if (hit.transform.CompareTag("MainButton"))
				NextText();
			else if (hit.transform.CompareTag("Aux"))
				DoExitAnimation();
		}
	}

	[Button("Enter")]
	// Enter the scene.
	public void DoEnterAnimation() {
		if (entered) return;

		transform.DOLocalMove(EndScrollPosition, ScrollDurationIn).SetEase(ScrollEaseIn)
			.OnComplete(() => entered = true);
	}

	[Button("Exit")]
	// Leave the scene.
	public void DoExitAnimation() {
		if (!entered) return;

		transform.DOLocalMove(StartScrollPosition, ScrollDurationOut).SetEase(ScrollEaseOut)
			.OnComplete( () => Destroy(gameObject)); // We probably shouldn't destroy, but not sure what else to do atm.
	}

	// Update the closer so that if we're on the last page it can be closed.
	private void UpdateCloser() {
		if (ActivePage >= PopupTexts.Count - 1) {
			closerMat.color = Color.green;
		} else {
			closerMat.color = Color.red;
		}
	}
	
	// Keep track of tweens so that going fast doesn't break things.
	private Tween textBackTween, textFrontTween;
	// Moves to the next page of text (if there is one).
	[Button("Next")]
	public void NextText() {
		if (ActivePage + 1 >= PopupTexts.Count) return;
		
		textBackTween.Complete();
		textFrontTween.Complete();
		
		TextBack.alpha = 1f;
		TextFront.alpha = 0f;
		
		TextBack.text = TextFront.text;
        TextFront.text = PopupTexts[++ActivePage];

		textBackTween = DOTween.To(x => TextBack.alpha = x, 1f, 0f, FadeDurationOut).SetEase(FadeEaseOut);
		textFrontTween = DOTween.To(x => TextFront.alpha = x, 0f, 1f, FadeDurationIn).SetEase(FadeEaseIn);

		UpdateCloser();
	}

	// Moves to the previous page of text (if there is one).
	[Button("Previous")]
	public void PreviousText() {
		if (ActivePage <= 0) return;
		
		textBackTween.Complete();
		textFrontTween.Complete();
		
		TextBack.alpha = 1f;
		TextFront.alpha = 0f;
		
		TextBack.text = TextFront.text;
        TextFront.text = PopupTexts[--ActivePage];

		textBackTween = DOTween.To(x => TextBack.alpha = x, 1f, 0f, FadeDurationOut).SetEase(FadeEaseOut);
		textFrontTween = DOTween.To(x => TextFront.alpha = x, 0f, 1f, FadeDurationIn).SetEase(FadeEaseIn);
		
		UpdateCloser();
	}
}
