using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class MaskWipe : MonoBehaviour {
	public float WipeTime;
	public Ease WipeEase;

	private SpriteMask mask;

	// Use this for initialization
	void Start () {
		mask = GetComponent<SpriteMask>();
	}

	[Button("Do Wipe")]
	public void DoWipe() {
		DOTween.To(() => mask.alphaCutoff, x => mask.alphaCutoff = x, 0, WipeTime).SetEase(WipeEase);
	}
	
	[Button("Do Wipe")]
	public void Reset() {
		mask.alphaCutoff = 1;
	}
}
