using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rotate : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}

	void FixedUpdate() {
		transform.RotateAround(transform.position, Vector3.up, .5f);
	}
}
