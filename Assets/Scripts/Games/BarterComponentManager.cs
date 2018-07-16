﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class BarterComponentManager : MonoBehaviour {
    public GameObject background;
    private Material backgroundMat;
    public TextMeshProUGUI txtPrice;
    public GameObject priceHolder;
    public TextMeshProUGUI txtDialogue;
    public SpriteRenderer wizardSprite;
    public TextMeshProUGUI infoText;
    public ItemInstance shonkyInstance;

    public Button buttonAccept;
    public Button buttonOffer;
    //public GameObject shonkyPrefab;
    //public Text txtDebug;
    //private float prevPlayerOffer; // debug.

    public float dragMultiplier = 0.1f;
    public float dragVelocityMultiplier = 1.0f;
    public float priceChangeMultiplier = 0.5f;
    public float dragTimeMultiplier = 1.0f;
    public Ease ease;

    private float dx = 0f;
    // So that fPrice can be accessed from barter.
    public float fPrice = 100f;
    private float basePrice;
    private int iPrice = 100;

    private float prevXPos = 0f;

    private bool wheelActive = true;

    private Vector2 offset;

	// Use this for initialization
	void Start () {
        this.prevXPos = this.background.transform.position.x;
        this.backgroundMat = background.GetComponent<MeshRenderer>().material;
        this.offset = new Vector2(0, 0);
        if (GameManager.Instance.SpriteTransfer) {
            this.wizardSprite.sprite = GameManager.Instance.SpriteTransfer;
        } else {
            Debug.Log("Couldn't find a sprite, will use default.");
        }

        this.basePrice = (shonkyInstance.item as Shonky).basePrice;
        string grade = Quality.GradeToString(shonkyInstance.Quality);
        string gradeCol = "#" + ColorUtility.ToHtmlStringRGB(Quality.GradeToColor(shonkyInstance.Quality));
        string str = string.Format("Base Price: <color=white>{0}</color>\n" +
                                   "Quality: <color={1}>{2}</color>\n", basePrice, gradeCol, grade);
        infoText.text = str;

        //Instantiate(shonkyPrefab, Vector3.zero, Quaternion.identity).transform.SetParent(backingPlate.transform);
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

		UpdateText();
	}

    private void ProcessMouse() {
        if (Input.GetMouseButtonDown(0)) {
            OnDown();
            prevXPos = Input.mousePosition.x;
        } else if (Input.GetMouseButton(0)) {
            OnHold(prevXPos - Input.mousePosition.x);
            prevXPos = Input.mousePosition.x;
		} else if (Input.GetMouseButtonUp(0)) {
            OnUp();
		}
    }

    private void ProcessTouch() {
		// Don't let the player use multiple fingers, and don't run if there's no input.
		if (Input.touchCount > 1 || Input.touchCount == 0) {
			return;
		}

		Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began) {
            OnDown();
		} else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
            OnUp();
		} else {
            OnHold(-touch.deltaPosition.x);
		}
    }

    public void MoveSliderBack(float val) {
        // Tween the price ticker.
		DOTween.To(() => fPrice, x => fPrice = x, fPrice - val, (val * dragTimeMultiplier)/100f).SetEase(ease);

        // TODO ugly...
        backgroundMat.DOOffset(new Vector2(offset.x - (val * dragVelocityMultiplier), 0),
            (Mathf.Abs(val) * dragTimeMultiplier) / 100f).SetEase(ease);
    }

    public void WizardPunch(float strength, float duration) {
		wizardSprite.transform.DOPunchRotation(Vector3.forward * 25 * strength, duration, 30).SetEase(Ease.OutQuint);
    }

    public void WizardSpeak(string txt) {
        txtDialogue.text = txt;
    }

    private void OnDown() {
		if (!wheelActive) return;

        // Set velocity to zero.
		dx = 0f;

        // Complete active tweens for the transform.
        backgroundMat.DOComplete();
        DOTween.CompleteAll();
    }

    private void OnHold(float delta) {
		if (!wheelActive) return;

        // Calculate difference we should move.
		dx = delta;
        // Calculate the amount we should add based on the distance.
		fPrice += dx * (priceChangeMultiplier / 100f);

        // Manually move the bar while holding.
		//background.transform.position -= new Vector3(dx * dragMultiplier, 0f, 0f);
        offset.x = offset.x + dx * (dragMultiplier / 100f);
        backgroundMat.SetTextureOffset("_MainTex", offset);
    }

    // TODO: lots of magic numbers...
    private void OnUp() {
		if (!wheelActive) return;

        // Tween the price ticker.
		DOTween.To(() => fPrice, x => fPrice = x, fPrice + (dx * (dragVelocityMultiplier/100f)), (Mathf.Abs(dx) * dragTimeMultiplier)/100f).SetEase(ease);

        //TODO: don't kick in until later.
        backgroundMat.DOOffset(new Vector2(offset.x + (dx * dragVelocityMultiplier/100f), 0),
            (Mathf.Abs(dx) * dragTimeMultiplier) / 100f).SetEase(ease);
    }

	private void UpdateText() {
        // Dirty guard.
        if (fPrice < 0) {
            fPrice = 0;
        }

        // If the new price has ticked over...
        int newPrice = (int)fPrice;
        if (newPrice != iPrice) {
            // Complete current tween.
		    priceHolder.transform.DOComplete();
            // Shake the text to indicate a tick.
		    priceHolder.transform.DOPunchRotation(Vector3.forward * 6, 0.4f, 18);
        }

        iPrice = newPrice;

        float profit = (fPrice / basePrice) - 1f;
        string colorstring = "";
        if (profit < 0) {
            colorstring = "<color=red>";
        } else {
            colorstring = "<color=green>+";
        }
		txtPrice.text = iPrice.ToString() + colorstring + profit.ToString("#0.#%") + "</color>";
	}

    public void SetBasePrice(float price) {
        this.basePrice = price;
    }
}
