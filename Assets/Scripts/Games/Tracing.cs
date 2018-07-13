using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Tracing : MonoBehaviour {
    public TextMeshProUGUI qualityText;

    //Tracing & Vector Lists
    public List<Vector3> playerPoints = new List<Vector3>();
    public List<Vector3> rune1 = new List<Vector3>();
    public List<Vector3> optimalPoints = new List<Vector3>();
    public List<int> optimalPointIndex = new List<int>();

    //This list represents the points at which the player can start
    public List<int> startPoints = new List<int>();

    //Cubes represent test positions
    public Material cubeMaterial;

    //Test Rune
    public GameObject[] cubeRune1;

    //Sphere that follows the player's finger
    public GameObject followSphere;

    //GameObject with all the colliders that cannot be hit.
    public GameObject rune1Colliders;

    //Line Renderer variables
    public LineRenderer lineRenderer;
    public float width;
    public Material material;
    public Color chosenStartColour;
    public Color chosenFinishColour;

    //Misc Variables
    //private int zero = 0;
    private bool canTrace = false;
    private Camera mainCamera;
    private bool isMouseDown = false;
    private float mouseDownTime;
    private SceneManager sceneManager;
    private int designatedStartIndex;

    //Distance and tracking variables
    public int hitPoints = 0;
    public float maxDistanceAway;
    private bool foundNumber = false;
    private int lastIndex = 0;
    private float bestDistanceSoFar;
    private float totalDistanceAway = 0;
    private float averageDistanceAway = 0;
    private int numberOfTouches = 0;
    private bool correctStart = false;

    //Score Variables
    private float score = 0;
    public float finalScore = 0;

    //Tming Variables
    public float startTime;
    public float currentTime;
    public float finishTime;
    private float finalTime;
    public float timeLimit = 10.00f;
    private bool startTimer = false;

    // Quality bar.
    public QualityBar qualityBar;
    public GameObject returnOrRetryButtons;

    private bool start = false;

    void Awake() {
        // Don't start until we're ready.
        Time.timeScale = 0;
        ReadyGo.onComplete += (() => { Time.timeScale = 1; start = true; });
    }

    // Use this for initialization
    void Start() {
        Countdown.onComplete += GameOver;
        mainCamera = Camera.main;
        SetupLineRenderer();
        GetNecessaryPositions(1);
        followSphere.SetActive(false);
        canTrace = true;
        Vector3 colliderTransform;
        colliderTransform = rune1Colliders.transform.position;
        colliderTransform.z = 10;
        rune1Colliders.transform.position = colliderTransform;
        hitPoints = 0;
        averageDistanceAway = 0;
        score = 0;
        totalDistanceAway = 0;
        DesignateStartPoints();
    }

    // Update is called once per frame
    void Update() {
        if (!start)
            return;

        if (canTrace) {
            currentTime = Time.time;
            GetInput();
        }
    }

    //Helper method to showcase optimal points
    private void DrawOptimalLines() {
        int ID = 0;
        foreach (Vector3 position in rune1) {
            if (!playerPoints.Contains(position)) {
                playerPoints.Add(position);
                lineRenderer.positionCount = playerPoints.Count;
                lineRenderer.SetPosition(playerPoints.Count - 1, playerPoints[playerPoints.Count - 1]);
                ID++;
            }
        }
    }

    private void SetupLineRenderer() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.positionCount = 0;
        lineRenderer.startColor = chosenStartColour;
        lineRenderer.endColor = chosenFinishColour;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.sortingLayerName = "LineRenderer";
    }
    private void GetNecessaryPositions(int Rune) {
        if (Rune == 1) {
            foreach (GameObject cube in cubeRune1) {
                Vector3 position = cube.transform.position;
                position.z = 10;
                optimalPoints.Add(position);

            }
        }
        DrawOptimalLines();
    }

    private void GetInput() {
        Vector3 mPosition = Input.mousePosition;
        Vector3 mWorldPosition = mainCamera.ScreenToWorldPoint(mPosition);
        mWorldPosition.z = 10;
        followSphere.transform.position = mWorldPosition;

        if (Input.GetMouseButtonDown(0)) {
            isMouseDown = true;
            mouseDownTime = Time.time;
            numberOfTouches++;
            if (!startTimer) {
                startTime = Time.time;
                finishTime = currentTime + timeLimit;
                startTimer = true;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            isMouseDown = false;
            followSphere.SetActive(false);
            if (numberOfTouches >= 3) {
                ResetOptimalPoints();
            }
            correctStart = false;
            hitPoints = 0;
            CheckPositions();
            score = CalculateColliderPenalties(CalculateAccuracy(CalculateWin()));
            if (score > 0) {
                GameOver();
            }
        }

        if (isMouseDown && (Time.time - mouseDownTime) > 0.02 && numberOfTouches < 3) {
            if (!playerPoints.Contains(mWorldPosition)) {
                followSphere.SetActive(true);
                playerPoints.Add(mWorldPosition);
                lineRenderer.positionCount = playerPoints.Count;
                lineRenderer.SetPosition(playerPoints.Count - 1, playerPoints[playerPoints.Count - 1]);
            }
        }

    }

	public Quality.QualityGrade grade = Quality.QualityGrade.Unset;
    private void GameOver() {
        Countdown.onComplete -= GameOver;
        grade = qualityBar.Finish();
        qualityText.text = Quality.GradeToString(grade);
        qualityText.color = Quality.GradeToColor(grade);
        qualityText.gameObject.SetActive(true);
        qualityBar.Disappear();
        ResetOptimalPoints();

        // TODO: back to shop button needs to change to facilitate restarting games.
		grade = Quality.CalculateCombinedQuality(DataTransfer.currentQuality, grade);
        //Inventory.Instance.InsertItem(new ItemInstance("Shell", 1, grade, true));
        
        ShowUIButtons();
    }

    /*
    private void DetermineQuality(float finalScore) {
        float decimalScore = finalScore / 1000;
        // For transferring quality between scenes.
        if (GameManager.instance) {
            GameManager.instance.UpdateQuality(decimalScore, 1);
        }
        grade = Quality.FloatToGrade(decimalScore, 3);
    }
    */

    private float CalculateTimeScore(float accuracyScore) {
        float percentageTimeRemaining = ((finishTime - currentTime) / timeLimit) * 100;
        Debug.Log(percentageTimeRemaining);
        if (percentageTimeRemaining >= 85) {
            return accuracyScore * 1.0f;
        }
        else if (percentageTimeRemaining < 85 && percentageTimeRemaining >= 70) {
            return accuracyScore * 0.85f;
        }
        else if (percentageTimeRemaining < 70 && percentageTimeRemaining >= 55) {
            return accuracyScore * 0.7f;
        }
        else if (percentageTimeRemaining < 55 && percentageTimeRemaining >= 40) {
            return accuracyScore * 0.55f;
        }
        else if (percentageTimeRemaining < 40 && percentageTimeRemaining >= 25) {
            return accuracyScore * 0.40f;
        }
        else if (percentageTimeRemaining < 25 && percentageTimeRemaining >= 0) {
            return accuracyScore * 0.25f;
        }
        else {
            return 0;
        }
    }

    private int CalculateAccuracy(bool success) {
        averageDistanceAway = totalDistanceAway / hitPoints;// optimalPointIndex.Count;
        //Debug.Log("avg dist away = " + averageDistanceAway);

        if (success) {
            averageDistanceAway = totalDistanceAway / optimalPointIndex.Count;
            Debug.Log("avg dist away = " + averageDistanceAway);
            if (averageDistanceAway >= 0 && averageDistanceAway <= 0.63) {
                return 1000;
            }
            else if (averageDistanceAway > 0.63 && averageDistanceAway < 0.67) {
                return 850;
            }
            else if (averageDistanceAway > 0.67 && averageDistanceAway < 0.7) {
                return 700;
            }
            else if (averageDistanceAway > 0.7 && averageDistanceAway < 0.77) {
                return 550;
            }
            else {
                return 400;
            }
        }
        else {
            return 0;
        }
    }

    private int CalculateColliderPenalties(int score) {
        int colliderHits = followSphere.GetComponent<TracingColliding>().counter;
        //Debug.Log("collider hits: " + colliderHits);
        // TODO: this is a bit rough...
        qualityBar.Subtract(colliderHits * 0.1f);
        if (colliderHits == 0)
            return score;
        else if (colliderHits < 10)
            return score - (score / 4);
        else
            return score / 5;
    }

    private bool CalculateWin() {
        if (optimalPoints.Count != hitPoints || currentTime > finishTime) {
            return false;
        }
        else {
            return true;
        }
    }
    //Check positions for Rune 1. This is a good system for runes that use 2 pieces
    private void CheckPositions() {
        lastIndex = 0;
        if (playerPoints.Count > 0 && playerPoints != null) {
            for (int i = 0; i < optimalPoints.Count; i++) {
                //Find the player point which is closest to this point if any
                if (FoundClosestPlayerPoint(i)) {
                    //Player can only start at specific points on each swipe
                    if (startPoints.Contains(i)) {
                        AddPoint();
                        correctStart = true;
                    }
                    else if (correctStart) {
                        AddPoint();
                    }
                }
            }

            // TODO: remove previous quality calculation?
            qualityBar.Subtract(totalDistanceAway / hitPoints);
        }
    }

    private void AddPoint() {
        Debug.Log("Optimal points is " + optimalPoints.Count + " hitpoints is " + hitPoints + "Last index was " + lastIndex);
        optimalPointIndex.Add(lastIndex);
        hitPoints += 1;
        totalDistanceAway += bestDistanceSoFar;
        if (correctStart) {
            correctStart = false;
        }
    }

    private bool FoundClosestPlayerPoint(int currentPointToCompare) {
        Vector3 positionArea = optimalPoints[currentPointToCompare];
        foundNumber = false;
        bestDistanceSoFar = maxDistanceAway;
        for (int j = 0; j < playerPoints.Count; j++) {
            if (Vector3.Distance(playerPoints[j], positionArea) < maxDistanceAway)
                if (Vector3.Distance(playerPoints[j], positionArea) <= bestDistanceSoFar && j > lastIndex) {
                    bestDistanceSoFar = Vector3.Distance(playerPoints[j], positionArea);
                    lastIndex = j;
                    foundNumber = true;
                }
        }
        if (foundNumber)
            return true;
        else
            return false;
    }

    private void ResetOptimalPoints() {
        hitPoints = 0;
        lineRenderer.positionCount = 0;
        playerPoints.RemoveRange(0, playerPoints.Count);
        optimalPointIndex.RemoveRange(0, optimalPointIndex.Count);
        startTime = Time.time;
        finishTime = currentTime + timeLimit;
        numberOfTouches = 0;
    }

    private void DesignateStartPoints() {
        startPoints.Add(0);
        startPoints.Add(2);
    }

    public void Return() {
		ReturnOrRetry.Return("Shell", grade);
	}

	public void Retry() {
		ReturnOrRetry.Retry();
	}

    public void ShowUIButtons() {
	    returnOrRetryButtons.SetActive(true);
        returnOrRetryButtons.GetComponent<UpdateRetryButton>().SetText();
    }

   
}
