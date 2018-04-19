using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracing : MonoBehaviour {
    //Tracing & Vector Lists
    public List<Vector3> playerPoints = new List<Vector3>();
    public List<Vector3> optimalPoints = new List<Vector3>();
    public List<int> optimalPointIndex = new List<int>();
    
    //Cubes represent test positions
    public GameObject[] cubes;
    public Material cubeMaterial;

    //Test Rune
    public GameObject rune1;
    public GameObject rune2;

    //Line Renderer variables
    private LineRenderer lineRenderer;
    public float width;
    public Material material;
    public Color chosenStartColour;
    public Color chosenFinishColour;

    //Misc Variables
    private Camera mainCamera;
    private bool isMouseDown = false;
    private float mouseDownTime;
    public enum Quality {
        NotGraded, F, E, D, C, B, A
    }
    public Quality itemQuality = Quality.NotGraded;

    //Distance and tracking variables
    public int hitPoints = 0;
    public int maxDistanceAway;
    private bool foundNumber = false;
    private int lastIndex = 0;
    private float bestDistanceSoFar;
    private float totalDistanceAway = 0;
    private float averageDistanceAway = 0;

    //Score Variables
    private float accuracyScore = 0;
    private float score = 0;
    public float finalScore = 0;

    //Tming Variables
    public float startTime;
    public float currentTime;
    public float finishTime;
    private float finalTime;
    public float timeLimit = 10.00f;
    


    // Use this for initialization
    void Start() {
        mainCamera = Camera.main;
        SetupLineRenderer();
        //cubes = GameObject.FindGameObjectsWithTag("point");
        foreach (GameObject cube in cubes) {
            cube.GetComponent<Renderer>().material = cubeMaterial;
        }
        foreach (GameObject cube in cubes) {
            cube.GetComponent<Renderer>().material.color = Color.gray;
        }
        startTime = Time.time;
        finishTime = currentTime + timeLimit;

        //if (rune1) {
        GetNecessaryPositions(1);
        // else if rune2
            //GetNecessaryPositions(2);
        //etc etc
    }

    // Update is called once per frame
    void Update() {
        GetInput();
        currentTime = Time.time;
        
    }

    private void SetupLineRenderer() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;
        lineRenderer.startColor = chosenStartColour;
        lineRenderer.endColor = chosenFinishColour;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }
    private void GetNecessaryPositions(int Rune) {
        foreach (GameObject cube in cubes) {
            Vector3 modifiedPosition = transform.TransformPoint(cube.transform.position);
            modifiedPosition.z = 10;
            optimalPoints.Add(modifiedPosition);
        }
    }

    private void GetInput() {
        Vector3 mPosition = Input.mousePosition;
        //mPosition.z = 10;
        Vector3 mWorldPosition = mainCamera.ScreenToWorldPoint(mPosition);
        mWorldPosition.z = 10;

        if (Input.GetMouseButtonDown(0)) {
            isMouseDown = true;
            mouseDownTime = Time.time;
            ResetOptimalPoints();
            hitPoints = 0;
            averageDistanceAway = 0;
            score = 0;
            accuracyScore = 0;
            totalDistanceAway = 0;
        }

        if (Input.GetMouseButtonUp(0)) {
            isMouseDown = false;
            lastIndex = 0;
            CheckPositions();
            score = CalculateAccuracy(CalculateWin());
            Debug.Log("Accuracy Score is " + score);
            if (score > 0) {
                finalScore = CalculateTimeScore(score);
                Debug.Log("final score is " + finalScore);
                DetermineQuality(finalScore);
                //Debug.Log(itemQuality);
            }
        }

        if (isMouseDown && (Time.time - mouseDownTime) > 0.05) {
            if (!playerPoints.Contains(mWorldPosition)) {
                playerPoints.Add(mWorldPosition);
                lineRenderer.positionCount = playerPoints.Count;
                lineRenderer.SetPosition(playerPoints.Count - 1, playerPoints[playerPoints.Count - 1]);
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            ResetOptimalPoints();
        }
    }

    private void DetermineQuality(float finalScore) {
        if (finalScore == 1000) {
            itemQuality = Quality.A;
        } else if (finalScore < 1000 && finalScore >= 835) {
            itemQuality = Quality.B;
        }
        else if (finalScore < 835 && finalScore >= 670) {
            itemQuality = Quality.C;
        }
        else if (finalScore < 670 && finalScore >= 505) {
            itemQuality = Quality.D;
        }
        else if (finalScore < 505 && finalScore >= 340) {
            itemQuality = Quality.E;
        }
        else {
            itemQuality = Quality.F;
        }
    }

    private float CalculateTimeScore(float accuracyScore) {
        float percentageTimeRemaining = ((finishTime - currentTime) / timeLimit) * 100;
        if(percentageTimeRemaining >= 85) {
            return accuracyScore * 1.0f;
        } else if (percentageTimeRemaining < 85 && percentageTimeRemaining >= 70) {
            return accuracyScore * 0.85f;
        } else if (percentageTimeRemaining < 70 && percentageTimeRemaining >= 55) {
            return accuracyScore * 0.7f;
        } else if (percentageTimeRemaining < 55 && percentageTimeRemaining >= 40) {
            return accuracyScore * 0.55f;
        }
        else if (percentageTimeRemaining < 40 && percentageTimeRemaining >= 25) {
            return accuracyScore * 0.40f;
        }
        else if (percentageTimeRemaining < 25 && percentageTimeRemaining >= 0) {
            return accuracyScore * 0.25f;
        } else {
            return 0;
        }
    }

    private int CalculateAccuracy(bool success) {
        if (success) {
            averageDistanceAway = totalDistanceAway / optimalPointIndex.Count;
            Debug.Log("avg dist away = " + averageDistanceAway);
            if (averageDistanceAway >= 0 && averageDistanceAway <= 0.63) {
                return 1000;
            } else if (averageDistanceAway > 0.63 && averageDistanceAway < 0.67) {
                return 850;
            } else if (averageDistanceAway > 0.67 && averageDistanceAway < 0.7) {
                return 700;
            }
            else if (averageDistanceAway > 0.7 && averageDistanceAway < 0.77) {
                return 550;
            }
            else {
                return 400;
            }
        } else {
            return 0;
        }
    }

    private bool CalculateWin() {
        if (optimalPoints.Count != hitPoints || currentTime > finishTime) {
            foreach (GameObject cube in cubes) {
                cube.GetComponent<Renderer>().material.color = Color.red;
            }
            return false;
        }
        else {
            foreach (GameObject cube in cubes) {
                cube.GetComponent<Renderer>().material.color = Color.green;
            }
            return true;
        }
    }

    private void CheckPositions() {
        if (playerPoints.Count > 0 && playerPoints != null) {
            lastIndex = 0;
            for (int i = 0; i < optimalPoints.Count; i++) {
                Vector3 positionArea = optimalPoints[i];
                foundNumber = false;
                bestDistanceSoFar = maxDistanceAway;
                for (int j = 0; j < playerPoints.Count; j++) {
                    if (Vector3.Distance(playerPoints[j], positionArea) < maxDistanceAway)
                            if (Vector3.Distance(playerPoints[j], positionArea) <= bestDistanceSoFar){ 
                                bestDistanceSoFar = Vector3.Distance(playerPoints[j], positionArea);
                                lastIndex = j;
                                foundNumber = true;
                            }
                }
                if (foundNumber) {
                    if (optimalPointIndex.Count == 0) {
                        optimalPointIndex.Add(lastIndex);
                        hitPoints += 1;
                        totalDistanceAway += bestDistanceSoFar;
                    } else if (optimalPointIndex[optimalPointIndex.Count - 1] < lastIndex) {
                        optimalPointIndex.Add(lastIndex);
                        hitPoints += 1;
                        totalDistanceAway += bestDistanceSoFar;
                    }
                } 
            }
        }
    }

    private void ResetOptimalPoints() {
        hitPoints = 0;
        foreach (GameObject cube in cubes) {
            cube.GetComponent<Renderer>().material.color = Color.gray;
        }
        lineRenderer.positionCount = 0;
        playerPoints.RemoveRange(0, playerPoints.Count);
        optimalPointIndex.RemoveRange(0, optimalPointIndex.Count);
        startTime = Time.time;
        finishTime = currentTime + timeLimit;
    }
}
