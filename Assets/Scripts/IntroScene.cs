using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : MonoBehaviour {
    public RawImage BG;
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;
    public TextMeshProUGUI loading;
    public TextMeshProUGUI next;
    List<TextMeshProUGUI> texts;

    private int textCounter = 0;
    // Use this for initialization
    void Start() {
        texts = new List<TextMeshProUGUI>();
        texts.Add(text1);
        texts.Add(text2);
        texts.Add(text3);
        texts.Add(text4);
        texts.Add(text5);
        foreach (TextMeshProUGUI text in texts) {
            text.enabled = true;
        }
        loading.enabled = false;
        next.enabled = true;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            AdvanceText();
        }
    }

    private void AdvanceText() {
        if (textCounter == 0) {
            BG.CrossFadeAlpha(0.1f, 2f, false);
            text1.CrossFadeAlpha(255f, 2f, false);
        }
        if (textCounter > 0 && textCounter < texts.Count) {
            texts[textCounter -1].CrossFadeAlpha(0f, 2f, false);
            if (textCounter <= texts.Count) {
                texts[textCounter].CrossFadeAlpha(255f, 2f, false);
            }
        }
        if (textCounter == texts.Count) {
            next.enabled = false;
            loading.enabled = true;
            SceneManager.LoadScene("Shop");
        }
        textCounter++;
    }
    
}

