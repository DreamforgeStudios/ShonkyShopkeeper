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
    public PointsManager pointsManager;
    public GemSpawnManager GemSpawnManager;
    public InstructionHandler InstructionManager;

    //Time Variables
    private float startTime;
    private float currentTime;
    private float finishTime;
    private float missDurationCounter;
    public float timeLimit;
    public float missDurationTimeout;

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
    //public TextMeshProUGUI text;
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
    
    //Quick and dirty sound tracking
    private bool pulse1, pulse2, pulse3 = false;

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
        SFX.Play("CraftingGem",1f,1f,0f,true,0f);
        mainCamera = Camera.main;
        //keyPoint = gemObject.transform.position;
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
            CalculateRelevantSFX();
            
            if (missDurationCounter > missDurationTimeout) {
                missDurationCounter = 0;
                InstructionManager.PushInstruction();
            }

            missDurationCounter += Time.deltaTime;
        } else {
            //GameOver();
        }
    }

    private void ObjectColourLerp() {
        //gemObject.GetComponent<Renderer>().material.color = Color.Lerp(colourStart, ColourEnd, (numberOfSwipes + 1) / (timeLimit * 10));
    }

    //Really quick and dirty
    private void CalculateRelevantSFX()
    {
        // Integrating this is a bit annoying, and probably not very efficient.
        Quality.QualityGrade grade = Quality.CalculateGradeFromPoints(pointsManager.GetPoints());
        //Debug.Log("grade is " + grade);
        if (grade == Quality.QualityGrade.Junk || grade == Quality.QualityGrade.Brittle)
        {
            if (!pulse1)
            {
                Debug.Log("Playing pulse 1");
                pulse1 = true;
                SFX.Play("Polishing_Pulse1", 1f, 1f, 0f, true, 0f);
            }

            if (pulse2)
            {
                SFX.StopSpecific("Polishing_Pulse2");
                pulse2 = false;
            }

            if (pulse3)
            {
                SFX.StopSpecific("Polishing_Pulse3");
                pulse3 = false;
            }

        } else if (grade == Quality.QualityGrade.Passable || grade == Quality.QualityGrade.Sturdy)
        {
            if (pulse1)
            {
                pulse1 = false;
                SFX.StopSpecific("Polishing_Pulse1");
            }

            if (!pulse2)
            {
                Debug.Log("Playing pulse 2");
                SFX.Play("Polishing_Pulse2",1f,1f,0f,true,0f);
                pulse2 = true;
            }

            if (pulse3)
            {
                SFX.StopSpecific("Polishing_Pulse3");
                pulse3 = false;
            } 
        }
        else
        {
            if (pulse1)
            {
                pulse1 = false;
                SFX.StopSpecific("Polishing_Pulse1");
            }

            if (pulse2)
            {
                SFX.StopSpecific("Polishing_Pulse2");
                pulse2 = false;
            }

            if (!pulse3)
            {
                Debug.Log("Playing pulse 3");
                SFX.Play("Polishing_Pulse3",1f,1f,0f,true,0f);
                pulse3 = true;
            } 
        }
    }

    private void GetInput()
    {
        mWorldPosition = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(15f);
        currentTime = Time.time;
        //text.text = "Swipes: " + numberOfSwipes;
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
            //Debug.Log(numberOfSwipes);
            Debug.Log("mouse world position x is" + mWorldPosition.x + "target is " + keyPoint.x);
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
        while (true)
        {
            //Debug.Log("left side start is " + leftSideStart);
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

    private void AddSwipe()
    {
        SFX.Play("Polish_swipe", 1f, 1f, 0f, false, 0f);
        numberOfSwipes++;
        pointsManager.AddPoints(swipeContribution);
        missDurationCounter = 0;
    }

	private Quality.QualityGrade grade = Quality.QualityGrade.Unset;
    private void GameOver() {
        StopCoroutine(CalculateSwipes(false));
        Countdown.onComplete -= GameOver;
        //if (gameOver) {
        //CalculateGrade();
        gameOver = true;

        var tmpGrade = Quality.CalculateGradeFromPoints(pointsManager.GetPoints());
        pointsManager.onFinishLeveling += () =>
        {
            GemSpawnManager.UpgradeGem(tmpGrade);
            pointsManager.gameObject.SetActive(false);
            qualityText.text = Quality.GradeToString(tmpGrade);
            qualityText.color = Quality.GradeToColor(tmpGrade);
            qualityText.gameObject.SetActive(true);
        };
            
        pointsManager.DoEndGameTransition();

        // Combine grade at the end for when we return to shop.
		grade = Quality.CalculateCombinedQuality(GameManager.Instance.QualityTransfer, tmpGrade);

        ShowUIButtons();
    }
    
    public void JunkReturn()
    {
        ReturnOrRetry.Return("Charged " + GameManager.Instance.GemTypeTransfer, grade);
    }

    public void Return() {
        if (grade != Quality.QualityGrade.Junk)
            ReturnOrRetry.Return("Charged " + GameManager.Instance.GemTypeTransfer, grade);
        else
            returnOrRetryButtons.GetComponent<UpdateRetryButton>().WarningTextEnable();
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
