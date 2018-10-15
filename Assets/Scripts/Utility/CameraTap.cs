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
    public TutorialGlass tutGlass;
    //Rune Indicator
    public GameObject runeIndicatorPrefab;
    public Canvas mainCanvas;
    private GameObject runeIndicator;
    private bool runeIndicatorEnabled;

    public void Awake() {
        //If top screenRotation was last remembered
        if (topScreenRotation.x == GameManager.Instance.CameraRotTransfer) {
            transform.localEulerAngles = topScreenRotation;
            img.transform.localEulerAngles = topScreenRotationImg;
            topScreen = true;
            if (tutGlass != null)
                tutGlass.Index = 0;
        } else {
            transform.localEulerAngles = bottomScreenRotation;
            img.transform.localEulerAngles = bottomScreenRotationImg;
            topScreen = false;
            if (tutGlass != null)
                tutGlass.Index = 1;
        }
    }

    public void RotateCamera()
    {
        if (GameManager.Instance.canUseTools)
        {
            if (topScreen)
            {
                //SFX.Play("sound");
                SFX.Play("Tap_to_look_DOWN", 1f, 1f, 0f, false, 0f);
                transform.DORotate(bottomScreenRotation, 0.4f).SetEase(Ease.InOutSine);
                img.transform.DORotate(bottomScreenRotationImg, 0.4f).SetEase(Ease.InOutSine);
                GameManager.Instance.CameraRotTransfer = bottomScreenRotation.x;
                topScreen = false;
                if (tutGlass != null)
                    tutGlass.Index = 1;
            }
            else
            {
                //SFX.Play("sound");
                SFX.Play("Tap_to_look_UP", 1f, 1f, 0f, false, 0f);
                transform.DORotate(topScreenRotation, 0.4f).SetEase(Ease.InOutSine);
                img.transform.DORotate(topScreenRotationImg, 0.4f).SetEase(Ease.InOutSine);
                GameManager.Instance.CameraRotTransfer = topScreenRotation.x;
                topScreen = true;
                if (tutGlass != null)
                    tutGlass.Index = 0;
            }

            if (!GameManager.Instance.TutorialIntroTopComplete && GameManager.Instance.InTutorial)
            {
                GameManager.Instance.TutorialIntroTopComplete = true;
                //tutManager.NextDialogue();
                tutManager.HideExposition();
                tutManager.StartToolText();
                tutManager.EnableCameraTap(false);

            }

            if (GameManager.Instance.MineGoleminteractGolem && !topScreen)
            {
                GameManager.Instance.OpenPouch = true;
                tutManager.NextInstruction();
                RemoveHighlight();
                tutManager.HighlightMagnifyerAndResourcePouch();
            }
        }
    }

    public void HighlightButton()
    {
        if (!runeIndicatorEnabled)
        {
            runeIndicator = Instantiate(runeIndicatorPrefab, mainCanvas.transform);
            runeIndicator.GetComponent<TutorialRuneIndicator>().SetPosition(img.gameObject,true);
            runeIndicatorEnabled = true;
        }
    }

    public void RemoveHighlight()
    {
        if (runeIndicator != null)
            Destroy(runeIndicator);

        runeIndicatorEnabled = false;
    }

	public bool AtTopScreen() {
		return topScreen;
	}
}
