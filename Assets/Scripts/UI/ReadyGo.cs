using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditorInternal;

public class ReadyGo : MonoBehaviour {
	public TextMeshProUGUI text;
	public bool waitForInitialTap;

	private int wordIndex;
	[ReorderableList]
	public string[] words;
	[ReorderableList]
	public float[] timeouts;
	[ReorderableList]
	public Color[] colors;

	[ReorderableList]
	public AudioClip[] sounds;	// Not in use yet...
	private float curTime;
	private bool start = false;

	public delegate void OnComplete();
	public static event OnComplete onComplete;

	// Use this for initialization
	void Start () {
		curTime = 0f;
		wordIndex = 0;
		text.text = words[wordIndex];
		// Timescale independent...
		text.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f).SetUpdate(true);
		text.color = colors[wordIndex];
	}
	
	// Update is called once per frame
	void Update () {
		// Check where we are running the program.
		RuntimePlatform p = Application.platform;
		if (p == RuntimePlatform.WindowsEditor || p == RuntimePlatform.WindowsPlayer || p == RuntimePlatform.OSXEditor || p == RuntimePlatform.OSXPlayer)
			// Process mouse inputs.
			ProcessMouse();
		else if (p == RuntimePlatform.IPhonePlayer || p == RuntimePlatform.Android)
			// Process touch inputs.
			ProcessTouch();

		// Don't progress until we tap.
		if (!start)
			return;
		
		if (curTime > timeouts[wordIndex]) {
			curTime = 0;
			if (wordIndex < words.Length-1) {
				wordIndex++;
			} else {
				OnCompleteTick();
			}

			ShowCurrentWord();
		}	

		curTime += Time.unscaledDeltaTime;
	}

    private void ProcessMouse() {
        if (Input.GetMouseButton(0)) {
			start = true;
		}
    }

    private void ProcessTouch() {
		if (Input.touches.Length > 0) {
			start = true;
		}
    }

	private void ShowCurrentWord() {
		text.text = words[wordIndex];
		// Timescale independent.
		text.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f).SetUpdate(true);
		text.color = colors[wordIndex];
	}

	private void OnCompleteTick() {
		this.gameObject.SetActive(false);
		if (onComplete != null) {
			onComplete();
		}
	}
}
