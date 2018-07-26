using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PortalSpin : MonoBehaviour {
    //Portal Visual Behaviour
    public SpriteRenderer entryPortal;
    private Transform t1;
    private Quaternion entrySpin1 = Quaternion.Euler(162.0f, -80.5f, -15.6f);
    private Quaternion entrySpin2 = Quaternion.Euler(180.3f, -75f, 19.39f);
    private Sequence entrySeq;

	// Use this for initialization
	void Start () {
        SetUpSequences();
        //RotatePortal();
        InvokeRepeating("GolemCollectCheck", 2.0f, 1.0f);
	}
    private void SetUpSequences() {
        //Transforms
        t1 = entryPortal.transform;

        //Entry Portal Sequence
        entrySeq = DOTween.Sequence();
        entrySeq.Append(t1.DORotateQuaternion(entrySpin1, 0.5f));
        entrySeq.Append(t1.DORotateQuaternion(entrySpin2, 0.5f));
        entrySeq.SetRecyclable(true);
        entrySeq.SetAutoKill(false);
        entrySeq.SetLoops(-1);
    }
	
	private void GolemCollectCheck() {
        //Debug.Log(Mine.ReadyToCollect() + " : mine ready to collect");
        if (Mine.Instance.ReadyToCollect()) {
            entrySeq.Play();
        } else {
            entrySeq.Pause();
        }
    }

    /*
    private void RotatePortal() {
        entrySeq.SetLoops(-1);
    }
    */
}
