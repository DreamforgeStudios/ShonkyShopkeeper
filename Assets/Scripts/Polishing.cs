﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Polishing : MonoBehaviour {
    //Misc variables and objects
    private Camera mainCamera;
    public GameObject gemObject;
    public Color colourStart;
    public Color ColourEnd;

    public QualityBar qualityBar;

    //Time Variables
    private float startTime;
    private float currentTime;
    private float finishTime;
    public float timeLimit;

    //Score and Grading
    public int numberOfSwipes = 0;
    public static Quality.QualityGrade finalGrade;

    // Hwom uch will swipes contribute to the grade increasing.
    public float swipeContribution;

    //State Tracking
    private bool isMouseDown;
    private bool coroutineRunning = false;
    private bool gameOver = false;
    private Vector3 mWorldPosition;

    //UI elements
    public TextMeshProUGUI text;
    public TextMeshProUGUI qualityText;
    public Slider timerSlider;
    public Image sliderImage;
    public Button nextScene;
    public Button retryScene;
    public GameObject nextButtonGroup;
    public GameObject retrySceneGroup;

    //Vector to swipe over
    private Vector3 keyPoint;

    //Vectors to track number of swipes
    private Vector3 leftSide;
    private Vector3 rightSide;

    //Particle System
    public ParticleSystem particle;
    public int amountOfParticles;
    private ParticleSystem.EmitParams emitParams;

    // DB.
    public ItemDatabase db;

    private bool start = false;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += (() => { Time.timeScale = 1; start = true; });
    }

    // Use this for initialization
    void Start() {
        mainCamera = Camera.main;
        keyPoint = gemObject.transform.position;
        //nextScene.enabled = false;
        //retryScene.enabled = false;
        //qualityText.enabled = false;
        emitParams = new ParticleSystem.EmitParams();
        Countdown.onComplete += GameOver;
    }

    // Update is called once per frame
    void Update() {
        if (!start)
            return;

        if (!gameOver) {
            GetInput();
            ObjectColourLerp();
            timerSlider.value = currentTime;
            timerSlider.minValue = startTime;
            timerSlider.maxValue = finishTime;
            Color sliderColour = Color.Lerp(Color.green, Color.red, timerSlider.value / timerSlider.maxValue);
            sliderImage.color = sliderColour;
        } else {
            //GameOver();
        }
    }

    private void ObjectColourLerp() {
        gemObject.GetComponent<Renderer>().material.color = Color.Lerp(colourStart, ColourEnd, (numberOfSwipes + 1) / (timeLimit * 10));
    }

    private void GetInput() {
        Vector3 mPosition = Input.mousePosition;
        mWorldPosition = mainCamera.ScreenToWorldPoint(mPosition);
        currentTime = Time.time;
        text.text = "Swipes: " + numberOfSwipes;
        //if (Input.touchCount > 0) {
        if (Input.GetMouseButtonDown(0)) {
            isMouseDown = true;
            if (!coroutineRunning) {
                startTime = Time.time;
                finishTime = currentTime + timeLimit;
                coroutineRunning = false;
            }
        }
        else {
            isMouseDown = false;
        }

        if (isMouseDown) {
            Debug.Log(numberOfSwipes);
            if (mWorldPosition.x < keyPoint.x && !coroutineRunning) {
                StartCoroutine(CalculateSwipes(true));
                coroutineRunning = true;
            }
            else if (mWorldPosition.x > keyPoint.x && !coroutineRunning) {
                StartCoroutine(CalculateSwipes(false));
                coroutineRunning = true;
            }
        }
    }
    //Calculate number of swipes
    IEnumerator CalculateSwipes(bool leftSideStart) {
        //while (currentTime < finishTime) {
        // A dirty fix...
        while (true) {
            //for (int i = 0; i < Input.touchCount; i++) {
            if (leftSideStart) {
                if (mWorldPosition.x > keyPoint.x) {
                    particle.Emit(20);
                    //numberOfSwipes++;
                    AddSwipe();
                    leftSideStart = false;
                }
            }
            else {
                if (mWorldPosition.x < keyPoint.x) {
                    emitParams.startLifetime = 2.0f;
                    particle.Emit(20);
                    
                    //numberOfSwipes++;
                    AddSwipe();
                    leftSideStart = true;
                }
            }
            yield return new WaitForSeconds(0.025f);
        }
        //coroutineRunning = false;
        //gameOver = true;
        //StopCoroutine(CalculateSwipes(false));
    }

    private void AddSwipe() {
        numberOfSwipes++;
        qualityBar.Add(swipeContribution);
    }

    private void GameOver() {
        StopCoroutine(CalculateSwipes(false));
        Countdown.onComplete -= GameOver;
        //if (gameOver) {
        //CalculateGrade();
        gameOver = true;
        nextButtonGroup.SetActive(true);
        retrySceneGroup.SetActive(true);
        var grade = qualityBar.Finish();
        qualityText.text = Quality.GradeToString(grade);
        qualityText.color = Quality.GradeToColor(grade);
        qualityText.gameObject.SetActive(true);
        qualityBar.Disappear();

        // TODO: back to shop button needs to change to facilitate restarting games.
        Inventory.Instance.InsertItem(new ItemInstance(db.GetActual("Charged " + DataTransfer.GemType), 1, grade, true));
		SaveManager save = ScriptableObject.CreateInstance<SaveManager>();
		save.SaveInventory();

        //nextScene.enabled = true;
        //retryScene.enabled = true;
        //}
    }

/*
    private void CalculateGrade() {
        float finalScore = numberOfSwipes / timeLimit;
        if (finalScore >= 10) {
            finalGrade = Quality.FloatToGrade(1f, 3);
        }
        else if (finalScore >= 8.5 && finalScore < 10) {
            finalGrade = Quality.FloatToGrade(0.85f, 3);
        }
        else if (finalScore < 8.5 && finalScore > 5) {
            finalGrade = Quality.FloatToGrade(0.5f, 3);
        }
        else {
            finalGrade = Quality.FloatToGrade(0, 3);
        }

        // For transfering quality between scenes.
        if (GameManager.instance) {
            // This should probably be calculated better.
            GameManager.instance.UpdateQuality((numberOfSwipes / timeLimit) / 10f, 3);
        }
    }
    */
}
