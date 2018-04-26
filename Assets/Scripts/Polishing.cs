using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polishing : MonoBehaviour {

    private Camera mainCamera;
    public GameObject gemObject;
    public int numberOfSwipes = 0;
    public static Quality.QualityGrade finalGrade;
    public float timeLimit;
    private bool isMouseDown;
    private float startTime;
    private float currentTime;
    private float finishTime;
    private bool coroutineRunning = false;
    private Vector3 mWorldPosition;

    //Vector to swipe over
    private Vector3 keyPoint;

    //Vectors to track number of swipes
    private Vector3 leftSide;
    private Vector3 rightSide;

    // Use this for initialization
    void Start() {
        mainCamera = Camera.main;
        keyPoint = gemObject.transform.position;
    }

    // Update is called once per frame
    void Update() {
        GetInput();
    }

    private void GetInput() {
        Vector3 mPosition = Input.mousePosition;
        mWorldPosition = mainCamera.ScreenToWorldPoint(mPosition);
        currentTime = Time.time;
        if (Input.GetMouseButtonDown(0)) {
            isMouseDown = true;
            startTime = Time.time;
            finishTime = currentTime + timeLimit;
            coroutineRunning = false;
        }

        if (Input.GetMouseButtonUp(0)) {
            isMouseDown = false;
        }

        if (isMouseDown) {
            Debug.Log(numberOfSwipes);
            if (mWorldPosition.x < keyPoint.x && !coroutineRunning) {
                StartCoroutine(CalculateSwipes(true));
                coroutineRunning = true;
            }
            else if (mWorldPosition.x > keyPoint.x && !coroutineRunning){
                StartCoroutine(CalculateSwipes(false));
                coroutineRunning = true;
            }
        }
    }
    //Calculate number of swipes
    IEnumerator CalculateSwipes(bool leftSideStart) {
        while (currentTime < finishTime) {
            if (leftSideStart) {
                if (mWorldPosition.x > keyPoint.x) {
                    numberOfSwipes++;
                    leftSideStart = false;
                }
            } else {
                if (mWorldPosition.x < keyPoint.x) {
                    numberOfSwipes++;
                    leftSideStart = true;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        StopCoroutine(CalculateSwipes(false));
    }
}
