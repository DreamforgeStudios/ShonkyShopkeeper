using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwipe : MonoBehaviour {
    //Camera Variables
    private Camera mainCamera;
    private Vector3 position1;
    private Vector3 position2;
    private bool startingPosition = true;
    public float rotationAmount;

    //Line Renderer variables
    private LineRenderer lineRenderer;
    public List<Vector3> playerSwipePoints = new List<Vector3>();
    public float width;
    public Material material;
    public Color chosenStartColour;
    public Color chosenFinishColour;

    //Point/touch variables
    private bool mouseDown;

    // Use this for initialization
    void Start () {
        mainCamera = Camera.main;
        SetupLineRenderer();
        position1 = new Vector3(0, 0, 0);
        position2 = new Vector3(rotationAmount, 0, 0);
        //mainCamera.transform.eulerAngles = new Vector3(rotationAmount, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        GetInput();
        //Debug.Log(mainCamera.ScreenToWorldPoint(Input.mousePosition));
	}

    private void GetInput() {
        Vector3 mPosition = Input.mousePosition;
        Vector3 mWorldPosition = mainCamera.ScreenToWorldPoint(mPosition);
        mWorldPosition.z = 10;
        if (Input.GetMouseButton(0)) {
            mouseDown = true;
            
        }
        if (Input.GetMouseButtonUp(0)) {
            mouseDown = false;
            ComparePath(startingPosition);
            ResetOptimalPoints();
        }

        if (mouseDown) {
            if (!playerSwipePoints.Contains(mWorldPosition) && playerSwipePoints.Count <= 40) {
                playerSwipePoints.Add(mWorldPosition);
                lineRenderer.positionCount = playerSwipePoints.Count;
                lineRenderer.SetPosition(playerSwipePoints.Count - 1, playerSwipePoints[playerSwipePoints.Count - 1]);
            }
        }
    }

    private void ComparePath(bool firstScreen) {
        Vector3 startingPoint = playerSwipePoints[0];
        float leeway = 2.0f;
        int validPoints = 0;
        if (firstScreen) {
            for (int i = 0; i < playerSwipePoints.Count; i++) {
                if (Math.Abs((playerSwipePoints[i] - startingPoint).x) <= leeway) {
                    if (playerSwipePoints[i].y >= startingPoint.y) {
                        validPoints += 1;
                    }
                }
            }
            if (validPoints == playerSwipePoints.Count) {
                TransformCamera(firstScreen);
            }
        } else {
            for (int i = 0; i < playerSwipePoints.Count; i++) {
                if (Math.Abs((playerSwipePoints[i] - startingPoint).x) <= leeway) {
                    if (playerSwipePoints[i].y <= startingPoint.y) {
                        validPoints += 1;
                        Debug.Log("Valid points to go up is" + validPoints);
                    }
                }
            }
            if (validPoints == playerSwipePoints.Count) {
                TransformCamera(firstScreen);
            }
        }
    }

    private void TransformCamera(bool firstScreen) {
        if (firstScreen) {
            mainCamera.transform.eulerAngles = position2;
            startingPosition = false;
        } else {
            mainCamera.transform.eulerAngles = position1;
            startingPosition = true;
        }
    }

    private void SetupLineRenderer() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;
        lineRenderer.startColor = chosenStartColour;
        lineRenderer.endColor = chosenFinishColour;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.alignment = LineAlignment.View;
    }

    private void ResetOptimalPoints() {
        lineRenderer.positionCount = 0;
        playerSwipePoints.RemoveRange(0, playerSwipePoints.Count);
    }
}
