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
    public GameObject Button1Group;
    public GameObject Button2Group;

    //Misc Variables
    private int zero = 0;
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

    //Score Variables
    private float accuracyScore = 0;
    private float score = 0;
    public float finalScore = 0;
    public Quality.QualityGrade grade;
    public static Quality.QualityGrade finalGrade;

    //Tming Variables
    public float startTime;
    public float currentTime;
    public float finishTime;
    private float finalTime;
    public float timeLimit = 10.00f;
    private bool startTimer = false;

    // Quality bar.
    public QualityBar qualityBar;

    public ItemDatabase db;

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
        Button1Group.SetActive(false);
        Button2Group.SetActive(false);
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
        accuracyScore = 0;
        totalDistanceAway = 0;
    }

    // Update is called once per frame
    void Update() {
        if (!start)
            return;

        if (canTrace) {
            currentTime = Time.time;
            GetInput();
            finalGrade = grade;
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

                Debug.Log("Adding cube " + ID);
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
            CheckPositions();
            Debug.Log(hitPoints);
            score = CalculateColliderPenalties(CalculateAccuracy(CalculateWin()));
            if (score > 0) {
                /*
                finalScore = CalculateTimeScore(score);
                Debug.Log("Time Score is " + finalScore + " accuracy score is " + score);
                DetermineQuality(finalScore);
                Button1Group.SetActive(true);
                Button2Group.SetActive(true);
                this.GetComponent<UISliderAndBehaviour>().QualityText();
                */
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

        if (Input.GetKeyDown(KeyCode.R)) {
            ResetOptimalPoints();
            SceneManager.LoadScene("Tracing");
        }
    }

    private void GameOver() {
        Countdown.onComplete -= GameOver;
        //finalScore = CalculateTimeScore(score);
        //Debug.Log("Time Score is " + finalScore + " accuracy score is " + score);
        //DetermineQuality(finalScore);
        var grade = qualityBar.Finish();
        qualityText.text = Quality.GradeToString(grade);
        qualityText.color = Quality.GradeToColor(grade);
        qualityText.gameObject.SetActive(true);
        qualityBar.Disappear();
        ResetOptimalPoints();
        Button1Group.SetActive(true);
        Button2Group.SetActive(true);

        // TODO: back to shop button needs to change to facilitate restarting games.
		grade = Quality.CalculateCombinedQuality(DataTransfer.currentQuality, grade);
        Inventory.Instance.InsertItem(new ItemInstance(db.GetActual("Shell"), 1, grade, true));
    }

    private void DetermineQuality(float finalScore) {
        float decimalScore = finalScore / 1000;
        // For transferring quality between scenes.
        if (GameManager.instance) {
            GameManager.instance.UpdateQuality(decimalScore, 1);
        }
        grade = Quality.FloatToGrade(decimalScore, 3);
    }

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
        Debug.Log("avg dist away = " + averageDistanceAway);

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
        if (playerPoints.Count > 0 && playerPoints != null) {
            lastIndex = 0;
            for (int i = 0; i < optimalPoints.Count; i++) {
                Vector3 positionArea = optimalPoints[i];
                foundNumber = false;
                bestDistanceSoFar = maxDistanceAway;
                designatedStartIndex = i;
                for (int j = 0; j < playerPoints.Count; j++) {
                    if (Vector3.Distance(playerPoints[j], positionArea) < maxDistanceAway)
                        if (Vector3.Distance(playerPoints[j], positionArea) <= bestDistanceSoFar && j > lastIndex) {
                            bestDistanceSoFar = Vector3.Distance(playerPoints[j], positionArea);
                            lastIndex = j;
                            foundNumber = true;
                        }
                }
                if (foundNumber) {
                    Debug.Log("Designated start is " + designatedStartIndex);
                    if (optimalPointIndex.Count == 0 && (designatedStartIndex == 0 || designatedStartIndex == 2)) {
                        optimalPointIndex.Add(lastIndex);
                        hitPoints += 1;
                        totalDistanceAway += bestDistanceSoFar;
                    }
                    else if (optimalPointIndex.Count == 2 && (designatedStartIndex == 0 || designatedStartIndex == 2)) {
                        optimalPointIndex.Add(lastIndex);
                        hitPoints += 1;
                        totalDistanceAway += bestDistanceSoFar;
                    }
                    else if (optimalPointIndex.Count == 1 || optimalPointIndex.Count == 3) {
                        optimalPointIndex.Add(lastIndex);
                        hitPoints += 1;
                        totalDistanceAway += bestDistanceSoFar;
                    }
                }
            }

            // TODO: remove previous quality calculation?
            qualityBar.Subtract(totalDistanceAway / hitPoints);
        }
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
}
