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

    public TutorialManager tutManager;

    public void Awake() {
        //If top screenRotation was last remembered
        if (topScreenRotation.x == GameManager.Instance.CameraRotTransfer) {
            transform.localEulerAngles = topScreenRotation;
            img.transform.localEulerAngles = topScreenRotationImg;
            topScreen = true;
        } else {
            transform.localEulerAngles = bottomScreenRotation;
            img.transform.localEulerAngles = bottomScreenRotationImg;
            topScreen = false;
        }
    }

    public void RotateCamera() {
        if (topScreen) {
            //SFX.Play("sound");
            SFX.Play("Tap_to_look_DOWN", 1f, 1f, 0f, false, 0f);
            transform.DORotate(bottomScreenRotation, 0.4f).SetEase(Ease.InOutSine);
			img.transform.DORotate(bottomScreenRotationImg, 0.4f).SetEase(Ease.InOutSine);
            GameManager.Instance.CameraRotTransfer = bottomScreenRotation.x;
            topScreen = false;
        } else {
            //SFX.Play("sound");
            SFX.Play("Tap_to_look_UP", 1f, 1f, 0f, false, 0f);
            transform.DORotate(topScreenRotation, 0.4f).SetEase(Ease.InOutSine);
			img.transform.DORotate(topScreenRotationImg, 0.4f).SetEase(Ease.InOutSine);
            GameManager.Instance.CameraRotTransfer = topScreenRotation.x;
            topScreen = true;
        }

        if (!GameManager.Instance.TutorialIntroTopComplete && GameManager.Instance.InTutorial)
        {
            GameManager.Instance.TutorialIntroTopComplete = true;
            tutManager.NextDialogue();
            tutManager.StartForcepParticles();
            if (GameManager.Instance.TutorialIntroComplete)
                tutManager.HideCanvas();
        }
            
    }

	public bool AtTopScreen() {
		return topScreen;
	}
}
