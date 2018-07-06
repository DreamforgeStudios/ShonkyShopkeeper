using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PortalSpin : MonoBehaviour {
    public SpriteRenderer entryPortal, exitPortal;
    private Transform t1, t2;
    private Quaternion spin1 = Quaternion.Euler(162.0f, -80.5f, -15.6f);
    private Quaternion spin2 = Quaternion.Euler(180.3f, -75f, 19.39f);
    private Sequence seq;
	// Use this for initialization
	void Start () {
        t1 = entryPortal.transform;
        t2 = exitPortal.transform;
        seq = DOTween.Sequence();
        seq.Append(t1.DORotateQuaternion(spin1, 0.5f));
        seq.Append(t1.DORotateQuaternion(spin2, 0.5f));
        RotatePortal();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void RotatePortal() {
        seq.SetLoops(-1);
        
    }
}
