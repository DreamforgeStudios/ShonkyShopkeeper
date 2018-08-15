using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rotate : MonoBehaviour {
	public Vector3 Axis = Vector3.up;
	public float Speed = .5f;
	public bool UseUnscaledTime = false;
	public bool Enable = false;

	void Update() {
		if (Enable) {
			float speed = UseUnscaledTime ? Speed * Time.unscaledDeltaTime : Speed * Time.deltaTime;
			transform.RotateAround(transform.position, Axis, speed);
		}
	}
}
