﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CameraTap : MonoBehaviour {
    public Vector3 topScreenRotation;
    public Vector3 bottomScreenRotation;
    private bool topScreen = true;

	private Vector3 topScreenRotationImg = new Vector3(0, 0, 0);//Quaternion.Euler(0, 0, 0);// new Vector3(0, 0, 0);
	private Vector3 bottomScreenRotationImg = new Vector3(0, 0, -180);//Quaternion.Euler(0, 0, 180);
	public Image img;

    public void Awake() {
        //If top screenRotation was last remembered
        if (topScreenRotation.x == DataTransfer.cameraRot) {
            transform.localEulerAngles = topScreenRotation;
            topScreen = true;
        } else {
            transform.localEulerAngles = bottomScreenRotation;
            topScreen = false;
        }
    }

    public void RotateCamera() {
        if (topScreen) {
            transform.DORotate(bottomScreenRotation, 0.4f).SetEase(Ease.InOutSine);
			img.transform.DORotate(bottomScreenRotationImg, 0.4f).SetEase(Ease.InOutSine);
            DataTransfer.cameraRot = bottomScreenRotation.x;
            topScreen = false;
        } else { 
            transform.DORotate(topScreenRotation, 0.4f).SetEase(Ease.InOutSine);
			img.transform.DORotate(topScreenRotationImg, 0.4f).SetEase(Ease.InOutSine);
            DataTransfer.cameraRot = topScreenRotation.x;
            topScreen = true;
        }
    }

	public bool AtTopScreen() {
		return topScreen;
	}
}
