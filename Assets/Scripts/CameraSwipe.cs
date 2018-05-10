using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwipe : MonoBehaviour {
    //Camera Variables
    private Camera mainCamera;
    private Vector3 position1;
    private Vector3 position2;
    public bool startingPosition = true;
    public float rotationAmount;
    Quaternion rot1;
    Quaternion rot2;
    public float speed = 0.10f;
    private bool movingCamera = false;

    //Line Renderer variables
    private LineRenderer lineRenderer;
    public List<Vector3> playerSwipePoints = new List<Vector3>();
    public float width;
    public Material material;
    public Color chosenStartColour;
    public Color chosenFinishColour;

    //Point/touch variables
    private bool mouseDown;
    private int minimumPoints = 3;

    //Particle System
    public ParticleSystem particle;
    public int amountOfParticles;
    private ParticleSystem.EmitParams emitParams;

    // Use this for initialization
    void Start () {
        mainCamera = Camera.main;
        SetupLineRenderer();
        rot1 = Camera.main.transform.rotation;
        rot2 = Quaternion.Euler((Camera.main.transform.eulerAngles.x + rotationAmount), 0, 0);
        Debug.Log(Camera.main.transform.eulerAngles.x);
        //position1 = new Vector3(0, 0, 0);
        //position2 = new Vector3(rotationAmount, 0, 0);
        emitParams = new ParticleSystem.EmitParams();
        //mainCamera.transform.eulerAngles = new Vector3(rotationAmount, 0, 0);
    }
	
	// Update is called once per frame
	void Update () {
        GetInput();
        //Debug.Log(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        if (movingCamera)
            TransformCamera(startingPosition);
	}

    private void GetInput() {
        Vector3 mPosition = Input.mousePosition;
        mPosition.z = 10;
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
                emitParams.position = mWorldPosition;
                particle.Emit(emitParams, amountOfParticles);
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
                        if (i == 0) {
                            validPoints += 1;
                        } else if (playerSwipePoints[i].y >= playerSwipePoints[i - 1].y) {
                            validPoints += 1;
                        }
                    }
                }
            }
            if (validPoints == playerSwipePoints.Count && validPoints >= minimumPoints) {
                movingCamera = true;
            }
        } else {
            for (int i = 0; i < playerSwipePoints.Count; i++) {
                if (Math.Abs((playerSwipePoints[i] - startingPoint).x) <= leeway) {
                    if (playerSwipePoints[i].y <= startingPoint.y) {
                        if (i == 0) {
                            validPoints += 1;
                        }
                        else if (playerSwipePoints[i].y <= playerSwipePoints[i - 1].y) {
                            validPoints += 1;
                        }
                    }
                }
            }
            if (validPoints == playerSwipePoints.Count && validPoints >= minimumPoints) {
                movingCamera = true;
            }
        }
    }

    private void TransformCamera(bool firstScreen) {
        if (firstScreen) {
            if(mainCamera.transform.rotation == rot2) {
                startingPosition = false;
                movingCamera = false;
            } else {
                Quaternion rot = Quaternion.Lerp(mainCamera.transform.rotation, rot2, speed);
                //Debug.Log("original slerp is " + rot + " current rotation is " + mainCamera.transform.rotation);
                mainCamera.transform.rotation = rot;
                //mainCamera.transform.rotation = rot2;
            }
        } else {
            if (mainCamera.transform.rotation == rot1) {
                //Debug.Log("rotation = rot 1");
                startingPosition = true;
                movingCamera = false;
            }
            else {
                Quaternion rot = Quaternion.Lerp(mainCamera.transform.rotation, rot1, speed);
                //Debug.Log("Going to rot 1");
                //Debug.Log("slerp is " + rot + " current rotation is " + mainCamera.transform.rotation);
                mainCamera.transform.rotation = rot;
                //mainCamera.transform.rotation = rot1;
            }
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
