using System.Collections;
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

    public void RotateCamera() {
        if (topScreen) {
            transform.DORotate(bottomScreenRotation, 0.4f).SetEase(Ease.InOutSine);
			img.transform.DORotate(bottomScreenRotationImg, 0.4f).SetEase(Ease.InOutSine);
            topScreen = false;
        } else { 
            transform.DORotate(topScreenRotation, 0.4f).SetEase(Ease.InOutSine);
			img.transform.DORotate(topScreenRotationImg, 0.4f).SetEase(Ease.InOutSine);
            topScreen = true;
        }
    }

	public bool AtTopScreen() {
		return topScreen;
	}
}
