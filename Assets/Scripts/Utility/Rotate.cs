using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rotate : MonoBehaviour {
	public Vector3 Axis = Vector3.up;
	public float Speed = .5f;
	public bool Enable = false;
	
	void FixedUpdate() {
		if (Enable)
			transform.RotateAround(transform.position, Axis, Speed);
	}
}
