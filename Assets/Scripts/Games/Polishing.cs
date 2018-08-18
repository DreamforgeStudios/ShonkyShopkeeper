using System;
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
    public QualityBar qualityBar;
    public GemSpawnManager GemSpawnManager;

    //Need to get the correct material based on gemtype passed in
    public Material Ruby;
    public Material Sapphire;
    public Material Emerald;
    public Material blackOutlines;

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
    public GameObject returnOrRetryButtons;

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
    //public ItemDatabase db;

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
        Material[] materials = new Material[2];
        //Determine gem material
        switch (GameManager.Instance.GemTypeTransfer.ToString()) {
            case ("Ruby"):
                materials[0] = blackOutlines;
                materials[1] = Ruby;
                gemObject.GetComponent<Renderer>().materials = materials;
                break;
            case ("Sapphire"):
                materials[0] = blackOutlines;
                materials[1] = Sapphire;
                gemObject.GetComponent<Renderer>().materials = materials;
                break;
            case ("Emerald"):
                materials[0] = blackOutlines;
                materials[1] = Emerald;
                gemObject.GetComponent<Renderer>().materials = materials;
                break;
            //case ("Diamond"):
              //  gemObject.GetComponent<Renderer>().material = Ruby;
              //  break;

        }
        //nextScene.enabled = false;
        //retryScene.enabled = false;
        //qualityText.enabled = false;
        emitParams = new ParticleSystem.EmitParams();
        //SFX.Play("sound");
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
        //gemObject.GetComponent<Renderer>().material.color = Color.Lerp(colourStart, ColourEnd, (numberOfSwipes + 1) / (timeLimit * 10));
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
        //SFX.Play("sound");
        numberOfSwipes++;
        qualityBar.Add(swipeContribution);
    }

	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
    private void GameOver() {
        StopCoroutine(CalculateSwipes(false));
        Countdown.onComplete -= GameOver;
        //if (gameOver) {
        //CalculateGrade();
        gameOver = true;
        grade = qualityBar.Finish();
        qualityText.text = Quality.GradeToString(grade);
        qualityText.color = Quality.GradeToColor(grade);
        qualityText.gameObject.SetActive(true);
        qualityBar.Disappear();

		grade = Quality.CalculateCombinedQuality(GameManager.Instance.QualityTransfer, grade);
        
        GemSpawnManager.UpgradeGem();

        ShowUIButtons();
    }
    
    // Return to shop.
	public void Return() {
		ReturnOrRetry.Return("Charged " + GameManager.Instance.GemTypeTransfer, grade);
	}

    // Retry (roload scene).
	public void Retry() {
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    returnOrRetryButtons.SetActive(true);
        returnOrRetryButtons.GetComponent<UpdateRetryButton>().SetText();
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
