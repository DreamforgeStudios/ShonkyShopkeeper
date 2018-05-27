﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour {
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;
    List<TextMeshProUGUI> texts;

    private int textCounter = 0;
    // Use this for initialization
    void Start () {
        texts = new List<TextMeshProUGUI>();
        texts.Add(text1);
        texts.Add(text2);
        texts.Add(text3);
        texts.Add(text4);
        texts.Add(text5);
        foreach (TextMeshProUGUI text in texts) {
            text.enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
            AdvanceText();
        }
	}

    private void AdvanceText() {
        if (textCounter <= texts.Count - 1) {
            texts[textCounter].CrossFadeAlpha(0f, 2f, false);
            textCounter++;
            texts[textCounter].CrossFadeAlpha(255f, 2f, false);
        } else {
            SceneManager.LoadScene("Shop");
        }
    }
}
