using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PortalSpin : MonoBehaviour {
    //Portal Visual Behaviour
    public SpriteRenderer entryPortal, exitPortal;
    private Transform t1, t2;
    private Quaternion entrySpin1 = Quaternion.Euler(162.0f, -80.5f, -15.6f);
    private Quaternion entrySpin2 = Quaternion.Euler(180.3f, -75f, 19.39f);
    private Quaternion exitSpin1 = Quaternion.Euler(0f, 0f, 11.86f);
    private Quaternion exitSpin2 = Quaternion.Euler(0f, 0f, -2f);
    private Sequence entrySeq, exitSeq;

	// Use this for initialization
	void Start () {
        SetUpSequences();
        RotatePortal();
        InvokeRepeating("GolemCollectCheck", 2.0f, 1.0f);
	}
    private void SetUpSequences() {
        //Transforms
        t1 = entryPortal.transform;
        t2 = exitPortal.transform;

        //Entry Portal Sequence
        entrySeq = DOTween.Sequence();
        entrySeq.Append(t1.DORotateQuaternion(entrySpin1, 0.5f));
        entrySeq.Append(t1.DORotateQuaternion(entrySpin2, 0.5f));

        //Exit portal Sequence
        exitSeq = DOTween.Sequence();
        exitSeq.SetRecyclable(true);
        exitSeq.SetAutoKill(false);
        exitSeq.Append(t2.DORotateQuaternion(exitSpin1, 0.5f));
        exitSeq.Append(t2.DORotateQuaternion(exitSpin2, 0.5f));
        exitSeq.SetLoops(-1);
    }
	
	private void GolemCollectCheck() {
        Debug.Log(Mine.ReadyToCollect() + " : mine ready to collect");
        if (Mine.ReadyToCollect()) {
            exitSeq.Play();
        } else {
            exitSeq.Pause();
        }
    }

    private void RotatePortal() {
        entrySeq.SetLoops(-1);
    }
}
